using FluentValidation;

namespace Application.Sessions;
public static class SessionValidators
{
    public static readonly ListSessionsRqValidator ListRq = new();
}

public class ListSessionsRqValidator : AbstractValidator<ListSessionsRq>
{
    public ListSessionsRqValidator()
    {
        RuleFor(x => x.Start).NotEqual(default(DateTime));
    }
}