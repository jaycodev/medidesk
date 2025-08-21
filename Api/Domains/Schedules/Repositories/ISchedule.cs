using Api.Domains.Schedules.DTOs;
using Api.Domains.Schedules.Models;

namespace Api.Domains.Schedules.Repositories
{
    public interface ISchedule
    {
        public List<string> CreateOrUpdateSchedules(List<ScheduleDTO> schedules);
        List<ScheduleDTO> GetListSchedulesByIdDoctor(int idDoctor);
    }
}
