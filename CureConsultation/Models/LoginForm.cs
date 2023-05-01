using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CureConsultation.Models
{
    public class LoginForm
    {
        [Required(ErrorMessage = "Please enter email")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        
    }
}
