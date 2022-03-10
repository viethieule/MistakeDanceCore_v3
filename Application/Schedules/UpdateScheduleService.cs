using Application.Branches;
using Application.Classes;
using Application.Common;
using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Memberships;
using Application.Packages;
using Application.Registrations;
using Application.Sessions;
using Application.Trainers;

namespace Application.Schedules
{
    public class UpdateScheduleRq : BaseRequest
    {
        public ScheduleFormDTO ScheduleFormDTO { get; set; }
    }

    public class UpdateScheduleRs : BaseResponse
    {
    }

    public class UpdateScheduleService : TransactionalService<UpdateScheduleRq, UpdateScheduleRs>
    {
        private readonly BranchDTC _branchDTC;
        private readonly TrainerDTC _trainerDTC;
        private readonly ClassDTC _classDTC;
        private readonly ScheduleDTC _scheduleDTC;
        private readonly SessionDTC _sessionDTC;
        private readonly RegistrationDTC _registrationDTC;
        private readonly PackageDTC _packageDTC;
        private readonly MembershipDTC _membershipDTC;

        public UpdateScheduleService(
            IMistakeDanceDbContext mistakeDanceDbContext,
            BranchDTC branchDTC,
            TrainerDTC trainerDTC,
            ClassDTC classDTC,
            ScheduleDTC scheduleDTC,
            SessionDTC sessionDTC,
            RegistrationDTC registrationDTC,
            PackageDTC packageDTC,
            MembershipDTC membershipDTC) : base(mistakeDanceDbContext)
        {
            _branchDTC = branchDTC;
            _trainerDTC = trainerDTC;
            _classDTC = classDTC;
            _scheduleDTC = scheduleDTC;
            _sessionDTC = sessionDTC;
            _registrationDTC = registrationDTC;
            _packageDTC = packageDTC;
            _membershipDTC = membershipDTC;
        }

        protected override async Task<UpdateScheduleRs> RunTransactionalAsync(UpdateScheduleRq rq)
        {
            ScheduleDTO scheduleDto = rq.ScheduleFormDTO.Schedule;

            if (!string.IsNullOrWhiteSpace(scheduleDto.BranchName))
            {
                BranchDTO branchDTO = new() { Name = scheduleDto.BranchName };
                await _branchDTC.CreateAsync(branchDTO);
                scheduleDto.BranchId = branchDTO.Id;
            }

            if (!string.IsNullOrWhiteSpace(scheduleDto.ClassName))
            {
                ClassDTO classDTO = new() { Name = scheduleDto.ClassName };
                await _classDTC.CreateAsync(classDTO);
                scheduleDto.ClassId = classDTO.Id;
            }

            if (!string.IsNullOrWhiteSpace(scheduleDto.TrainerName))
            {
                TrainerDTO trainerDTO = new() { Name = scheduleDto.TrainerName };
                await _trainerDTC.CreateAsync(trainerDTO);
                scheduleDto.TrainerId = trainerDTO.Id;
            }

            ScheduleDTO currentDto = await _scheduleDTC.GetByIdAsync(scheduleDto.Id, true);

            await _scheduleDTC.UpdateAsync(scheduleDto);

            bool isUpdateSessions =
                scheduleDto.OpeningDate.Date != currentDto.OpeningDate.Date ||
                !(scheduleDto.DaysPerWeek.All(currentDto.DaysPerWeek.Contains) && scheduleDto.DaysPerWeek.Count == currentDto.DaysPerWeek.Count) ||
                scheduleDto.TotalSessions != currentDto.TotalSessions;

            if (isUpdateSessions)
            {
                List<SessionDTO> currentSessions = await _sessionDTC.GetByScheduleIdAsync(scheduleDto.Id);
                List<SessionDTO> updateSessions = SessionsGenerator.Generate(scheduleDto);

                List<SessionDTO> toBeAddedSessions = updateSessions.Where(x => !currentSessions.Any(y => y.Date.Date == x.Date.Date)).ToList();
                List<SessionDTO> toBeRemovedSessions = currentSessions.Where(x => !updateSessions.Any(y => y.Date.Date == x.Date.Date)).ToList();

                // TODO: inform user a message of change
                // Domain event / message ??

                await _sessionDTC.CreateRangeAsync(toBeAddedSessions);
                await _sessionDTC.DeleteRangeAsync(toBeRemovedSessions);
                await _sessionDTC.RebuildScheduleSessionsNumberAsync(currentSessions.Where(x => toBeAddedSessions.Select(y => y.Id).Contains(x.Id)).Concat(toBeAddedSessions).ToList());

                // TODO:
                // Check if registrations are cascaded
                List<RegistrationDTO> registrations = await _registrationDTC.GetBySessionIdsAsync(toBeRemovedSessions.Select(x => x.Id).ToList());
                if (registrations.Count > 0)
                {
                    Dictionary<int, int> memberIdAndRemainingSessionDiffs = registrations.GroupBy(x => x.MemberId).ToDictionary(x => x.Key, x => x.Count());

                    await _packageDTC.UpdateRemainingSessionsByMemberIds(memberIdAndRemainingSessionDiffs);
                    await _membershipDTC.UpdateRemainingSessionsByMemberIds(memberIdAndRemainingSessionDiffs);
                    await _registrationDTC.DeleteRangeAsync(registrations);
                }
            }

            return new UpdateScheduleRs();
        }
    }
}