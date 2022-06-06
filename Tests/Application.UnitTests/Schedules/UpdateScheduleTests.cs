using Application.Schedules;
using Application.UnitTests.Common;
using Xunit;

namespace Application.UnitTests.Schedules;

public class UpdateScheduleTests : TestBase
{
    //
    // Test cases
    // - Update correct basic data
    // - Update correct basic data + navigation data (trainer / class /branch)
    // - Update with correct (TotalSessions + DaysPerWeek + OpeningDate), expect:
    // --- Corrected new sessions created
    // --- Previous sessions removed
    // ------ Remove registration
    // ------ Increase membership session back to member
    // - Messages:
    // --- Start Time changed
    // --- Selected sessions deleted
    // --- Inform member of session deleted
    // - Validators
    // - Cannot update opening date of started schedule
    public async Task Handle_GivenCorrectInput_CreatesSchedule_WithCorrectValues()
    {
        UpdateScheduleRq rq = new UpdateScheduleRq()
        {
        };

        UpdateScheduleService updateScheduleService = new UpdateScheduleService(
            _context, _userContextMock.Object, 
            _dtcCollection.BranchDTC, _dtcCollection.TrainerDTC, _dtcCollection.ClassDTC, _dtcCollection.ScheduleDTC,
            _dtcCollection.SessionDTC, _dtcCollection.RegistrationDTC, _dtcCollection.PackageDTC, _dtcCollection.MembershipDTC
        );

        UpdateScheduleRs rs = await updateScheduleService.RunAsync(rq);
        ScheduleDTO schedule = rq.Schedule;

        Assert.NotEqual(0, schedule.Id);
        Assert.Equal(rq.Schedule.Song, schedule.Song);
        Assert.Equal(rq.Schedule.StartTime.Hours, schedule.StartTime.Hours);
        Assert.Equal(rq.Schedule.StartTime.Minutes, schedule.StartTime.Minutes);
        Assert.Equal(rq.Schedule.StartTime.Seconds, schedule.StartTime.Seconds);
        Assert.Equal(rq.Schedule.BranchName, schedule.BranchName);
        Assert.Equal(rq.Schedule.ClassName, schedule.ClassName);
        Assert.Equal(rq.Schedule.TrainerName, schedule.TrainerName);
        Assert.Equal(rq.Schedule.TotalSessions, schedule.TotalSessions);
        Assert.Equal(rq.Schedule.DaysPerWeek.Count, schedule.DaysPerWeek.Count);
        Assert.Equal(rq.Schedule.OpeningDate, schedule.OpeningDate);
        Assert.Equal(TestConstants.TEST_USER_NAME, schedule.CreatedBy);
        Assert.Equal(TestConstants.TEST_USER_NAME, schedule.UpdatedBy);
        Assert.Equal(DateTime.Now.Date, schedule.CreatedDate.Date);
        Assert.Equal(DateTime.Now.Date, schedule.UpdatedDate.Date);
    }
}