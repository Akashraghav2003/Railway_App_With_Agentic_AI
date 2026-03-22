using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public class ReservationDTO
    {
        [Required(ErrorMessage = "UserID is required.")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "TrainId is required.")]
        public int TrainId { get; set; }

        [Required(ErrorMessage = "TravelDate is required.")]
        [DataType(DataType.Date)]
        public DateTime TravelDate { get; set; }

        [Required(ErrorMessage = "ClassId is required.")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "NoOfSeats is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "NoOfSeats must be at least 1.")]
        public int NoOfSeats { get; set; }

        [Required(ErrorMessage = "Quota is required.")]
        [StringLength(50, ErrorMessage = "Quota can't be longer than 50 characters.")]
        public string? Quota { get; set; }

        [StringLength(100, ErrorMessage = "BankName can't be longer than 100 characters.")]
        public string? BankName { get; set; }

        [Required(ErrorMessage = "Passenger list is required.")]
        [MinLength(1, ErrorMessage = "At least one passenger is required.")]
        public List<PassengerDTO> Passenger { get; set; } = new List<PassengerDTO>();
    }
}
