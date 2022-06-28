using Application.Common.Exceptions;
using Application.Memberships;
using Application.Registrations;
using Application.Schedules;
using Application.Sessions;
using Application.UnitTests.Common;
using Xunit;

namespace Application.UnitTests.Schedules;

public class UpdateScheduleTests : ScheduleTestBase
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
            Schedule = PrepareScheduleDTO()
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();

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

        UpdateScheduleService updateScheduleService = GetUpdateScheduleService();

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
            Schedule = PrepareScheduleDTO()
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();

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

        UpdateScheduleService updateScheduleService = GetUpdateScheduleService();

        UpdateScheduleRs rs = await updateScheduleService.RunAsync(updateRq);

        ScheduleDTO schedule = await _dtcCollection.ScheduleDTC.SingleByIdAsync(updatedSchedule.Id);

        Assert.Equal(updatedSchedule.BranchId, schedule.BranchId);
        Assert.Equal(updatedSchedule.BranchName, schedule.BranchName);
        Assert.Equal(updatedSchedule.ClassId, schedule.ClassId);
        Assert.Equal(updatedSchedule.ClassName, schedule.ClassName);
        Assert.Equal(updatedSchedule.TrainerId, schedule.TrainerId);
        Assert.Equal(updatedSchedule.TrainerName, schedule.TrainerName);
    }

    public static IEnumerable<object[]> SessionInputTestData =>
        new List<object[]>
        {
            UpdateScheduleTestData.CreateSessionInputTestData1(),
            UpdateScheduleTestData.CreateSessionInputTestData2(),
            UpdateScheduleTestData.CreateSessionInputTestData3(),
        };

    [Theory]
    [MemberData(nameof(SessionInputTestData))]
    public async Task Handle_UpdateTimeInput_UpdatesSchedule(
        SessionInputData initData, SessionInputData updatedData,
        List<DateTime> expectedSessionDates, bool isExpectRegistrationOnFirstSessionDeleted)
    {
        DateTime openingDate = initData.OpeningDate;
        List<DayOfWeek> daysPerWeek = initData.DaysPerWeek;
        int totalSessions = initData.TotalSessions;

        ScheduleDTO schedule = PrepareScheduleDTO();

        schedule.OpeningDate = openingDate;
        schedule.DaysPerWeek = daysPerWeek;
        schedule.TotalSessions = totalSessions;

        CreateScheduleRq createRq = new CreateScheduleRq
        {
            Schedule = schedule
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();

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

        schedule.OpeningDate = updatedData.OpeningDate;
        schedule.TotalSessions = updatedData.TotalSessions;
        schedule.DaysPerWeek = updatedData.DaysPerWeek;

        UpdateScheduleService updateScheduleService = GetUpdateScheduleService();

        UpdateScheduleRq updateRq = new UpdateScheduleRq
        {
            Schedule = schedule
        };

        await updateScheduleService.RunAsync(updateRq);

        List<SessionDTO> updatedSessions = await _dtcCollection.SessionDTC.ListShallowByScheduleIdAsync(schedule.Id);
        Assert.Equal(updatedData.TotalSessions, updatedSessions.Count);
        Assert.True(expectedSessionDates.ToList().All(date => updatedSessions.Any(session => session.Date == date)));

        registration = await _dtcCollection.RegistrationDTC.GetByIdAsync(registration.Id);
        MembershipDTO updatedMembership = await _dtcCollection.MembershipDTC.SingleByMemberIdAsync(TestConstants.MEMBER_1_ID);
        if (isExpectRegistrationOnFirstSessionDeleted)
        {
            Assert.Null(registration);
            Assert.Equal(previousRemainingSessions, updatedMembership.RemainingSessions);
        }
        else
        {
            Assert.NotNull(registration);
            Assert.Equal(previousRemainingSessions - 1, updatedMembership.RemainingSessions);
        }
    }

    [Fact]
    public async Task Handle_UpdateStartTime_ReturnsMessage()
    {
        ScheduleDTO schedule = PrepareScheduleDTO();
        CreateScheduleRq createRq = new CreateScheduleRq
        {
            Schedule = schedule
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();
        await createScheduleService.RunAsync(createRq);

        schedule.StartTime = new TimeSpan(10, 0, 0);

        UpdateScheduleRq updateRq = new UpdateScheduleRq
        {
            Schedule = schedule
        };

        UpdateScheduleService updateScheduleService = GetUpdateScheduleService();
        UpdateScheduleRs updateRs = await updateScheduleService.RunAsync(updateRq);

        Assert.Contains(UpdateScheduleService.MESSAGE_START_TIME_CHANGED, updateRs.Messages);
    }

    [Fact]
    // Test for two messages:
    // - When the schedule is updated via the selected session on UI and that selected session is later deleted, returns one message inform that deletion
    // - One of the deleted session has registration, returns one message informing that user is returned a sessions
    public async Task Handle_OnUpdate_SelectedSession_WithRegistration_Deleted_ReturnsMessages()
    {
        ScheduleDTO schedule = PrepareScheduleDTO();
        CreateScheduleRq createRq = new CreateScheduleRq
        {
            Schedule = schedule
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();
        await createScheduleService.RunAsync(createRq);

        // Add registration to the second session, then later update to delete the sessions
        List<SessionDTO> sessions = await _dtcCollection.SessionDTC.ListShallowByScheduleIdAsync(schedule.Id);

        CreateRegistrationService createRegistrationService = new CreateRegistrationService(
            _context, _userContextMock.Object, _dtcCollection.MembershipDTC, _dtcCollection.RegistrationDTC, _dtcCollection.MemberDTC);

        int selectedSessionId = sessions.First(x => x.Number == 2).Id;
        await createRegistrationService.RunAsync(new CreateRegistrationRq
        {
            SessionId = selectedSessionId,
            MemberId = TestConstants.MEMBER_1_ID
        });

        schedule.DaysPerWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Thursday };

        UpdateScheduleRq updateRq = new UpdateScheduleRq
        {
            Schedule = schedule,
            SelectedSessionId = selectedSessionId
        };

        UpdateScheduleService updateScheduleService = GetUpdateScheduleService();
        UpdateScheduleRs updateRs = await updateScheduleService.RunAsync(updateRq);

        List<string> expectedMessages = new List<string>
        {
            UpdateScheduleService.MESSAGE_SELECTED_SESSION_DELETED,
            UpdateScheduleService.MESSAGE_INFORM_OF_DELETED_REGISTRATION,
        };

        Assert.Equal(expectedMessages, updateRs.Messages);
    }

    [Fact]
    public async Task Handle_UpdateOpeningDateOfStartedSchedule_ThrowsException()
    {
        ScheduleDTO schedule = PrepareScheduleDTO();
        CreateScheduleRq createRq = new CreateScheduleRq
        {
            Schedule = schedule
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();
        await createScheduleService.RunAsync(createRq);

        schedule.OpeningDate = new DateTime(2022, 6, 8);

        UpdateScheduleRq updateRq = new UpdateScheduleRq
        {
            Schedule = schedule
        };

        UpdateScheduleService updateScheduleService = GetUpdateScheduleService();

        await Assert.ThrowsAsync<ServiceException>(async () =>
        {
            await updateScheduleService.RunAsync(updateRq);
        });
    }

    [Theory]
    [MemberData(nameof(RequiredFieldData))]
    public async Task Handle_NotInputRequiredProperty_ThrowException(Func<ScheduleDTO, string> func, object emptyValue)
    {
        CreateScheduleRq createRq = new CreateScheduleRq
        {
            Schedule = PrepareScheduleDTO()
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();
        CreateScheduleRs createRs = await createScheduleService.RunAsync(createRq);

        UpdateScheduleRq updateRq = new UpdateScheduleRq
        {
            Schedule = createRs.Schedule
        };

        string propertyName = func.Invoke(updateRq.Schedule);
        updateRq.Schedule.GetType().GetProperty(propertyName)!.SetValue(updateRq.Schedule, emptyValue);

        UpdateScheduleService updateScheduleService = GetUpdateScheduleService();
        await Assert.ThrowsAsync<ServiceException>(async () =>
        {
            await updateScheduleService.RunAsync(updateRq);
        });
    }

    [Theory]
    [MemberData(nameof(RequiredNavigationData))]
    public async Task Handle_NotInputRequiredNavigationPair_ThrowException(
        Func<ScheduleDTO, string> idPropNameFunc, Func<ScheduleDTO, string> namePropNameFunc)
    {
        CreateScheduleRq createRq = new CreateScheduleRq
        {
            Schedule = PrepareScheduleDTO()
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();
        CreateScheduleRs createRs = await createScheduleService.RunAsync(createRq);

        UpdateScheduleRq updateRq = new UpdateScheduleRq
        {
            Schedule = createRs.Schedule
        };

        string idPropName = idPropNameFunc.Invoke(updateRq.Schedule);
        string namePropName = namePropNameFunc.Invoke(updateRq.Schedule);
        updateRq.Schedule.GetType().GetProperty(idPropName)!.SetValue(updateRq.Schedule, null);
        updateRq.Schedule.GetType().GetProperty(namePropName)!.SetValue(updateRq.Schedule, null);

        UpdateScheduleService updateScheduleService = GetUpdateScheduleService();
        await Assert.ThrowsAsync<ServiceException>(async () =>
        {
            await updateScheduleService.RunAsync(updateRq);
        });
    }

    protected new ScheduleDTO PrepareScheduleDTO()
    {
        return new ScheduleDTO
        {
            Song = TestConstants.SCHEDULE_TEST_SONG,
            OpeningDate = new DateTime(2022, 6, 6),
            StartTime = new TimeSpan(9, 0, 0),
            DaysPerWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
            TotalSessions = 6,
            BranchId = TestConstants.BRANCH_1_ID,
            TrainerId = TestConstants.TRAINER_1_ID,
            ClassId = TestConstants.CLASS_1_ID
        };
    }
}