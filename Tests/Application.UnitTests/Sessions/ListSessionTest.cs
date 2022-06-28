using Application.Common.Helpers;
using Application.Schedules;
using Application.Sessions;
using Application.UnitTests.Common;
using Xunit;

namespace Application.UnitTests.Sessions;
public class ListSessionTest : TestBase
{
    [Fact]
    public async Task Handle_ListSessionsByDate_ReturnCorrectData()
    {
        foreach (ScheduleDTO scheduleDTO in SchedulesData)
        {
            CreateScheduleRq createRq = new CreateScheduleRq
            {
                Schedule = scheduleDTO
            };

            CreateScheduleService createScheduleService = GetCreateScheduleService();
            await createScheduleService.RunAsync(createRq);
        }

        ListSessionsRq rq = new ListSessionsRq
        {
            Start = new DateTime(2022, 6, 20)
        };

        ListSessionsService listSessionsService = GetListSessionService();
        ListSessionsRs rs = await listSessionsService.RunAsync(rq);

        List<SessionDTO> sessions = rs.Sessions;

        DateTime end = rq.Start.AddDays(7).AddSeconds(-1);
        List<SessionDTO> expectedSessions = SessionsData.Where(x => x.Date <= end && x.Date >= rq.Start).ToList();

        // Assert all date of sessions are correctly fetched
        Assert.Equal(expectedSessions.Count, sessions.Count);
        Assert.True(sessions.All(s =>
            expectedSessions.Any(es => es.Date == s.Date && es.Number == s.Number && es.ScheduleId == s.ScheduleId)));

        // Assert schedule info is correct
        foreach (SessionDTO session in sessions)
        {
            ScheduleDTO schedule = SchedulesData.First(x => x.Id == session.ScheduleId);
            Assert.Equal(schedule.Song, session.Song);
            Assert.Equal(schedule.BranchId, session.BranchId);
            Assert.Equal(schedule.ClassId, session.ClassId);
            Assert.Equal(schedule.TrainerId, session.TrainerId);
            Assert.Equal(schedule.TotalSessions, session.TotalSessions);
            Assert.Equal(schedule.StartTime, session.StartTime);
            Assert.Equal(schedule.OpeningDate, session.OpeningDate);
            Assert.Equal(schedule.DaysPerWeek.Count, session.DaysPerWeek.Count);
            Assert.True(session.DaysPerWeek.All(d => schedule.DaysPerWeek.Contains(d)));
        }
    }

    private CreateScheduleService GetCreateScheduleService()
    {
        return new CreateScheduleService(
            _context, _userContextMock.Object, _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC,
            _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC, _dtcCollection.SessionDTC);
    }

    private ListSessionsService GetListSessionService()
    {
        return new ListSessionsService(_dtcCollection.SessionDTC);
    }

    private static List<ScheduleDTO> SchedulesData =>
        new List<ScheduleDTO>
        {
            // Out of date range (before)
            PrepareScheduleDTO(
                1000, new DateTime(2022, 6, 15), new TimeSpan(9, 0, 0), new List<DayOfWeek> { DayOfWeek.Wednesday, DayOfWeek.Sunday }, 2),

            // Out of date range (after)
            PrepareScheduleDTO(
                2000, new DateTime(2022, 6, 28), new TimeSpan(21, 0, 0), new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday }, 5),

            // In date range, with a few behind
            PrepareScheduleDTO(
                3000, new DateTime(2022, 6, 13), new TimeSpan(15, 0, 0), new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday }, 6),
            PrepareScheduleDTO(
                4000, new DateTime(2022, 6, 14), new TimeSpan(10, 0, 0), new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday }, 3),

            // In date range, with a few after
            // With a session in the left limit
            PrepareScheduleDTO(
                5000, new DateTime(2022, 6, 20), new TimeSpan(19, 0, 0), new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Thursday }, 5),

            // In date range, with a few after
            // With a session in the right limit
            PrepareScheduleDTO(
                6000, new DateTime(2022, 6, 24), new TimeSpan(22, 0, 0), new List<DayOfWeek> { DayOfWeek.Friday, DayOfWeek.Sunday }, 5)
        };

    private static List<SessionDTO> SessionsData
    {
        get
        {
            List<SessionDTO> sessions = new();
            foreach (ScheduleDTO schedule in SchedulesData)
            {
                sessions.AddRange(SessionsGenerator.Generate(schedule));
            }
            return sessions;
        }
    }

    private static ScheduleDTO PrepareScheduleDTO(
        int id, DateTime openingDate, TimeSpan startTime, List<DayOfWeek> daysPerWeek, int? totalSessions)
    {
        return new ScheduleDTO
        {
            Id = id,
            Song = TestConstants.SCHEDULE_TEST_SONG,
            OpeningDate = openingDate,
            StartTime = startTime,
            DaysPerWeek = daysPerWeek,
            TotalSessions = totalSessions,
            BranchId = TestConstants.BRANCH_1_ID,
            ClassId = TestConstants.CLASS_1_ID,
            TrainerId = TestConstants.TRAINER_1_ID
        };
    }
}