﻿using System.Text.Json.Serialization;

namespace _PerfectPickUsers_MS.Models.User
{
    public class UserModel
    {
        public int IdUser { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Birthdate { get; set; } = null!;

        public bool Role { get; set; }

        public string? AvatarUrl { get; set; }

        public string? CreatedTime { get; set; }

        public bool Verified { get; set; }

        public bool Setup { get; set; }

        public int? CountryId { get; set; }

        public string? Gender { get; set; }
        


    }
}
