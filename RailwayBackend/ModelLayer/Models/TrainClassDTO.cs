using System;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public class TrainClassDTO
    {
        [Required(ErrorMessage = "ClassName is required.")]
        [StringLength(50, ErrorMessage = "ClassName can't be longer than 50 characters.")]
        public string? ClassName { get; set; }

        [Required(ErrorMessage = "TotalSeat is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "TotalSeat must be at least 1.")]
        public int TotalSeat { get; set; }

        [Required(ErrorMessage = "Fare is required.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Fare must be a positive value.")]
        public decimal Fare { get; set; }
    }
}
