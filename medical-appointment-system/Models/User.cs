using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace medical_appointment_system.Models
{
    public class User
    {
        [Display(Name = "Código")]
        public int UserId { get; set; }

        [Display(Name = "Nombre(s)")]
        [Required(ErrorMessage = "Ingrese un(os) nombre(s)")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido(s)")]
        [Required(ErrorMessage = "Ingrese un(os) apellido(s)")]
        public string LastName { get; set; }

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "Ingrese su correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string Email { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Ingrese una contraseña")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un símbolo.")]
        public string Password { get; set; }

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "Ingrese el número de teléfono")]
        [RegularExpression(@"^\+?\d{9,15}$", ErrorMessage = "El teléfono debe contener entre 9 y 15 dígitos, y puede comenzar con '+' si es internacional.")]
        public string Phone { get; set; }

        [Display(Name = "Rol(es)")]
        [Required(ErrorMessage = "Seleccione una combinación de rol(es)")]
        public string SelectedRoleCombo { get; set; }

        [Display(Name = "Rol(es)")]
        public List<string> Roles { get; set; } = new List<string>();

        [Display(Name = "Foto perfil")]
        public string ProfilePicture { get; set; }

        public string ActiveRole { get; set; }
        public int? SpecialtyId { get; set; }
        
        public bool? Status { get; set; }
        
        public DateTime? BirthDate { get; set; }

        public string BloodType { get; set; }

        public bool CanDelete { get; set; }
    }
}