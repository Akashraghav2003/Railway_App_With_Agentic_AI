using System;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public class PassengerDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120.")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other.")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "AdharCard is required.")]
        public long? AdharCard { get; set; }
    }
}
