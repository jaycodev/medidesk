using Shared.DTOs.Patients.Requests;
using Shared.DTOs.Patients.Responses;

namespace Api.Repositories.Patients
{
    public interface IPatientRepository
    {
        List<PatientListResponse> GetList();
        PatientResponse? GetById(int id);
        (int newId, string? error) Create(CreatePatientRequest request);
        int Update(int id, UpdatePatientRequest request);
    }
}
