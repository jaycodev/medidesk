using Shared.DTOs.Specialties.Requests;
using Shared.DTOs.Specialties.Responses;

namespace Api.Repositories.Specialties
{
    public interface ISpecialtyRepository
    {
        List<SpecialtyResponse> GetList();
        SpecialtyResponse? GetById(int id);
        int Create(SpecialtyRequest request);
        int Update(int id, SpecialtyRequest request);
    }
}
