using Application.Common.AutoMapper;
using AutoMapper;
using Domain;

namespace Application.Sessions
{
    public class SessionDTO : IMapFrom<Session>
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public int Number { get; set; }
        public int ScheduleId { get; set; }
        public string Song { get; set; }
        public DateTime OpeningDate { get; set; }
        public List<DayOfWeek> DaysPerWeek { get; set; }
        public int TotalSessions { get; set; }
        public TimeSpan StartTime { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }
        public int TotalRegistered { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Session, SessionDTO>()
                .ForMember(d => d.Song, opt => opt.MapFrom(s => s.Schedule.Song))
                .ForMember(d => d.OpeningDate, opt => opt.MapFrom(s => s.Schedule.OpeningDate))
                .ForMember(d => d.DaysPerWeek, opt => opt.MapFrom(s => s.Schedule.DaysPerWeek))
                .ForMember(d => d.TotalSessions, opt => opt.MapFrom(s => s.Schedule.TotalSessions))
                .ForMember(d => d.StartTime, opt => opt.MapFrom(s => s.Schedule.StartTime))
                .ForMember(d => d.BranchId, opt => opt.MapFrom(s => s.Schedule.BranchId))
                .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Schedule.Branch.Name))
                .ForMember(d => d.TrainerName, opt => opt.MapFrom(s => s.Schedule.Trainer.Name))
                .ForMember(d => d.ClassName, opt => opt.MapFrom(s => s.Schedule.Class.Name))
                .ForMember(d => d.TotalRegistered, opt => opt.MapFrom(s => s.Registrations.Count));
        }
    }
}