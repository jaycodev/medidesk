using Shared.DTOs.Users.Requests;
using Shared.DTOs.Users.Responses;

namespace Web.Services.User
{
    public interface IUserService
    {
        Task<UserResponse?> GetByIdAsync(int id);
        Task<List<UserListResponse>> GetListAsync(int? loggedUserId = null);
        Task<(bool Success, string Message)> CreateAsync(CreateUserRequest request);
        Task<(bool Success, string Message)> UpdateAsync(int id, UpdateUserRequest request);
        Task<(bool Success, string Message)> DeleteAsync(int id);

        Task<byte[]> GeneratePdfAsync();
        Task<byte[]> GenerateExcelAsync();
    }
}
