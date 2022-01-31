using Microsoft.AspNetCore.Http;
using SomeCollections.Settings;
using System.ComponentModel.DataAnnotations;

namespace SomeCollections.ViewModels
{
    public class CollectionsViewModel
    {
        [Required(ErrorMessage = "Emptyfield")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Emptyfield")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Emptyfield")]
        [Display(Name = "Theme")]
        public int Tag { get; set; }

        [Display(Name = "Img")]
        [MaxFileSize(1 * 1080 * 1080)]
        [AvailableImgFormat(new string[] { ".jpg", ".png", ".gif", ".jpeg" })]
        public IFormFile Img { get; set; }
    }
}