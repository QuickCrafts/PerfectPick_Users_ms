using System;
using System.Collections.Generic;

namespace _PerfectPickUsers_MS.DB;

public partial class User
{
    public int IdUser { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Birthdate { get; set; } = null!;

    public string? Gender { get; set; }

    public string CreatedTime { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public int? IdCountry { get; set; }

    public bool Verified { get; set; }

    public bool Setup { get; set; }

    public virtual ResToken? ResToken { get; set; }
}
