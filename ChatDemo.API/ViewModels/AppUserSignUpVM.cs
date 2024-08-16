using System.ComponentModel.DataAnnotations;

namespace ChatDemo.API.ViewModels
{
    public class AppUserSignUpVM
    {
        [Required(ErrorMessage = "Username is required...")]
        [StringLength(15, ErrorMessage = "Username must be at least 4 and maximum 16 characters...", MinimumLength = 4)]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "Email is required...")]
        [EmailAddress(ErrorMessage = "Email must be in correct mail format...")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required...")]
        [DataType(DataType.Password, ErrorMessage = "Please consider all the rules of creating password...")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
