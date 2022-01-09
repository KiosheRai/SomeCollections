using System.ComponentModel.DataAnnotations;

namespace SomeCollections.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Пустое поле!")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Пустое поле!")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
