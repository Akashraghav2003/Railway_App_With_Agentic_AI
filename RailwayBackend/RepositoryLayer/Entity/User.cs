using System;
using System.ComponentModel.DataAnnotations;



namespace RepositoryLayer.Entity;

public class User
{
    [Key]
    public int UserId {get; set;}

    [Required]
    public string? Name {get; set;}
    [Required]
    public string? Gender {get; set;}

    [Required]
    public int Age {get; set;}

    [Required]
    public string? Address {get; set;}

    [Required] 
    public string? Email{get; set;}

    [Required]
    public long Phone {get; set;}

    [Required]
    public string? UserName {get; set;}
    
    [Required]
    public string? Password {get; set;}

    [Required]
    public string? Role {get; set;}

    public virtual ICollection<Reservation>? Reservations { get; set; }

}
