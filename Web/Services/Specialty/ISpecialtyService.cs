using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.DTOs.Specialties.Requests;
using Shared.DTOs.Specialties.Responses;

namespace Web.Services.Specialty
{
    public interface ISpecialtyService
    {
        Task<List<SpecialtyResponse>> GetListAsync();
        Task<SelectList> GetSelectListAsync(int? selectedId = null);
        Task<SpecialtyResponse?> GetByIdAsync(int id);
        Task<bool> CreateAsync(SpecialtyRequest request);
        Task<bool> UpdateAsync(int id, SpecialtyRequest request);
    }
}
