using SomeCollections.Models;
using System.ComponentModel.DataAnnotations;

namespace SomeCollections.ViewModels
{
    public class CollectionsViewModel
    {
        [Required(ErrorMessage = "Пустое поле!")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Пустое поле!")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Требуется выбор!")]
        [Display(Name = "Theme")]
        public int Tag { get; set; }
    }
}