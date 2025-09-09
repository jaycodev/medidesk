using Shared.DTOs.Account.Requests;
using Shared.DTOs.Account.Responses;

namespace Web.Services.Account
{
    public interface IAccountService
    {
        Task<(bool Success, LoggedUserResponse? User, string Message)> LoginAsync(LoginRequest request);
    }
}
