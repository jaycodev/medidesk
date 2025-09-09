using Shared.DTOs.Account.Requests;
using Shared.DTOs.Account.Responses;

namespace Api.Repositories.Account
{
    public interface IAccountRepository
    {
        LoggedUserResponse? Login(LoginRequest request);
        public int UpdateProfile(int id, UpdateProfileRequest request);
        int UpdateProfilePicture(int id, UpdateProfilePictureRequest request);
        int UpdatePassword(int id, UpdatePasswordRequest request);
    }
}
