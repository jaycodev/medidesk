using Api.Domains.Doctors.DTOs;

namespace Api.Domains.Doctors.Repositories
{
    public interface IDoctorRepository
    {
        List<DoctorListDTO> GetList();
        DoctorDetailDTO? GetById(int id);
        (int newId, string? error) Create(CreateDoctorDTO dto);
        int Update(int id, UpdateDoctorDTO dto);
    }
}
