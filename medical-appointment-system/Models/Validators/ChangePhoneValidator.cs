using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace medical_appointment_system.Models.Validators
{
    public class ChangePhoneValidator
    {
        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Ingrese el número de teléfono")]
        [Display(Name = "Teléfono")]
        [RegularExpression(@"^\+?\d{9,15}$", ErrorMessage = "El teléfono debe contener entre 9 y 15 dígitos, y puede comenzar con '+' si es internacional.")]
        public string Phone { get; set; }
    }
}