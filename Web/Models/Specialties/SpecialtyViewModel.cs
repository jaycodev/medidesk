using System.ComponentModel.DataAnnotations;

namespace Web.Models.Specialties
{
    public class SpecialtyViewModel
    {
        [Display(Name = "Código")]
        public int SpecialtyId { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Description { get; set; }
    }
}
