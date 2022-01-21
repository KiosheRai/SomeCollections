using System;
using System.ComponentModel.DataAnnotations;

namespace SomeCollections.ViewModels
{
    public class ItemViewModel
    {
        [Required(ErrorMessage = "Пустое поле!")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Пустое поле!")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
