using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Entity;

public class Train
{
    [Key]
    public int TrainId { get; set; }

    [Required]

    public string? TrainName { get; set; }

    [Required]

    public string? SourceStation { get; set; }

    [Required]

    public string? DestinationStation { get; set; }

    [Required]
    public DateTime DepartureTime { get; set; }

    [Required]
    public DateTime ArrivalTime { get; set; }

    [Required]
    public int TotalSeats { get; set; }


    public virtual ICollection<Reservation>? Reservations { get; set; }
    public virtual ICollection<TrainClass>? TrainClasses { get; set; }


}

