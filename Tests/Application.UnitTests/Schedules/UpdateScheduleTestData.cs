namespace Application.UnitTests.Schedules;
public static class UpdateScheduleTestData
{
    public static object[] CreateSessionInputTestData1()
    {
        DateTime openingDate = DateTime.Now.AddDays(1);
        SessionInputData initData = new SessionInputData
        {
            OpeningDate = openingDate,
            DaysPerWeek = new List<DayOfWeek> { openingDate.DayOfWeek, openingDate.AddDays(2).DayOfWeek, openingDate.AddDays(4).DayOfWeek },
            TotalSessions = 6
        };

        DateTime newOpeningDate = openingDate.AddDays(1);
        SessionInputData updatedData = new SessionInputData
        {
            OpeningDate = newOpeningDate,
            DaysPerWeek = new List<DayOfWeek> { newOpeningDate.DayOfWeek, newOpeningDate.AddDays(2).DayOfWeek },
            TotalSessions = 5
        };

        List<DateTime> expectedSessionDates = new List<DateTime>
        {
            newOpeningDate,
            newOpeningDate.AddDays(2),
            newOpeningDate.AddDays(7),
            newOpeningDate.AddDays(2 + 7),
            newOpeningDate.AddDays(7 + 7),
        };

        bool isExpectRegistrationDeleted = true;
        return new object[] { initData, updatedData, expectedSessionDates, isExpectRegistrationDeleted };
    }

    public static object[] CreateSessionInputTestData2()
    {
        DateTime openingDate = DateTime.Now.AddDays(1);
        SessionInputData initData = new SessionInputData
        {
            OpeningDate = openingDate,
            DaysPerWeek = new List<DayOfWeek> { openingDate.DayOfWeek, openingDate.AddDays(2).DayOfWeek, openingDate.AddDays(4).DayOfWeek },
            TotalSessions = 6
        };

        SessionInputData updatedData = new SessionInputData
        {
            OpeningDate = openingDate,
            DaysPerWeek = new List<DayOfWeek> { openingDate.DayOfWeek, openingDate.AddDays(3).DayOfWeek },
            TotalSessions = 4
        };

        List<DateTime> expectedSessionDates = new List<DateTime>
        {
            openingDate,
            openingDate.AddDays(3),
            openingDate.AddDays(7),
            openingDate.AddDays(3 + 7)
        };

        bool isExpectRegistrationDeleted = false;
        return new object[] { initData, updatedData, expectedSessionDates, isExpectRegistrationDeleted };
    }

    public static object[] CreateSessionInputTestData3()
    {
        DateTime openingDate = DateTime.Now.AddDays(1);
        SessionInputData initData = new SessionInputData
        {
            OpeningDate = openingDate,
            DaysPerWeek = new List<DayOfWeek> { openingDate.DayOfWeek, openingDate.AddDays(2).DayOfWeek, openingDate.AddDays(4).DayOfWeek },
            TotalSessions = 3
        };

        SessionInputData updatedData = new SessionInputData
        {
            OpeningDate = initData.OpeningDate,
            DaysPerWeek = initData.DaysPerWeek,
            TotalSessions = 4
        };

        List<DateTime> expectedSessionDates = new List<DateTime>
        {
            openingDate,
            openingDate.AddDays(2),
            openingDate.AddDays(4),
            openingDate.AddDays(7)
        };

        bool isExpectRegistrationDeleted = false;
        return new object[] { initData, updatedData, expectedSessionDates, isExpectRegistrationDeleted };
    }
}

public class SessionInputData
{
    public DateTime OpeningDate { get; set; }
    public List<DayOfWeek> DaysPerWeek { get; set; } = new List<DayOfWeek>();
    public int TotalSessions { get; set; }
}
