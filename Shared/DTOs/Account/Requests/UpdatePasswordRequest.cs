using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Account.Requests
{
    public class UpdatePasswordRequest
    {
        [Required]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
    }
}
