using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.DTOs.Specialties.Requests;
using Shared.DTOs.Specialties.Responses;

namespace Web.Services.Specialty
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly HttpClient _http;

        public SpecialtyService(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        public async Task<List<SpecialtyResponse>> GetListAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<SpecialtyResponse>>("api/specialties") ?? new List<SpecialtyResponse>();
            }
            catch
            {
                return new List<SpecialtyResponse>();
            }
        }

        public async Task<SelectList> GetSelectListAsync(int? selectedId = null)
        {
            try
            {
                var specialties = await GetListAsync();
                return new SelectList(specialties, "SpecialtyId", "Name", selectedId);
            }
            catch
            {
                return new SelectList(new List<SpecialtyResponse>(), "SpecialtyId", "Name", selectedId);
            }
        }

        public async Task<SpecialtyResponse?> GetByIdAsync(int id)
        {
            try
            {
                var resp = await _http.GetAsync($"api/specialties/{id}");
                if (!resp.IsSuccessStatusCode) return null;
                return await resp.Content.ReadFromJsonAsync<SpecialtyResponse>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateAsync(SpecialtyRequest request)
        {
            try
            {
                var resp = await _http.PostAsJsonAsync("api/specialties", request);
                return resp.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(int id, SpecialtyRequest request)
        {
            try
            {
                var resp = await _http.PutAsJsonAsync($"api/specialties/{id}", request);
                return resp.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
