using System.ComponentModel.DataAnnotations;

namespace SomeCollections.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Emptyfield")]
        [Display(Name = "Логин")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Emptyfield")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Emptyfield")]
        [DataType(DataType.Password)]
        [StringLength(12, ErrorMessage = "Password length less than 4", MinimumLength = 4)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "PassConf")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}
