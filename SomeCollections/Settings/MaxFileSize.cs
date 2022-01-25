using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SomeCollections.Settings
{
    public class MaxFileSize : ValidationAttribute
    {
        private readonly int maxFileSize;
        public MaxFileSize(int maxFileSize)
        {
            this.maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (file != null)
            {
                if (file.Length > maxFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Файл слишком большой";
        }
    }
}
