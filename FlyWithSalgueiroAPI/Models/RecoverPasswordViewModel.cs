using System.ComponentModel.DataAnnotations;

namespace FlyWithSalgueiroAPI.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
