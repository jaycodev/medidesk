using Shared.DTOs.Doctors.Requests;
using Shared.DTOs.Doctors.Responses;

namespace Api.Repositories.Doctors
{
    public interface IDoctorRepository
    {
        List<DoctorListResponse> GetList();
        DoctorResponse? GetById(int id);
        List<DoctorBySpecialtyResponse> GetBySpecialty(int specialtyId, int userId);
        (int newId, string? error) Create(CreateDoctorRequest request);
        int Update(int id, UpdateDoctorRequest request);
    }
}
