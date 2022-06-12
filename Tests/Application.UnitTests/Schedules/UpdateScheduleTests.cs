using Application.Memberships;
using Application.Registrations;
using Application.Schedules;
using Application.Sessions;
using Application.UnitTests.Common;
using Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.UnitTests.Schedules;

public class UpdateScheduleTests : TestBase
{
    //
    // Test cases:
    // - Update correct basic data
    // - Update correct basic data + navigation data (trainer / class /branch)
    // - Update with correct (TotalSessions + DaysPerWeek + OpeningDate), expect:
    // --- Corrected new sessions created
    // --- Previous sessions removed
    // ------ Remove registration
    // ------ Increase membership session back to member
    // - Messages:
    // --- Start Time changed
    // --- Selected sessions deleted
    // --- Inform member of session deleted
    // - Validators
    // - Cannot update opening date of started schedule
    //

    [Fact]
    public async Task Handle_ChangeBasicInput_NotIncludeTimeAndSessions_UpdatesSchedule_WithCorrectValues()
    {
        CreateScheduleRq createRq = new CreateScheduleRq
        {
            Schedule = new ScheduleDTO
            {
                Song = "Test song",
                OpeningDate = new DateTime(2022, 6, 6),
                StartTime = new TimeSpan(9, 0, 0),
                DaysPerWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
                TotalSessions = 6,
                BranchId = TestConstants.BRANCH_1_ID,
                TrainerId = TestConstants.TRAINER_1_ID,
                ClassId = TestConstants.CLASS_1_ID
            }
        };

        CreateScheduleService createScheduleService = new CreateScheduleService(
            _context, _userContextMock.Object, _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC,
            _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC, _dtcCollection.SessionDTC
        );

        CreateScheduleRs createRs = await createScheduleService.RunAsync(createRq);
        ScheduleDTO initialSchedule = createRs.Schedule;

        ScheduleDTO updatedSchedule = new ScheduleDTO
        {
            Id = initialSchedule.Id,
            Song = "Test song 2",
            OpeningDate = initialSchedule.OpeningDate,
            TotalSessions = initialSchedule.TotalSessions,
            DaysPerWeek = initialSchedule.DaysPerWeek,
            StartTime = new TimeSpan(10, 0, 0),
            BranchId = TestConstants.BRANCH_2_ID,
            TrainerId = TestConstants.TRAINER_2_ID,
            ClassId = TestConstants.CLASS_2_ID
        };

        UpdateScheduleRq updateRq = new UpdateScheduleRq
        {
            Schedule = updatedSchedule
        };

        UpdateScheduleService updateScheduleService = new UpdateScheduleService(
            _context, _userContextMock.Object,
            _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC, _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC,
            _dtcCollection.SessionDTC, _dtcCollection.RegistrationDTC, _dtcCollection.PackageDTC, _dtcCollection.MembershipDTC
        );

        UpdateScheduleRs rs = await updateScheduleService.RunAsync(updateRq);
        ScheduleDTO schedule = await _dtcCollection.ScheduleDTC.SingleByIdAsync(updatedSchedule.Id);

        Assert.NotEqual(0, schedule.Id);
        Assert.Equal(updatedSchedule.Song, schedule.Song);
        Assert.Equal(updatedSchedule.StartTime.Hours, schedule.StartTime.Hours);
        Assert.Equal(updatedSchedule.StartTime.Minutes, schedule.StartTime.Minutes);
        Assert.Equal(updatedSchedule.StartTime.Seconds, schedule.StartTime.Seconds);
        Assert.Equal(TestConstants.BRANCH_2_NAME, schedule.BranchName);
        Assert.Equal(TestConstants.CLASS_2_NAME, schedule.ClassName);
        Assert.Equal(TestConstants.TRAINER_2_NAME, schedule.TrainerName);
        Assert.Equal(updatedSchedule.TotalSessions, schedule.TotalSessions);
        Assert.Equal(updatedSchedule.DaysPerWeek.Count, schedule.DaysPerWeek.Count);
        Assert.Equal(updatedSchedule.OpeningDate, schedule.OpeningDate);
        Assert.Equal(TestConstants.TEST_USER_NAME, schedule.CreatedBy);
        Assert.Equal(TestConstants.TEST_USER_NAME, schedule.UpdatedBy);
        Assert.Equal(DateTime.Now.Date, schedule.CreatedDate.Date);
        Assert.Equal(DateTime.Now.Date, schedule.UpdatedDate.Date);
        Assert.True(schedule.UpdatedDate > schedule.CreatedDate);
    }

