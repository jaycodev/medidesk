using Shared.DTOs.Account.Requests;
using Shared.DTOs.Account.Responses;
using Web.Models.Account;

namespace Web.Mappers
{
    public static class AccountMapper
    {
        // Login ViewModel -> LoginRequest
        public static LoginRequest ToLoginRequest(this LoginViewModel model)
            => new()
            {
                Email = model.Email,
                Password = model.Password
            };

        // API response -> UserSession
        public static UserSession ToUserSession(this LoggedUserResponse response)
            => new()
            {
                UserId = response.UserId,
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                Roles = response.Roles ?? new List<string>(),
                Phone = response.Phone ?? string.Empty,
                ProfilePicture = response.ProfilePicture
            };
    }
}
