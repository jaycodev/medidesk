using System.ComponentModel.DataAnnotations;

namespace Web.Models.User
{
    public class UserEditViewModel
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        [Display(Name = "Rol(es)")]
        [Required(ErrorMessage = "Seleccione una combinación de rol(es)")]
        public string SelectedRoleCombo { get; set; }


    }
}
