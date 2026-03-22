using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Entity;



public class TrainClass
{
    [Key]
    public int ClassId { get; set; }

    
    [ForeignKey("Train")]
    public int TrainId { get; set; }


    [Required]
    
    public string? ClassName { get; set; }

    [Required]
    public int TotalSeat { get; set; }

    [Required]
    public decimal Fare { get; set; }

    public virtual Train? Train { get; set; }
}

