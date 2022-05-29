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
    [Fact]
    public async Task Handle_GivenCorrectTimeInput_CreatesSchedule_WithCorrectSessions()
    {
        DateTime openingDate = new DateTime(2022, 5, 9);
        CreateScheduleRq rq = new CreateScheduleRq()
        {
            Schedule = new()
            {
                Song = "Test song",
                StartTime = new TimeSpan(9, 0, 0),
                BranchName = "Test branch",
                ClassName = "Test class",
                TrainerName = "Test trainer",
                TotalSessions = 3,
                DaysPerWeek = new() { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
                OpeningDate = openingDate
            }            
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();

        CreateScheduleRs rs = await createScheduleService.RunAsync(rq);
        ScheduleDTO schedule = rq.Schedule;
        List<SessionDTO> sessions = rs.Sessions;

        Assert.Equal(3, sessions.Count);
        List<DateTime> expectedSessionDates = new List<DateTime> { openingDate, openingDate.AddDays(2), openingDate.AddDays(4) };
        Assert.True(expectedSessionDates.All(date => sessions.Any(session => session.Date == date)));

        Assert.NotEqual(0, schedule.Id);

        Assert.Equal(TEST_USER_NAME, schedule.CreatedBy);
    }

    [Fact]
    public async Task Handle_GivenCorrectInput_CreatesSchedule_WithCorrectValues()
    {
        DateTime openingDate = new DateTime(2022, 5, 9);
        CreateScheduleRq rq = new CreateScheduleRq()
        {
            Schedule = new()
            {
                Song = "Test song",
                StartTime = new TimeSpan(9, 0, 0),
                BranchName = "Test branch",
                ClassName = "Test class",
                TrainerName = "Test trainer",
                TotalSessions = 3,
                DaysPerWeek = new() { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
                OpeningDate = openingDate
            }            
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();

        CreateScheduleRs rs = await createScheduleService.RunAsync(rq);
        ScheduleDTO schedule = rq.Schedule;

        Assert.NotEqual(0, schedule.Id);
        Assert.Equal("Test song", schedule.Song);
        Assert.Equal(9, schedule.StartTime.Hours);
        Assert.Equal(0, schedule.StartTime.Minutes);
        Assert.Equal(0, schedule.StartTime.Seconds);
        Assert.Equal("Test branch", schedule.BranchName);
        Assert.Equal("Test class", schedule.ClassName);
        Assert.Equal("Test trainer", schedule.TrainerName);
        Assert.Equal(3, schedule.TotalSessions);
        Assert.Equal(3, schedule.DaysPerWeek.Count);
        Assert.Equal(openingDate, schedule.OpeningDate);
        Assert.Equal(TEST_USER_NAME, schedule.CreatedBy);
        Assert.Equal(TEST_USER_NAME, schedule.UpdatedBy);
        Assert.Equal(DateTime.Now.Date, schedule.CreatedDate.Date);
        Assert.Equal(DateTime.Now.Date, schedule.UpdatedDate.Date);
    }

    [Fact]
    // TODO: Change to theory with inline data
    public async Task Handle_GivenScheduleForm_WithoutSong_ThrowException()
    {
        DateTime openingDate = new DateTime(2022, 5, 9);
        CreateScheduleRq rq = new CreateScheduleRq()
        {
            Schedule = new()
            {
                Song = string.Empty,
                StartTime = new TimeSpan(9, 0, 0),
                BranchName = "Test branch",
                ClassName = "Test class",
                TrainerName = "Test trainer",
                TotalSessions = 3,
                DaysPerWeek = new() { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
                OpeningDate = openingDate
            }
        };

        CreateScheduleService createScheduleService = GetCreateScheduleService();

        await Assert.ThrowsAsync<ServiceException>(async () =>
        {
            await createScheduleService.RunAsync(rq);
        });
    }

    private CreateScheduleService GetCreateScheduleService()
    {
        return new CreateScheduleService(
            _context, _userContextMock.Object, _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC,
            _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC, _dtcCollection.SessionDTC
        );
    }
}
