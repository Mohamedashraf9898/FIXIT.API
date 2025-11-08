using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{
    
    public class LocationFormatAttribute : ValidationAttribute
    {
        private readonly List<string> allowedGovernorates = new()
    {
        "Cairo",
        "Giza",
        "Alexandria",
        "Mansoura",
        "Sharqia"
    };

        private readonly Dictionary<string, List<string>> allowedCities = new()
    {
        // القاهرة
        { "Cairo", new List<string> { "Nasr City", "Heliopolis", "Maadi", "New Cairo" } },

        // الجيزة
        { "Giza", new List<string> { "6 October", "Dokki", "Faisal", "Haram" } },

        // الإسكندرية
        { "Alexandria", new List<string> { "Sidi Gaber", "Smouha", "Stanley", "Gleem" } },

        // المنصورة
        { "Mansoura", new List<string> { "Talkha", "Tora", "Mit Ghamr", "Aga" } },

        // الشرقية
        { "Sharqia", new List<string> { "Zagazig", "10th of Ramadan", "Abu Hammad", "Belbeis" } },
    };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var parts = value.ToString().Split(',');
            if (parts.Length != 3)
                return new ValidationResult("Location must be in format 'Governorate,City,Village'.");

            var governorate = parts[0].Trim();
            var city = parts[1].Trim();
            var village = parts[2].Trim();

            // Governorate check
            if (!allowedGovernorates.Contains(governorate))
                return new ValidationResult($"Governorate '{governorate}' is not valid.");

            // City check
            if (!allowedCities.ContainsKey(governorate) || !allowedCities[governorate].Contains(city))
                return new ValidationResult($"City '{city}' is not valid for governorate '{governorate}'.");

            // Village name check
            if (string.IsNullOrWhiteSpace(village))
                return new ValidationResult("Village name cannot be empty.");

            return ValidationResult.Success;
        }
    }

}
