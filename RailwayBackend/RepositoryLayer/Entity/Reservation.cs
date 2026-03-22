using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Entity;

public class Reservation
{

    [Key]
    public int ReservationId { get; set; }


    [ForeignKey("User")]
    public int UserID { get; set; }


    [ForeignKey("Train")]
    public int TrainId { get; set; }

    [Required]
    public DateTime TravelDate { get; set; }

    [Required]
    public int ClassId { get; set; }
    [Required]
    public int NoOfSeats { get; set; }

    [Required]
    public int PNRNumber { get; set; }

    [Required]

    public string? BookingStatus { get; set; }

    [Required]

    public string? Quota { get; set; }

    [Required]
    public string? BankName { get; set; }

    [Required]
    public double TotalFare { get; set; }



    public virtual User? User { get; set; }
    public virtual Train? Train { get; set; }
    public virtual ICollection<Passenger>? Passenger { get; set; }
    


}
