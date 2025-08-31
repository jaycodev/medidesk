using DocumentFormat.OpenXml.Wordprocessing;
using Web.Models.User;

namespace medical_appointment_system.Models.ViewModels
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string ProfilePicture { get; set; }

        public ChangePasswordValidator ChangePassword { get; set; }
    }
}


