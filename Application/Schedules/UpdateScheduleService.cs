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
using FluentValidation.Results;

namespace Application.Schedules
{
    public class UpdateScheduleRq : BaseRequest
    {
        public ScheduleDTO Schedule { get; set; }
        public int SelectedSessionId { get; set; }
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

        private const string MESSAGE_START_TIME_CHANGED = "Thời gian lớp học đã thay đổi. Vui lòng thông báo lại cho các hội viên đã đăng ký";
        private const string MESSAGE_SELECTED_SESSION_DELETED = "Buổi học này đã bị xóa sau khi sửa lịch học";
        private const string MESSAGE_INFORM_OF_DELETED_REGISTRATION = "Có một hoặc nhiều buổi học đã bị xóa sau cập nhật. Các hội viên đã đăng ký sẽ được hoàn lại đăng ký của mình";

        public UpdateScheduleService(
            IMistakeDanceDbContext mistakeDanceDbContext,
            IUserContext userContext,
            BranchDTC branchDTC,
            TrainerDTC trainerDTC,
            ClassDTC classDTC,
            ScheduleDTC scheduleDTC,
            SessionDTC sessionDTC,
            RegistrationDTC registrationDTC,
            PackageDTC packageDTC,
            MembershipDTC membershipDTC) : base(mistakeDanceDbContext, userContext)
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

        protected override ValidationResult Validate(UpdateScheduleRq rq)
        {
            return ScheduleValidators.UpdateRq.Validate(rq);
        }

        protected override async Task<UpdateScheduleRs> RunTransactionalAsync(UpdateScheduleRq rq)
        {
            UpdateScheduleRs rs = new UpdateScheduleRs();
            ScheduleDTO scheduleDto = rq.Schedule;

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

            ScheduleDTO currentDto = await _scheduleDTC.SingleByIdAsync(scheduleDto.Id);

            if (scheduleDto.StartTime != currentDto.StartTime)
            {
                rs.Messages.Add(MESSAGE_START_TIME_CHANGED);
            }

            await _scheduleDTC.UpdateAsync(scheduleDto);

            bool isUpdateSessions =
                scheduleDto.OpeningDate.Date != currentDto.OpeningDate.Date ||
                !(scheduleDto.DaysPerWeek.All(currentDto.DaysPerWeek.Contains) && scheduleDto.DaysPerWeek.Count == currentDto.DaysPerWeek.Count) ||
                scheduleDto.TotalSessions != currentDto.TotalSessions;

            if (isUpdateSessions)
            {
                List<SessionDTO> currentSessions = await _sessionDTC.ListShallowByScheduleIdAsync(scheduleDto.Id);
                List<SessionDTO> updateSessions = SessionsGenerator.Generate(scheduleDto);

                List<SessionDTO> toBeAddedSessions = updateSessions.Where(x => !currentSessions.Any(y => y.Date.Date == x.Date.Date)).ToList();
                List<SessionDTO> toBeRemovedSessions = currentSessions.Where(x => !updateSessions.Any(y => y.Date.Date == x.Date.Date)).ToList();

                if (toBeRemovedSessions.Any(x => x.Id == rq.SelectedSessionId))
                {
                    rs.Messages.Add(MESSAGE_SELECTED_SESSION_DELETED);
                }

                List<RegistrationDTO> toBeRemovedRegistrations = await _registrationDTC.GetBySessionIdsAsync(toBeRemovedSessions.Select(x => x.Id).ToList());
                if (toBeRemovedRegistrations.Count > 0)
                {
                    Dictionary<int, int> memberIdAndRemainingSessionDiffs = toBeRemovedRegistrations.GroupBy(x => x.MemberId).ToDictionary(x => x.Key, x => x.Count());

                    await _membershipDTC.UpdateRemainingSessionsByMemberIds(memberIdAndRemainingSessionDiffs);

                    rs.Messages.Add(MESSAGE_INFORM_OF_DELETED_REGISTRATION);
                }

                await _sessionDTC.CreateRangeAsync(toBeAddedSessions);

                await _registrationDTC.DeleteRangeAsync(toBeRemovedRegistrations);
                await _sessionDTC.DeleteRangeAsync(toBeRemovedSessions);
                
                await _sessionDTC.RebuildScheduleSessionsNumberAsync(currentSessions.Where(x => toBeAddedSessions.Select(y => y.Id).Contains(x.Id)).Concat(toBeAddedSessions).ToList());
            }

            return rs;
        }
    }
}