using System.ComponentModel.DataAnnotations;

namespace ChatDemo.API.ViewModels
{
    public class AppUserLoginVM
    {
        [Required(ErrorMessage = "Lütfen e-posta adresini boş geçmeyiniz.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Lütfen uygun formatta e-posta adresi giriniz.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lütfen şifreyi boş geçmeyiniz.")]
        [MinLength(3,ErrorMessage = "Şifre 3 karakterden az olamaz.")]
        [DataType(DataType.Password, ErrorMessage = "Lütfen uygun formatta şifre giriniz.")]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        public bool Lock { get; set; }
    }
}
