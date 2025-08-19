using System.ComponentModel.DataAnnotations;

namespace Web.Models.Specialties
{
    public class UpdateSpecialtyDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
