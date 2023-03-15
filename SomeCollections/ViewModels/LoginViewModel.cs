using System.ComponentModel.DataAnnotations;

namespace SomeCollections.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Login is empty")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is empty")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
