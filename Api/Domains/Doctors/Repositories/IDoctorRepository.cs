using Api.Domains.Doctors.DTOs;

namespace Api.Domains.Doctors.Repositories
{
    public interface IDoctorRepository
    {
        List<DoctorListDto> GetList();
        DoctorDetailDto? GetById(int id);
        (int newId, string? error) Create(CreateDoctorDto dto);
        int Update(int id, UpdateDoctorDto dto);
    }
}
