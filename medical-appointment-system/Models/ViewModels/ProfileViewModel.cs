using medical_appointment_system.Models.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace medical_appointment_system.Models.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public ChangePasswordValidator ChangePassword { get; set; }
        public ChangePhoneValidator ChangePhone { get; set; }
    }
}