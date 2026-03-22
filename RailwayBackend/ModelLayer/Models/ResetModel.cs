using System;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models;

public class ResetModel
{
    [Required(ErrorMessage = "Token not provided")]
    public string? Token {get; set;}

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\\W).{6,20}$", ErrorMessage = "Give a valid password with at least one digit, one lowercase letter, one uppercase letter, and one special character, and between 6 and 20 characters long")]
    public string? NewPassword {get; set;}

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\\W).{6,20}$", ErrorMessage = "Give a valid password with at least one digit, one lowercase letter, one uppercase letter, and one special character, and between 6 and 20 characters long")]
     public string? ConfirmPassword {get; set;}
}
