using System.ComponentModel.DataAnnotations;
using Web.Models.Users;

namespace Web.Models.Doctors
{
    public class DoctorCreateViewModel : UserCreateViewModel
    {
        [Display(Name = "Especialidad")]
        [Required(ErrorMessage = "Seleccione una especialidad")]
        public int SpecialtyId { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "Seleccione un estado")]
        public bool? Status { get; set; }
    }
}
