using Api.Domains.Specialties.DTOs;
using Api.Domains.Specialties.Models;

namespace Api.Domains.Specialties.Repositories
{
    public interface ISpecialtyRepository
    {
        List<Specialty> GetList();
        Specialty? GetById(int id);
        int Create(CreateSpecialtyDto dto);
        int Update(int id, UpdateSpecialtyDto dto);
    }
}
