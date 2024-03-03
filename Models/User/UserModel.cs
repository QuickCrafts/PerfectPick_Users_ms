using System.Text.Json.Serialization;

namespace _PerfectPickUsers_MS.Models.User
{
    public class UserModel
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public bool IsAdmin { get; set; }


    }
}
