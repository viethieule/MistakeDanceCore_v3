using Application.Common.Exceptions;
using Application.Schedules;
using Application.Sessions;
using Application.UnitTests.Common;
using Xunit;

namespace Application.UnitTests.Schedules;

public class CreateScheduleTests : TestBase
{
    [Fact]
    public async Task Handle_GivenScheduleForm_CreatesSchedule()
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

        var createScheduleService = new CreateScheduleService(
            _context, _userContextMock.Object, _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC,
            _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC, _dtcCollection.SessionDTC
        );

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

        var createScheduleService = new CreateScheduleService(
            _context, _userContextMock.Object, _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC,
            _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC, _dtcCollection.SessionDTC
        );

        await Assert.ThrowsAsync<ServiceException>(async () =>
        {
            await createScheduleService.RunAsync(rq);
        });
    }
}
