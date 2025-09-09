using Web.Models.Account;

namespace Web.Services.Profile
{
    public interface IProfileService
    {
        Task<bool> ChangePasswordAsync(ChangePasswordViewModel model);
        Task<bool> UpdateProfilePictureAsync(int userId, string pictureUrl);
        Task<bool> UpdateProfileAsync(int userId, string phone);
        Task<string?> UploadProfilePictureAsync(IFormFile file);
    }
}
