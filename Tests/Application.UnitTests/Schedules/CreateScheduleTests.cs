using Application.Common.Exceptions;
using Application.Schedules;
using Application.Sessions;
using Application.UnitTests.Common;
using Xunit;

namespace Application.UnitTests.Schedules;

public class CreateScheduleTests : TestBase
{
    //
    //  Test cases:
    //  1/ [Theory] Create with correct (TotalSessions + DaysPerWeek + OpeningDate): expect correct sessions date
    //  2/ [Theory]  Create with correct (DaysPerWeek + OpeningDate) without TotalSessions: expect correct sessions date
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
        DateTime openingDate = new DateTime(2022, 5, 9);
        CreateScheduleRq rq = new CreateScheduleRq()
        {
            Schedule = PrepareScheduleDTO()
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();

        CreateScheduleRs rs = await createScheduleService.RunAsync(rq);
        ScheduleDTO schedule = rq.Schedule;

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
        Assert.Equal(TEST_USER_NAME, schedule.CreatedBy);
        Assert.Equal(TEST_USER_NAME, schedule.UpdatedBy);
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
    public async Task Handle_GivenScheduleForm_WithoutRequiredProperty_ThrowException(Func<ScheduleDTO, string> func, object emptyValue)
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

    private ScheduleDTO PrepareScheduleDTO()
    {
        return new ScheduleDTO
        {
            Song = "Test song",
            StartTime = new TimeSpan(9, 0, 0),
            BranchName = "Test branch",
            ClassName = "Test class",
            TrainerName = "Test trainer",
            OpeningDate = new(2022, 5, 9),
            DaysPerWeek = new() { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
            TotalSessions = 3,
        };
    }

    private CreateScheduleService GetCreateScheduleService()
    {
        return new CreateScheduleService(
            _context, _userContextMock.Object, _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC,
            _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC, _dtcCollection.SessionDTC
        );
    }
}
