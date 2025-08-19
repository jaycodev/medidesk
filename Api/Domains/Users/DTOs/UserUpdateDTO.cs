using System.ComponentModel.DataAnnotations;

namespace Api.Domains.Users.DTOs
{
    public class UserUpdateDTO
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

    }
}
