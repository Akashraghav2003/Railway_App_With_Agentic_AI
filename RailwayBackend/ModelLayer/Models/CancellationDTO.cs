using System;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models;

public class CancellationDTO
{
    [Required(ErrorMessage = "Enter the PNR Number")]
    public int PNRNumber { get; set; }

    [Required(ErrorMessage = "Enter the Reason")]
    public string? Reason { get; set; }

    [Required(ErrorMessage ="Enter the reservation number")]
    public int ReservationId { get; set; }
    

}
