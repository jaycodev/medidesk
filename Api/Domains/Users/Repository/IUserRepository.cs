using Api.Domains.Specialties.DTOs;
using Api.Domains.Users.DTOs;
using Api.Domains.Users.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Api.Domains.Users.Repository
{
    public interface IUserRepository
    {
        List<User> GetList(int id);
        User GetById(int id);
        int Create(UserDTO dto);
        int Update(int id, UserUpdateDTO dto);
        int Delete(int id);
        LoggedUserDTO? Login(string email, string password);
        int UpdatePassword(int userId, string newPassword,string currentPassword);
        int UpdateProfilePicture(int userId, string profilePictureUrl);
        public int UpdateProfile(int userId, string phone);
    }
}
