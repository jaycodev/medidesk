using System.ComponentModel.DataAnnotations;

namespace Web.Models.Specialties
{
    public class CreateSpecialtyDTO
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Ingrese un nombre")]
        public string Name { get; set; }

        [Display(Name = "Descripción")]
        public string? Description { get; set; }
    }
}
