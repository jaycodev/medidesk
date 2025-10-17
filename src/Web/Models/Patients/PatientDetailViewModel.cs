﻿using System.ComponentModel.DataAnnotations;

namespace Web.Models.Patients
{
    public class PatientDetailViewModel
    {
        [Display(Name = "Código")]
        public int UserId { get; set; }

        [Display(Name = "Nombre(s)")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Apellido(s)")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        public string? Phone { get; set; }

        [Display(Name = "Foto perfil")]
        public string? ProfilePicture { get; set; }

        [Display(Name = "Fecha nacimiento")]
        public DateOnly BirthDate { get; set; }

        [Display(Name = "Grupo sanguíneo")]
        public string? BloodType { get; set; }
    }
}
