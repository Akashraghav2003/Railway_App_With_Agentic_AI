using System;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public class UserDTO
    {
        [Required(ErrorMessage = "Please provide name")]
        [StringLength(50, ErrorMessage = "Name should not be greater than 50 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Give the gender")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Please give age between 10 to 100")]
        [Range(15, 100, ErrorMessage = "Age must be greater than 10 and less than 100")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Give the address")]
        public string? Address { get; set; }

        [EmailAddress(ErrorMessage = "Give correct email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Give correct phone number")]
        public long Phone { get; set; }

        [Required(ErrorMessage = "Give the username")]
        [RegularExpression("^[a-zA-Z0-9]{3,20}$", ErrorMessage = "Give valid username without using space and special character")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\\W).{6,20}$", ErrorMessage = "Give a valid password with at least one digit, one lowercase letter, one uppercase letter, and one special character, and between 6 and 20 characters long")]
        public string? Password { get; set; }
    }
}
