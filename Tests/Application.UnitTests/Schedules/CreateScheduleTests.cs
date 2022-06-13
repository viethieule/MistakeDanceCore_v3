using Application.Common.Exceptions;
using Application.Schedules;
using Application.Sessions;
using Application.UnitTests.Common;
using Xunit;

namespace Application.UnitTests.Schedules;

public class CreateScheduleTests : ScheduleTestBase
{
    //
    //  Test cases:
    //  1/ [Theory] Create with correct (TotalSessions + DaysPerWeek + OpeningDate): expect correct sessions date
    //  2/ [Theory] Create with correct (DaysPerWeek + OpeningDate) without TotalSessions: expect correct sessions date
    //  3/ [Theory] Create: expect correct field values inserted
    //  4/ [Theory] or [Multiple test cases] Create tests with all rules in ScheduleValidators
    //

    [Theory]
    [InlineData
    (
        // Inputs
        "2022-06-01", 4, new DayOfWeek[] { DayOfWeek.Wednesday, DayOfWeek.Saturday },
        // Expected
        new string[] { "2022-06-01", "2022-06-04", "2022-06-08", "2022-06-11" }
    )]
    [InlineData
    (
        // Inputs
        "2022-06-01", 3, new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
        // Expected
        new string[] { "2022-06-01", "2022-06-03", "2022-06-06" }
    )]
    [InlineData
    (
        // Inputs
        "2022-06-01", 1, new DayOfWeek[] { DayOfWeek.Wednesday },
        // Expected
        new string[] { "2022-06-01" }
    )]
    [InlineData
    (
        // Inputs
        "2022-06-04", 5, new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday },
        // Expected
        new string[] { "2022-06-04", "2022-06-05", "2022-06-11", "2022-06-12", "2022-06-18" }
    )]
    // Create with correct (TotalSessions + DaysPerWeek + OpeningDate): expect correct sessions date
    public async Task Handle_GivenCorrectTimeInput_CreatesSchedule_WithCorrectSessions(
        DateTime openingDate, int totalSessions, DayOfWeek[] daysPerWeek,
        string[] expectedSessionDateStrings
    )
    {
        CreateScheduleRq rq = new CreateScheduleRq()
        {
            Schedule = PrepareScheduleDTO()
        };

        rq.Schedule.TotalSessions = totalSessions;
        rq.Schedule.DaysPerWeek = daysPerWeek.ToList();
        rq.Schedule.OpeningDate = openingDate;

        CreateScheduleService createScheduleService = GetCreateScheduleService();

        CreateScheduleRs rs = await createScheduleService.RunAsync(rq);
        List<SessionDTO> sessions = rs.Sessions;

        List<DateTime> expectedSessionDates = expectedSessionDateStrings.Select(x => DateTime.Parse(x)).ToList();

        Assert.Equal(totalSessions, sessions.Count);
        Assert.True(expectedSessionDates.ToList().All(date => sessions.Any(session => session.Date == date)));
    }

    [Theory]
    [InlineData
    (
        "2022-06-04",
        new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday },
        "2022-06-04"
    )]
    [InlineData
    (
        "2022-06-05",
        new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday },
        "2022-06-05"
    )]
    [InlineData
    (
        "2022-06-01",
        new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
        "2022-06-01"
    )]
    // Create with correct (DaysPerWeek + OpeningDate) without TotalSessions: expect 1 session
    public async Task Handle_GivenCorrectTimeInput_WithoutTotalSession_CreatesSchedule_WithCorrectSessions(
        DateTime openingDate, DayOfWeek[] daysPerWeek, DateTime expectedOneSessionDate
    )
    {
        CreateScheduleRq rq = new CreateScheduleRq()
        {
            Schedule = PrepareScheduleDTO()
        };

        rq.Schedule.TotalSessions = null;
        rq.Schedule.DaysPerWeek = daysPerWeek.ToList();
        rq.Schedule.OpeningDate = openingDate;

        CreateScheduleService createScheduleService = GetCreateScheduleService();

        CreateScheduleRs rs = await createScheduleService.RunAsync(rq);
        List<SessionDTO> sessions = rs.Sessions;

        Assert.Single(sessions);
        Assert.Equal(expectedOneSessionDate, sessions.First().Date);
    }

    [Fact]
    public async Task Handle_GivenCorrectInput_CreatesSchedule_WithCorrectValues()
    {
        CreateScheduleRq rq = new CreateScheduleRq()
        {
            Schedule = PrepareScheduleDTO()
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();

        CreateScheduleRs rs = await createScheduleService.RunAsync(rq);
        ScheduleDTO schedule = rs.Schedule;

        Assert.NotEqual(0, schedule.Id);
        Assert.Equal(rq.Schedule.Song, schedule.Song);
        Assert.Equal(rq.Schedule.StartTime.Hours, schedule.StartTime.Hours);
        Assert.Equal(rq.Schedule.StartTime.Minutes, schedule.StartTime.Minutes);
        Assert.Equal(rq.Schedule.StartTime.Seconds, schedule.StartTime.Seconds);
        Assert.Equal(rq.Schedule.BranchName, schedule.BranchName);
        Assert.Equal(rq.Schedule.ClassName, schedule.ClassName);
        Assert.Equal(rq.Schedule.TrainerName, schedule.TrainerName);
        Assert.Equal(rq.Schedule.TotalSessions, schedule.TotalSessions);
        Assert.Equal(rq.Schedule.DaysPerWeek.Count, schedule.DaysPerWeek.Count);
        Assert.Equal(rq.Schedule.OpeningDate, schedule.OpeningDate);
        Assert.Equal(TestConstants.TEST_USER_NAME, schedule.CreatedBy);
        Assert.Equal(TestConstants.TEST_USER_NAME, schedule.UpdatedBy);
        Assert.Equal(DateTime.Now.Date, schedule.CreatedDate.Date);
        Assert.Equal(DateTime.Now.Date, schedule.UpdatedDate.Date);
    }

    [Fact]
    public async Task Handle_GivenCorrectInput_CreateSchedule_WithNewlyCreatedNavigation()
    {
        CreateScheduleRq rq = new CreateScheduleRq()
        {
            Schedule = PrepareScheduleDTO()
        };

        rq.Schedule.ClassId = null;
        rq.Schedule.TrainerId = null;
        rq.Schedule.BranchId = null;

        rq.Schedule.ClassName = "New test class name";
        rq.Schedule.TrainerName = "New test trainer name";
        rq.Schedule.BranchName = "New test branch name";

        CreateScheduleService createScheduleService = GetCreateScheduleService();

        CreateScheduleRs rs = await createScheduleService.RunAsync(rq);
        ScheduleDTO schedule = rs.Schedule;

        Assert.NotEqual(0, schedule.Id);
        Assert.Equal(rq.Schedule.Song, schedule.Song);
        Assert.Equal(rq.Schedule.StartTime.Hours, schedule.StartTime.Hours);
        Assert.Equal(rq.Schedule.StartTime.Minutes, schedule.StartTime.Minutes);
        Assert.Equal(rq.Schedule.StartTime.Seconds, schedule.StartTime.Seconds);
        Assert.Equal(rq.Schedule.BranchName, schedule.BranchName);
        Assert.Equal(rq.Schedule.ClassName, schedule.ClassName);
        Assert.Equal(rq.Schedule.TrainerName, schedule.TrainerName);
        Assert.Equal(rq.Schedule.TotalSessions, schedule.TotalSessions);
        Assert.Equal(rq.Schedule.DaysPerWeek.Count, schedule.DaysPerWeek.Count);
        Assert.Equal(rq.Schedule.OpeningDate, schedule.OpeningDate);
        Assert.Equal(TestConstants.TEST_USER_NAME, schedule.CreatedBy);
        Assert.Equal(TestConstants.TEST_USER_NAME, schedule.UpdatedBy);
        Assert.Equal(DateTime.Now.Date, schedule.CreatedDate.Date);
        Assert.Equal(DateTime.Now.Date, schedule.UpdatedDate.Date);
    }

    public static IEnumerable<object[]> RequiredFieldData =>
        new List<object[]>
        {
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.Song)), null! },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.Song)), string.Empty },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.Song)), " " },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.StartTime)), default(TimeSpan) },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.OpeningDate)), default(DateTime) },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.DaysPerWeek)), null!},
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.DaysPerWeek)), new List<DayOfWeek>() },
        };

    [Theory]
    [MemberData(nameof(RequiredFieldData))]
    public async Task Handle_NotInputRequiredProperty_ThrowException(Func<ScheduleDTO, string> func, object emptyValue)
    {
        CreateScheduleRq rq = new CreateScheduleRq()
        {
            Schedule = PrepareScheduleDTO()
        };

        string propertyName = func.Invoke(rq.Schedule);
        rq.Schedule.GetType().GetProperty(propertyName)!.SetValue(rq.Schedule, emptyValue);

        CreateScheduleService createScheduleService = GetCreateScheduleService();

        await Assert.ThrowsAsync<ServiceException>(async () =>
        {
            await createScheduleService.RunAsync(rq);
        });
    }

    public static IEnumerable<object[]> RequiredNavigationData =>
        new List<object[]>
        {
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.ClassId)), (Func<ScheduleDTO, string>)(x => nameof(x.ClassName)) },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.TrainerId)), (Func<ScheduleDTO, string>)(x => nameof(x.TrainerName)) },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.BranchId)), (Func<ScheduleDTO, string>)(x => nameof(x.BranchName)) },
        };

    [Theory]
    [MemberData(nameof(RequiredNavigationData))]
    public async Task Handle_NotInputRequiredNavigationPair_ThrowException(
        Func<ScheduleDTO, string> idPropNameFunc, Func<ScheduleDTO, string> namePropNameFunc)
    {
        CreateScheduleRq rq = new CreateScheduleRq
        {
            Schedule = PrepareScheduleDTO()
        };

        string idPropName = idPropNameFunc.Invoke(rq.Schedule);
        string namePropName = namePropNameFunc.Invoke(rq.Schedule);
        rq.Schedule.GetType().GetProperty(idPropName)!.SetValue(rq.Schedule, null);
        rq.Schedule.GetType().GetProperty(namePropName)!.SetValue(rq.Schedule, null);

        CreateScheduleService createScheduleService = GetCreateScheduleService();

        await Assert.ThrowsAsync<ServiceException>(async () =>
        {
            await createScheduleService.RunAsync(rq);
        });
    }

    [Theory]
    [InlineData("2022-06-07", new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday })]
    [InlineData("2022-06-04", new DayOfWeek[] { DayOfWeek.Friday, DayOfWeek.Sunday })]
    public async Task Handle_OpeningDateIsNotInDaysPerWeek_ThrowException(DateTime openingDate, DayOfWeek[] daysPerWeek)
    {
        CreateScheduleRq rq = new CreateScheduleRq()
        {
            Schedule = PrepareScheduleDTO()
        };

        rq.Schedule.OpeningDate = openingDate;
        rq.Schedule.DaysPerWeek = daysPerWeek.ToList();

        CreateScheduleService createScheduleService = GetCreateScheduleService();

        ServiceException exception = await Assert.ThrowsAsync<ServiceException>(async () =>
        {
            await createScheduleService.RunAsync(rq);
        });

        Assert.Contains(ScheduleValidator.MESSAGE_OPENING_DATE_NOT_MATCH_WITH_DAYS_PER_WEEK, exception.Message);
    }

    private ScheduleDTO PrepareScheduleDTO()
    {
        return new ScheduleDTO
        {
            Song = "Test song",
            StartTime = new TimeSpan(9, 0, 0),
            BranchId = TestConstants.BRANCH_1_ID,
            ClassId = TestConstants.CLASS_1_ID,
            TrainerId = TestConstants.TRAINER_1_ID,
            OpeningDate = new(2022, 5, 9),
            DaysPerWeek = new() { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
            TotalSessions = 3,
        };
    }
}
