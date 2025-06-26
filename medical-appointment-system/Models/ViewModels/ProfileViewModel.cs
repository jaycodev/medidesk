using medical_appointment_system.Models.Validators;

namespace medical_appointment_system.Models.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }

        public ChangePasswordValidator ChangePassword { get; set; }

        public ChangePhoneValidator ChangePhone { get; set; }
    }
}