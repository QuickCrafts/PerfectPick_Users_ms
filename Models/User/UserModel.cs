using System.Text.Json.Serialization;

namespace _4UUsers.Models.User
{
    public class UserModel
    {
        public int? ID { get; set; }

        public string FirstName { get; set; }

        public string? LastName { get; set; }

        public string Email { get; set; }

        public string? Password { get; set; }


    }
}
