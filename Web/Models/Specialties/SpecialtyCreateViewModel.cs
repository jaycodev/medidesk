using System.ComponentModel.DataAnnotations;

namespace Web.Models.Specialties
{
    public class SpecialtyCreateViewModel
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Ingrese un nombre")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Description { get; set; }
    }
}
