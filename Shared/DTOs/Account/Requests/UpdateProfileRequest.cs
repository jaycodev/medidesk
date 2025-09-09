using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Account.Requests
{
    public class UpdateProfileRequest
    {
        [RegularExpression(@"^$|^\d{9,}$", ErrorMessage = "Phone must have at least 9 numeric digits")]
        public string? Phone { get; set; }
    }
}
