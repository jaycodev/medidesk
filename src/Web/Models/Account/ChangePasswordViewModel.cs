﻿using System.ComponentModel.DataAnnotations;

namespace Web.Models.Account
{
    public class ChangePasswordViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Ingrese su contraseña actual")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la nueva contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un símbolo.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme la nueva contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
