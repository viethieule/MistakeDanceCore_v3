using Application.Schedules;
using Application.Sessions;
using Domain;

namespace Application.Common.Helpers
{
    public static class SessionsGenerator
    {
        public static List<SessionDTO> Generate(ScheduleDTO schedule)
        {
            List<SessionDTO> sessions = new List<SessionDTO>();
            if (!schedule.TotalSessions.HasValue || schedule.DaysPerWeek.Count == 0)
            {
                return sessions;
            }

            DateTime date = schedule.OpeningDate;
            int totalSessions = schedule.TotalSessions.Value;
            List<DayOfWeek> daysPerWeek = schedule.DaysPerWeek;

            // To be validate when create / update schedule DTO
            int startIndex = Array.IndexOf(schedule.DaysPerWeek.ToArray(), date.DayOfWeek);
            if (startIndex == -1)
            {
                return new List<SessionDTO>();
            }

            for (int i = startIndex, j = 1; i >= -1 && j > 0; i++, j++)
            {
                sessions.Add(new SessionDTO
                {
                    ScheduleId = schedule.Id,
                    Date = date,
                    Number = j
                });

                if (sessions.Count == totalSessions)
                {
                    break;
                }

                if (i == daysPerWeek.Count - 1)
                {
                    date = date.AddDays(7 - ((int)daysPerWeek[i] - (int)daysPerWeek[0]));
                    i = -1;
                }
                else
                {
                    date = date.AddDays((int)daysPerWeek[i + 1] - (int)daysPerWeek[i]);
                }
            }

            return sessions;
        }
    }
}