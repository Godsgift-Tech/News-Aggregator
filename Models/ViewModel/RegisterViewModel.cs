using System.ComponentModel.DataAnnotations;

namespace News_Aggregator.Models
{
    public class RegisterViewModel
    {
        [Required]

        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Invalid Password lenght", MinimumLength = 6)]

        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password do not Match")]
        public string ConfirmPassword { get; set; }
        
    }
}
