using Application.Branches;
using Application.Classes;
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
    public DTCCollection(MistakeDanceDbContext context)
    {
        BranchDTC = new(context);
        ClassDTC = new(context);
        DefaultPackageDTC = new(context);
        MemberDTC = new(context);
        MembershipDTC = new(context);
        PackageDTC = new(context, DefaultPackageDTC);
        RegistrationDTC = new(context);
        ScheduleDTC = new(context);
        SessionDTC = new(context);
        TrainerDTC = new(context);
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