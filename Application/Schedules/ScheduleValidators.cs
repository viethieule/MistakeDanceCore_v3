using FluentValidation;

namespace Application.Schedules
{
    public static class ScheduleValidators
    {
        public static readonly CreateScheduleRqValidator CreateRq = new();
        public static readonly UpdateScheduleRqValidator UpdateRq = new();
        public static readonly DeleteScheduleRqValidator DeleteRq = new();
    }

    public class CreateScheduleRqValidator : AbstractValidator<CreateScheduleRq>
    {
        public CreateScheduleRqValidator()
        {
            RuleFor(x => x.ScheduleFormDTO).NotNull().SetValidator(new ScheduleFormValidator());
        }
    }

    public class UpdateScheduleRqValidator : AbstractValidator<UpdateScheduleRq>
    {
        public UpdateScheduleRqValidator()
        {
            RuleFor(x => x.ScheduleFormDTO).NotNull().SetValidator(new ScheduleFormValidator());
        }
    }

    public class ScheduleFormValidator : AbstractValidator<ScheduleFormDTO>
    {
        public ScheduleFormValidator()
        {
            RuleFor(x => x.Schedule).NotNull();

            RuleFor(x => x.Schedule.Song).NotEmpty();
            RuleFor(x => x.Schedule.StartTime).NotNull();
            RuleFor(x => x.Schedule.OpeningDate).NotNull();
            
            When(x => x.Schedule.DaysPerWeek.Count > 0, () =>
            {
                RuleFor(x => x.Schedule.OpeningDate)
                    .Must((rq, x) => rq.Schedule.DaysPerWeek.Contains(x.DayOfWeek));
                
                RuleFor(x => x.Schedule.TotalSessions).NotNull().NotEqual(0);
            });

            When(x => x.Schedule.TotalSessions.HasValue, () =>
            {
                RuleFor(x => x.Schedule.DaysPerWeek).NotEmpty();
            });

            When(x => !x.Schedule.BranchId.HasValue, () =>
            {
                RuleFor(x => x.Schedule.BranchName).NotEmpty();
            });

            When(x => !x.Schedule.TrainerId.HasValue, () =>
            {
                RuleFor(x => x.Schedule.TrainerName).NotEmpty();
            });

            When(x => !x.Schedule.ClassId.HasValue, () =>
            {
                RuleFor(x => x.Schedule.ClassName).NotEmpty();
            });

            When(x => string.IsNullOrEmpty(x.Schedule.BranchName), () =>
            {
                RuleFor(x => x.Schedule.BranchId).NotEmpty();
            });

            When(x => string.IsNullOrEmpty(x.Schedule.TrainerName), () =>
            {
                RuleFor(x => x.Schedule.TrainerId).NotEmpty();
            });

            When(x => string.IsNullOrEmpty(x.Schedule.ClassName), () =>
            {
                RuleFor(x => x.Schedule.ClassId).NotEmpty();
            });
        }
    }

    public class DeleteScheduleRqValidator : AbstractValidator<DeleteScheduleRq>
    {
        public DeleteScheduleRqValidator()
        {
            RuleFor(x => x.ScheduleId).NotEmpty();
        }
    }
}