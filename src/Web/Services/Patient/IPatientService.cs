using Shared.DTOs.Patients.Requests;
using Shared.DTOs.Patients.Responses;

namespace Web.Services.Patient
{
    public interface IPatientService
    {
        Task<List<PatientListResponse>> GetListAsync();
        Task<PatientResponse?> GetByIdAsync(int id);
        Task<(bool Success, string Message)> CreateAsync(CreatePatientRequest request);
        Task<(bool Success, string Message)> UpdateAsync(int id, UpdatePatientRequest request);

        Task<byte[]> GeneratePdfAsync();
        Task<byte[]> GenerateExcelAsync();
    }
}
