using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Specialties.Requests
{
    public class SpecialtyRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
