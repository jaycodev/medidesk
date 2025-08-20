using Web.Models.User;

namespace medical_appointment_system.Models.ViewModels
{
    public class UserDetailsViewModel
    {
        public User User { get; set; }

        public Doctor Doctor { get; set; }

        public Patient Patient { get; set; }
    }
}