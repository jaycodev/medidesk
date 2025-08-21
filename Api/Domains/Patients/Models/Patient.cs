using System;
using System.ComponentModel.DataAnnotations;
using Api.Domains.Users.Models;

namespace Api.Domains.Patients.Models
{
    public class Patient : User
    {
        public DateTime? BirthDate { get; set; }
        public string BloodType { get; set; }
    }
}