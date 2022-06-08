using Application.Schedules;
using Application.UnitTests.Common;
using Xunit;

namespace Application.UnitTests.Schedules;

public class UpdateScheduleTests : TestBase
{
    //
    // Test cases
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
        ScheduleDTO schedule = await _dtcCollection.ScheduleDTC.SingleWithIncludeByIdAsync(updatedSchedule.Id);

        Assert.NotEqual(0, schedule.Id);
        Assert.Equal(updatedSchedule.Song, schedule.Song);
        Assert.Equal(updatedSchedule.StartTime.Hours, schedule.StartTime.Hours);
        Assert.Equal(updatedSchedule.StartTime.Minutes, schedule.StartTime.Minutes);
        Assert.Equal(updatedSchedule.StartTime.Seconds, schedule.StartTime.Seconds);
        Assert.Equal(updatedSchedule.BranchName, schedule.BranchName);
        Assert.Equal(updatedSchedule.ClassName, schedule.ClassName);
        Assert.Equal(updatedSchedule.TrainerName, schedule.TrainerName);
        Assert.Equal(updatedSchedule.TotalSessions, schedule.TotalSessions);
        Assert.Equal(updatedSchedule.DaysPerWeek.Count, schedule.DaysPerWeek.Count);
        Assert.Equal(updatedSchedule.OpeningDate, schedule.OpeningDate);
        Assert.Equal(TestConstants.TEST_USER_NAME, schedule.CreatedBy);
        Assert.Equal(TestConstants.TEST_USER_NAME, schedule.UpdatedBy);
        Assert.Equal(DateTime.Now.Date, schedule.CreatedDate.Date);
        Assert.Equal(DateTime.Now.Date, schedule.UpdatedDate.Date);
        Assert.True(schedule.UpdatedDate > schedule.CreatedDate);
    }
}