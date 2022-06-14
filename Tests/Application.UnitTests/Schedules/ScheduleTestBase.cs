using Application.Schedules;
using Application.UnitTests.Common;

namespace Application.UnitTests.Schedules;

public class ScheduleTestBase : TestBase
{
    protected CreateScheduleService GetCreateScheduleService()
    {
        return new CreateScheduleService(
            _context, _userContextMock.Object, _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC,
            _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC, _dtcCollection.SessionDTC);
    }

    protected UpdateScheduleService GetUpdateScheduleService()
    {
        return new UpdateScheduleService(
            _context, _userContextMock.Object,
            _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC, _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC,
            _dtcCollection.SessionDTC, _dtcCollection.RegistrationDTC, _dtcCollection.PackageDTC, _dtcCollection.MembershipDTC);
    }

    protected ScheduleDTO PrepareScheduleDTO()
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
