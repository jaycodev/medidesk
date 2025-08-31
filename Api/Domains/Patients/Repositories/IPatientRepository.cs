using Api.Domains.Patients.DTOs;

namespace Api.Domains.Patients.Repositories
{
    public interface IPatientRepository
    {
        List<PatientListDTO> GetList();
        PatientDetailDTO? GetById(int id);
        (int newId, string? error) Create(CreatePatientDTO dto);
        int Update(int id, UpdatePatientDTO dto);
    }
}