    [Fact]
    public async Task Handle_ChangeNavigationInput_UpdatesSchedule_WithCorrectValues()
    {
        CreateScheduleRq createRq = new CreateScheduleRq
        {
            Schedule = new ScheduleDTO
            {
                Song = "Test song",
                OpeningDate = new DateTime(2022, 6, 6),
                StartTime = new TimeSpan(9, 0, 0),
                DaysPerWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
                TotalSessions = 6,
                BranchId = TestConstants.BRANCH_1_ID,
                TrainerId = TestConstants.TRAINER_1_ID,
                ClassId = TestConstants.CLASS_1_ID
            }
        };

        CreateScheduleService createScheduleService = new CreateScheduleService(
            _context, _userContextMock.Object, _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC,
            _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC, _dtcCollection.SessionDTC
        );

        CreateScheduleRs createRs = await createScheduleService.RunAsync(createRq);
        ScheduleDTO updatedSchedule = createRs.Schedule;

        updatedSchedule.BranchId = null;
        updatedSchedule.ClassId = null;
        updatedSchedule.TrainerId = null;

        updatedSchedule.BranchName = "New branch for this update";
        updatedSchedule.ClassName = "New class for this update";
        updatedSchedule.TrainerName = "New trainer for this update";

        UpdateScheduleRq updateRq = new UpdateScheduleRq
        {
            Schedule = updatedSchedule
        };

        UpdateScheduleService updateScheduleService = new UpdateScheduleService(
            _context, _userContextMock.Object,
            _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC, _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC,
            _dtcCollection.SessionDTC, _dtcCollection.RegistrationDTC, _dtcCollection.PackageDTC, _dtcCollection.MembershipDTC
        );

        UpdateScheduleRs rs = await updateScheduleService.RunAsync(updateRq);

        ScheduleDTO schedule = await _dtcCollection.ScheduleDTC.SingleByIdAsync(updatedSchedule.Id);

        Assert.Equal(updatedSchedule.BranchId, schedule.BranchId);
        Assert.Equal(updatedSchedule.BranchName, schedule.BranchName);
        Assert.Equal(updatedSchedule.ClassId, schedule.ClassId);
        Assert.Equal(updatedSchedule.ClassName, schedule.ClassName);
        Assert.Equal(updatedSchedule.TrainerId, schedule.TrainerId);
        Assert.Equal(updatedSchedule.TrainerName, schedule.TrainerName);
    }

    [Fact]
    public async Task Handle_UpdateTimeInput_UpdatesSchedule()
    {
        DateTime openingDate = DateTime.Now.AddDays(1);
        List<DayOfWeek> daysPerWeek = new List<DayOfWeek> { openingDate.DayOfWeek, openingDate.AddDays(2).DayOfWeek, openingDate.AddDays(4).DayOfWeek };
        int totalSessions = 6;

        ScheduleDTO schedule = new ScheduleDTO
        {
            Song = "Test song",
            OpeningDate = openingDate,
            StartTime = new TimeSpan(9, 0, 0),
            DaysPerWeek = daysPerWeek,
            TotalSessions = totalSessions,
            BranchId = TestConstants.BRANCH_1_ID,
            TrainerId = TestConstants.TRAINER_1_ID,
            ClassId = TestConstants.CLASS_1_ID
        };

        CreateScheduleRq createRq = new CreateScheduleRq
        {
            Schedule = schedule
        };

        CreateScheduleService createScheduleService = new CreateScheduleService(
            _context, _userContextMock.Object, _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC,
            _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC, _dtcCollection.SessionDTC
        );

        await createScheduleService.RunAsync(createRq);

        List<SessionDTO> currentSessions = await _dtcCollection.SessionDTC.ListShallowByScheduleIdAsync(schedule.Id);

        MembershipDTO membership = await _dtcCollection.MembershipDTC.SingleByMemberIdAsync(TestConstants.MEMBER_1_ID);
        int previousRemainingSessions = membership.RemainingSessions;
        CreateRegistrationService createRegistrationService = new CreateRegistrationService(
            _context, _userContextMock.Object, _dtcCollection.MembershipDTC, _dtcCollection.RegistrationDTC, _dtcCollection.MemberDTC);

        await createRegistrationService.RunAsync(new CreateRegistrationRq
        {
            SessionId = currentSessions.First().Id,
            MemberId = TestConstants.MEMBER_1_ID
        });

        List<RegistrationDTO> registrations = await _dtcCollection.RegistrationDTC.ListShallowBySessionIdsAsync(
            new List<int> { currentSessions.First().Id });

        RegistrationDTO registration = registrations.First();
        membership = await _dtcCollection.MembershipDTC.SingleByMemberIdAsync(TestConstants.MEMBER_1_ID);
        Assert.Equal(previousRemainingSessions - 1, membership.RemainingSessions);

        DateTime newOpeningDate = openingDate.AddDays(1);
        schedule.OpeningDate = newOpeningDate;
        schedule.TotalSessions = 5;
        schedule.DaysPerWeek = new List<DayOfWeek> { newOpeningDate.DayOfWeek, newOpeningDate.AddDays(2).DayOfWeek };

        UpdateScheduleService updateScheduleService = new UpdateScheduleService(
            _context, _userContextMock.Object,
            _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC, _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC,
            _dtcCollection.SessionDTC, _dtcCollection.RegistrationDTC, _dtcCollection.PackageDTC, _dtcCollection.MembershipDTC
        );

        UpdateScheduleRq updateRq = new UpdateScheduleRq
        {
            Schedule = schedule
        };

        await updateScheduleService.RunAsync(updateRq);

        List<DateTime> expectedSessionDates = new List<DateTime>
        {
            newOpeningDate,
            newOpeningDate.AddDays(2),
            newOpeningDate.AddDays(7),
            newOpeningDate.AddDays(2 + 7),
            newOpeningDate.AddDays(7 + 7),
        };

        List<SessionDTO> updatedSessions = await _dtcCollection.SessionDTC.ListShallowByScheduleIdAsync(schedule.Id);
        Assert.Equal(schedule.TotalSessions, updatedSessions.Count);
        Assert.True(expectedSessionDates.ToList().All(date => updatedSessions.Any(session => session.Date == date)));

        registration = await _dtcCollection.RegistrationDTC.GetByIdAsync(registration.Id);
        MembershipDTO updatedMembership = await _dtcCollection.MembershipDTC.SingleByMemberIdAsync(TestConstants.MEMBER_1_ID);
        Assert.Null(registration);
        Assert.Equal(previousRemainingSessions, updatedMembership.RemainingSessions);
    }
}