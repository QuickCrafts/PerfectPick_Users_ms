using System;
using System.Collections.Generic;

namespace _PerfectPickUsers_MS.DB;

public partial class ResToken
{
    public string Token { get; set; } = null!;

    public int IdUser { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
