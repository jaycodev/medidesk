using System.ComponentModel.DataAnnotations;

namespace Api.Domains.Users.DTOs
{
    public class UserUpdateDTO
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string SelectedRoleCombo { get; set; }

    }
}
