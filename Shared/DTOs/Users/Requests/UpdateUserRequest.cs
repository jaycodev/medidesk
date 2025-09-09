using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Users.Requests
{
    public class UpdateUserRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Email is not a valid email address")]
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^$|^\d{9,}$", ErrorMessage = "Phone must have at least 9 numeric digits")]
        public string? Phone { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }
}
