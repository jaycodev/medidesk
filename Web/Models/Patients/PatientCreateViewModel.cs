using System.ComponentModel.DataAnnotations;
using Web.Models.Users;

namespace Web.Models.Patients
{
    public class PatientCreateViewModel : UserCreateViewModel
    {
        [Required(ErrorMessage = "Ingrese una fecha de nacimiento")]
        [Display(Name = "Fecha nacimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly? BirthDate { get; set; }

        [Display(Name = "Grupo sanguíneo")]
        public string? BloodType { get; set; }
    }
}
