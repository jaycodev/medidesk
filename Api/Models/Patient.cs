using System;
using System.ComponentModel.DataAnnotations;
using Api.Domains.Users.Models;

namespace Api.Models
{
    public class Patient : User
    {
        [Required(ErrorMessage = "Ingrese una fecha de nacimiento")]
        [Display(Name = "Fecha nacimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Grupo sanguíneo")]
        public string BloodType { get; set; }
    }
}