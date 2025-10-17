using Shared.DTOs.Schedules.Requests;
using Shared.DTOs.Schedules.Responses;

namespace Api.Repositories.Schedules
{
    public interface IScheduleRepository
    {
        List<ScheduleResponse> GetList(int idDoctor);
        List<ScheduleByDateResponse> GetByDate(int doctorId, DateTime date);
        List<string> CreateOrUpdate(List<ScheduleRequest> schedules);
    }
}
