namespace _PerfectPickUsers_MS.Models.Login
{
    public class PasswordChangeModel
    {
        public string Email { get; set; } = null!;
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
