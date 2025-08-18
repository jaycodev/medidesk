using Api.Domains.Specialties.DTOs;
using Api.Domains.Specialties.Models;

namespace Api.Domains.Specialties.Repositories
{
    public interface ISpecialtyRepository
    {
        List<Specialty> GetList();
        Specialty? GetById(int id);
        int Create(CreateSpecialtyDTO dto);
        int Update(int id, UpdateSpecialtyDTO dto);
    }
}
