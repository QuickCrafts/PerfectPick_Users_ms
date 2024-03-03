using System;
using System.Collections.Generic;

namespace _PerfectPickUsers_MS.DB;

public partial class User
{
    public string UserName { get; set; } = null!;

    public string UserSurname { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public bool UserIsAdmin { get; set; }
}
