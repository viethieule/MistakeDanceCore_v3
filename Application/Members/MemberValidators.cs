using Application.Packages;
using FluentValidation;

namespace Application.Members;
public class MemberValidators
{
    public static readonly CreateMemberRqValidator CreateRq = new();
}

public class CreateMemberRqValidator : AbstractValidator<CreateMemberRq>
{
    public CreateMemberRqValidator()
    {
        RuleFor(x => x.Member).NotNull()
            .SetValidator(new MemberValidator());
        RuleFor(x => x.Package).NotNull()
            .SetValidator(new PackageValidator());
    }

    private class MemberValidator : AbstractValidator<MemberDTO>
    {
        public MemberValidator()
        {
            RuleFor(x => x.FullName).NotEmpty();
            RuleFor(x => x.BranchId).NotEqual(default(int));
            RuleFor(x => x.PhoneNumber).NotEmpty().Must(x => x.Trim().All(c => char.IsDigit(c)));
        }
    }

    private class PackageValidator : AbstractValidator<PackageDTO>
    {
        public PackageValidator()
        {
            When(x => !x.DefaultPackageId.HasValue, () =>
            {
                RuleFor(x => x.Months).NotEqual(default(int));
                RuleFor(x => x.Price).NotEqual(default(double));
                RuleFor(x => x.NumberOfSessions).NotEqual(default(int));
            });
        }
    }
}
