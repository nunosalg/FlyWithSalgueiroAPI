using System.ComponentModel.DataAnnotations;

namespace FlyWithSalgueiroAPI.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string? Email { get; set; }


        [Required]
        public string? Token { get; set; }


        [Required]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }


        [Required]
        [Compare("NewPassword")]
        [Display(Name = "Confirm Password")]
        public string? Confirm { get; set; }
    }
}
