using System.ComponentModel.DataAnnotations;

namespace Conduit.Models
{
    public class Register
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} can't be blank")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} can't be blank")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} can't be blank")]
        [StringLength(72, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

    }
}
