using Web.Models.Account;
using Web.Models.Profile;

namespace Web.Mappers
{
    public static class ProfileMapper
    {
        public static ProfileViewModel ToViewModel(UserSession user)
        {
            return new ProfileViewModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                ProfilePicture = user.ProfilePicture,
                ChangePassword = new ChangePasswordViewModel { UserId = user.UserId }
            };
        }
    }
}
