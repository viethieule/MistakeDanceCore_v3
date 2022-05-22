using Application.Branches;
using Application.Classes;
using Application.Common.Interfaces;
using Application.DefaultPackages;
using Application.Members;
using Application.Memberships;
using Application.Packages;
using Application.Registrations;
using Application.Schedules;
using Application.Sessions;
using Application.Trainers;
using Persistence;

namespace Application.UnitTests.Common;

public class DTCCollection
{
    public DTCCollection(MistakeDanceDbContext context, IUserContext userContext)
    {
        BranchDTC = new(context, userContext);
        ClassDTC = new(context, userContext);
        DefaultPackageDTC = new(context, userContext);
        MemberDTC = new(context, userContext);
        MembershipDTC = new(context, userContext);
        PackageDTC = new(context, userContext, DefaultPackageDTC);
        RegistrationDTC = new(context, userContext);
        ScheduleDTC = new(context, userContext);
        SessionDTC = new(context, userContext);
        TrainerDTC = new(context, userContext);
    }

    public readonly BranchDTC BranchDTC;
    public readonly ClassDTC ClassDTC;
    public readonly DefaultPackageDTC DefaultPackageDTC;
    public readonly MemberDTC MemberDTC;
    public readonly MembershipDTC MembershipDTC;
    public readonly PackageDTC PackageDTC;
    public readonly RegistrationDTC RegistrationDTC;
    public readonly ScheduleDTC ScheduleDTC;
    public readonly SessionDTC SessionDTC;
    public readonly TrainerDTC TrainerDTC;
}