using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Specialties.Responses
{
    public class SpecialtyResponse
    {
        public int SpecialtyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
