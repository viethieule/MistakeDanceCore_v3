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
        public const string MESSAGE_OPENING_DATE_NOT_MATCH_WITH_DAYS_PER_WEEK = "Opening date does not match with days per week";

        public ScheduleValidator()
        {
            RuleFor(x => x.Song).NotEmpty();
            RuleFor(x => x.StartTime).NotEqual(default(TimeSpan));
            RuleFor(x => x.OpeningDate).NotEqual(default(DateTime));
            RuleFor(x => x.DaysPerWeek).NotEmpty();

            When(x => x.DaysPerWeek != null, () =>
            {
                RuleFor(x => x.OpeningDate).Must((rq, x) => rq.DaysPerWeek.Contains(x.DayOfWeek))
                    .WithMessage(MESSAGE_OPENING_DATE_NOT_MATCH_WITH_DAYS_PER_WEEK);
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