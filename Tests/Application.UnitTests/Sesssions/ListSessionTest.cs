using Application.Schedules;
using Application.UnitTests.Common;
using Xunit;

namespace Application.UnitTests.Sesssions;
public class ListSessionTest : TestBase
{
    public ListSessionTest() : base()
    {
    }

    [Fact]
    public async Task Handle_ListSessionsByDate_ReturnCorrectData()
    {
    }

    private static List<ScheduleDTO> ScheduleData =>
        new List<ScheduleDTO>
        {
            // Out of date range (before)
            PrepareScheduleDTO(
                1000, new DateTime(2022, 15, 6), new TimeSpan(9, 0, 0), new List<DayOfWeek> { DayOfWeek.Wednesday, DayOfWeek.Sunday }, 2),

            // In date range, with a few behind
            PrepareScheduleDTO(
                2000, new DateTime(2022, 13, 6), new TimeSpan(15, 0, 0), new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday }, 6),
            PrepareScheduleDTO(
                3000, new DateTime(2022, 14, 6), new TimeSpan(10, 0, 0), new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday }, 3),

            // In date range, with a few after
            // With a session in the left limit
            PrepareScheduleDTO(
                4000, new DateTime(2022, 20, 6), new TimeSpan(19, 0, 0), new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Thursday }, 5),

            // In date range, with a few after
            // With a session in the right limit
            PrepareScheduleDTO(
                5000, new DateTime(2022, 24, 6), new TimeSpan(20, 0, 0), new List<DayOfWeek> { DayOfWeek.Friday, DayOfWeek.Sunday }, 5),

            // Out of date range (after)
            PrepareScheduleDTO(
                6000, new DateTime(2022, 28, 6), new TimeSpan(21, 0, 0), new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday }, 5),
        };

    public static ScheduleDTO PrepareScheduleDTO(
        int id, DateTime openingDate, TimeSpan startTime, List<DayOfWeek> daysPerWeek, int? totalSessions)
    {
        return new ScheduleDTO
        {
            Id = id,
            Song = "Test song",
            OpeningDate = openingDate,
            StartTime = startTime,
            DaysPerWeek = daysPerWeek,
            TotalSessions = totalSessions,
            BranchId = TestConstants.BRANCH_1_ID,
            ClassId = TestConstants.CLASS_1_ID,
            TrainerId = TestConstants.TRAINER_1_ID
        };
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}