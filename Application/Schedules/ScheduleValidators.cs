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
            RuleFor(x => x.Schedule).NotNull().SetValidator(new ScheduleValidator());
        }
    }

    public class UpdateScheduleRqValidator : AbstractValidator<UpdateScheduleRq>
    {
        public UpdateScheduleRqValidator()
        {
            RuleFor(x => x.Schedule).NotNull().SetValidator(new ScheduleValidator());
        }
    }

    public class ScheduleValidator : AbstractValidator<ScheduleDTO>
    {
        public ScheduleValidator()
        {
            RuleFor(x => x.Song).NotEmpty();
            RuleFor(x => x.StartTime).NotNull();
            RuleFor(x => x.OpeningDate).NotNull();
            
            When(x => x.DaysPerWeek.Count > 0, () =>
            {
                RuleFor(x => x.OpeningDate)
                    .Must((rq, x) => rq.DaysPerWeek.Contains(x.DayOfWeek));
                
                RuleFor(x => x.TotalSessions).NotNull().NotEqual(0);
            });

            When(x => x.TotalSessions.HasValue, () =>
            {
                RuleFor(x => x.DaysPerWeek).NotEmpty();
            });

            When(x => !x.BranchId.HasValue, () =>
            {
                RuleFor(x => x.BranchName).NotEmpty();
            });

            When(x => !x.TrainerId.HasValue, () =>
            {
                RuleFor(x => x.TrainerName).NotEmpty();
            });

            When(x => !x.ClassId.HasValue, () =>
            {
                RuleFor(x => x.ClassName).NotEmpty();
            });

            When(x => string.IsNullOrEmpty(x.BranchName), () =>
            {
                RuleFor(x => x.BranchId).NotEmpty();
            });

            When(x => string.IsNullOrEmpty(x.TrainerName), () =>
            {
                RuleFor(x => x.TrainerId).NotEmpty();
            });

            When(x => string.IsNullOrEmpty(x.ClassName), () =>
            {
                RuleFor(x => x.ClassId).NotEmpty();
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