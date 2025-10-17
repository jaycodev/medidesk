using Shared.DTOs.Doctors.Requests;
using Shared.DTOs.Doctors.Responses;

namespace Web.Services.Doctor
{
    public interface IDoctorService
    {
        Task<List<DoctorListResponse>> GetListAsync();
        Task<DoctorResponse?> GetByIdAsync(int id);
        Task<List<DoctorBySpecialtyResponse>> GetBySpecialtyAsync(int specialtyId, int userId);
        Task<(bool Success, string Message)> CreateAsync(CreateDoctorRequest request);
        Task<(bool Success, string Message)> UpdateAsync(int id, UpdateDoctorRequest request);

        Task<byte[]> GeneratePdfAsync();
        Task<byte[]> GenerateExcelAsync();
    }
}
