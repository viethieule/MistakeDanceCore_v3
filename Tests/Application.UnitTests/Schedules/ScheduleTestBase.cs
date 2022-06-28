using Application.Schedules;
using Application.UnitTests.Common;

namespace Application.UnitTests.Schedules;

public class ScheduleTestBase : TestBase
{
    public static IEnumerable<object[]> RequiredFieldData =>
        new List<object[]>
        {
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.Song)), null! },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.Song)), string.Empty },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.Song)), " " },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.StartTime)), default(TimeSpan) },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.OpeningDate)), default(DateTime) },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.DaysPerWeek)), null!},
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.DaysPerWeek)), new List<DayOfWeek>() },
        };

    public static IEnumerable<object[]> RequiredNavigationData =>
        new List<object[]>
        {
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.ClassId)), (Func<ScheduleDTO, string>)(x => nameof(x.ClassName)) },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.TrainerId)), (Func<ScheduleDTO, string>)(x => nameof(x.TrainerName)) },
            new object[] { (Func<ScheduleDTO, string>)(x => nameof(x.BranchId)), (Func<ScheduleDTO, string>)(x => nameof(x.BranchName)) },
        };

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
            Song = TestConstants.SCHEDULE_TEST_SONG,
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
