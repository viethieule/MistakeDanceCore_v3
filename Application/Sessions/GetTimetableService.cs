using System.Globalization;
using Application.Common;

namespace Application.Sessions;

public class GetTimetableRq : BaseRequest
{
    public DateTime Start { get; set; }
}

public class GetTimetableRs : BaseResponse
{
    public List<TimetableRow> Timetable { get; set; }
}

public class TimetableRow
{
    public TimetableRow(TimeSpan startTime)
    {
        StartTime = startTime;
        SessionCells = new()
            {
                new TimetableCell(DayOfWeek.Monday),
                new TimetableCell(DayOfWeek.Tuesday),
                new TimetableCell(DayOfWeek.Wednesday),
                new TimetableCell(DayOfWeek.Thursday),
                new TimetableCell(DayOfWeek.Friday),
                new TimetableCell(DayOfWeek.Saturday),
                new TimetableCell(DayOfWeek.Sunday),
            };
    }

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime => StartTime.Add(new TimeSpan(1, 0, 0));
    public string StartToEnd => $"{StartTime.ToString(@"hh\:mm")} - {EndTime.ToString(@"hh\:mm")}";
    public List<TimetableCell> SessionCells { get; set; }
}

public class TimetableCell
{
    public TimetableCell(DayOfWeek dayOfWeek)
    {
        DayOfWeek = dayOfWeek;
        Sessions = new();
    }

    public DayOfWeek DayOfWeek { get; set; }
    public string DayOfWeekString => new CultureInfo("vi-VN").DateTimeFormat.GetDayName(DayOfWeek);
    public List<SessionDTO> Sessions { get; set; }
}

public class GetTimetableService : BaseService<GetTimetableRq, GetTimetableRs>
{
    private readonly ListSessionsService _listSessionsService;

    // TODO: to a db config
    private static HashSet<TimeSpan> DefaultTimeSlots = new HashSet<TimeSpan>
    {
        new TimeSpan(9, 0, 0),
        new TimeSpan(10, 0, 0),
        new TimeSpan(11, 0, 0),
        new TimeSpan(12, 0, 0),
        new TimeSpan(17, 0, 0),
        new TimeSpan(18, 0, 0),
        new TimeSpan(18, 30, 0),
        new TimeSpan(19, 0, 0),
        new TimeSpan(20, 35, 0),
    };

    public GetTimetableService(ListSessionsService listSessionsService)
    {
        _listSessionsService = listSessionsService;
    }

    protected override async Task<GetTimetableRs> DoRunAsync(GetTimetableRq rq)
    {
        ListSessionsRq listSessionsRq = new() { Start = rq.Start };
        ListSessionsRs listSessionsRs = await _listSessionsService.RunAsync(listSessionsRq);

        var rs = new GetTimetableRs();

        List<TimetableRow> seedTimetable = DefaultTimeSlots.Select(timeSlot => new TimetableRow(timeSlot)).ToList();
        rs.Timetable = listSessionsRs.Sessions
            .Aggregate(seedTimetable, (timetable, session) =>
            {
                TimetableRow row = timetable.FirstOrDefault(x => x.StartTime == session.StartTime);
                if (row == null)
                {
                    row = new TimetableRow(session.StartTime);
                    timetable.Add(row);
                }

                TimetableCell cell = row.SessionCells.First(x => x.DayOfWeek == session.Date.DayOfWeek);
                cell.Sessions.Add(session);

                return timetable;
            })
            .OrderBy(x => x.StartTime)
            .ToList();

        return rs;
    }
}

