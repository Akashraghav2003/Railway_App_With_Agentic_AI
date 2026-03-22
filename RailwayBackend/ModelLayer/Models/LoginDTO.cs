using System;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models;

public class LoginDTO
{
    
    [Required(ErrorMessage = "Give the username")]
    public string? EmailOrUserName {get; set;}

    [Required(ErrorMessage = "Password is required")]
    public string? Password{get; set;}
}
