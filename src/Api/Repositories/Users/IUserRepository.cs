using Shared.DTOs.Users.Requests;
using Shared.DTOs.Users.Responses;

namespace Api.Repositories.Users
{
    public interface IUserRepository
    {
        List<UserListResponse> GetList(int id);
        UserResponse? GetById(int id);
        (int newId, string? error) Create(CreateUserRequest request);
        (int affectedRows, string? error) Update(int id, UpdateUserRequest request);
        int Delete(int id);
    }
}
