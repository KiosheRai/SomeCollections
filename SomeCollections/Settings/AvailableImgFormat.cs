using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace SomeCollections.Settings
{
    public class AvailableImgFormat : ValidationAttribute
    {
        private readonly string[] permittedExtensions;
        public AvailableImgFormat(string[] permittedExtensions)
        {
            this.permittedExtensions = permittedExtensions;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (!(file == null))
            {
                var extension = Path.GetExtension(file.FileName);
                if (!permittedExtensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Неверный формат файла";
        }
    }
}
