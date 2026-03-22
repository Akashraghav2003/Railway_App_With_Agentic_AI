using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Entity;
public class Cancellation
{
    [Key]
    public int CancellationId { get; set; }

    [Required]
    public int PNRNumber { get; set; }

    [Required]
    
    public string? Reason { get; set; }

    [Required]
    
    public int? ReservationId { get; set; }

    [Required]
    public DateTime CancellationDate { get; set; }
  
}

