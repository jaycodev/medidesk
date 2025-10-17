using Shared.DTOs.Schedules.Requests;
using Shared.DTOs.Schedules.Responses;

namespace Web.Services.Schedule
{
    public interface IScheduleService
    {
        Task<List<ScheduleResponse>> GetByIdAsync(int userId);
        Task<List<ScheduleByDateResponse>?> GetByDateAsync(int doctorId, DateTime date);
        Task<List<string>> UpdateAsync(List<ScheduleRequest> requests);
    }
}
