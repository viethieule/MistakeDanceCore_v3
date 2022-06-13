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
}
