using Application.Schedules;
using Domain;

namespace Application.Common.Helpers
{
    public class SessionsGenerator
    {
        public static List<Session> Generate(ScheduleDTO schedule)
        {
            List<Session> sessions = new List<Session>();
            if (!schedule.TotalSessions.HasValue || schedule.DaysPerWeek.Count == 0)
            {
                return new List<Session>();
            }

            DateTime date = schedule.OpeningDate;
            int totalSessions = schedule.TotalSessions.Value;
            int[] recurDays = schedule.DaysPerWeek.Select(x => int.Parse(x.ToString())).ToArray();

            // To be validate when create / update schedule DTO
            int startIndex = Array.IndexOf(recurDays, (int)date.DayOfWeek);
            if (startIndex == -1)
            {
                return new List<Session>();
            }

            for (int i = startIndex, j = 1; i >= -1 && j > 0; i++, j++)
            {
                sessions.Add(new Session
                {
                    ScheduleId = schedule.Id,
                    Date = date,
                    Number = j
                });

                if (sessions.Count == totalSessions)
                {
                    break;
                }

                if (i == recurDays.Length - 1)
                {
                    date = date.AddDays(7 - (recurDays[i] - recurDays[0]));
                    i = -1;
                }
                else
                {
                    date = date.AddDays(recurDays[i + 1] - recurDays[i]);
                }
            }

            return sessions;
        }
    }
}