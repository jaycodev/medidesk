using Shared.DTOs.Schedules.Requests;
using Shared.DTOs.Schedules.Responses;

namespace Web.Services.Schedule
{
    public class ScheduleService : IScheduleService
    {
        private readonly HttpClient _http;

        public ScheduleService(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        public async Task<List<ScheduleResponse>> GetByIdAsync(int userId)
        {
            try
            {
                var response = await _http.GetAsync($"api/schedules/{userId}");
                if (!response.IsSuccessStatusCode) return new List<ScheduleResponse>();

                var schedules = await response.Content.ReadFromJsonAsync<List<ScheduleResponse>>();
                return schedules ?? new List<ScheduleResponse>();
            }
            catch
            {
                return new List<ScheduleResponse>();
            }
        }

        public async Task<List<ScheduleByDateResponse>?> GetByDateAsync(int doctorId, DateTime date)
        {
            try
            {
                var resp = await _http.GetAsync(
                    $"api/schedules/by-date?doctorId={doctorId}&date={date:yyyy-MM-dd}");

                if (!resp.IsSuccessStatusCode) return null;

                return await resp.Content.ReadFromJsonAsync<List<ScheduleByDateResponse>>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<string>> UpdateAsync(List<ScheduleRequest> requests)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/schedules", requests);
                if (!response.IsSuccessStatusCode) return new List<string> { "Error al actualizar los horarios." };

                var messages = await response.Content.ReadFromJsonAsync<List<string>>();
                return messages ?? new List<string>();
            }
            catch
            {
                return new List<string> { "Error al actualizar los horarios." };
            }
        }
    }
}
