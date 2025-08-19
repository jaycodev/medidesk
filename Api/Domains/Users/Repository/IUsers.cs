using Api.Domains.Specialties.DTOs;
using Api.Domains.Users.DTOs;
using Api.Domains.Users.Models;

namespace Api.Domains.Users.Repository
{
    public interface IUsers
    {

        List<User> GetList();
        User GetById(int id);
        int Create(UserDTO dto);

        int Update(int id, UserDTO dto);

        int Delete(int id);
    }
}
