using System.ComponentModel.DataAnnotations;

namespace FlyWithSalgueiroAPI.Models
{
    public class RecoverPasswordModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
