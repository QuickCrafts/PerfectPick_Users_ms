using System.ComponentModel.DataAnnotations;

namespace _PerfectPickUsers_MS.Models.Contact
{
    public class ContactModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Message { get; set; } = null!;
    }
}
