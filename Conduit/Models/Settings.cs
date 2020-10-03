using System.ComponentModel.DataAnnotations;

namespace Conduit.Models
{
    public class Settings
    {
        [Required(ErrorMessage = "{0} can't be blank")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} can't be blank")]
        public string Password { get; set; }
        [Required(ErrorMessage = "{0} can't be blank")]
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
    }
}
