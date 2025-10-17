using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Account.Requests
{
    public class UpdateProfilePictureRequest
    {
        [Required]
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}
