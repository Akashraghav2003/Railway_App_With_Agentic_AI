using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Entity
{
    public class Passenger
    {
        [Key]
        public int PassengerId { get; set; }

        
        [ForeignKey("ReservationId")]
        public int ReservationId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public int? Age { get; set; }

        [Required]
        public string? Gender { get; set; }

        [Required]
        public long? AdharCard { get; set; }

        public virtual Reservation? Reservation { get; set; }
    }
}
