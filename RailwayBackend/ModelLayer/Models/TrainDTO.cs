using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public class TrainDTO
    {
        [Required(ErrorMessage = "TrainName is required.")]
        [StringLength(100, ErrorMessage = "TrainName can't be longer than 100 characters.")]
        public string? TrainName { get; set; }

        [Required(ErrorMessage = "SourceStation is required.")]
        [StringLength(100, ErrorMessage = "SourceStation can't be longer than 100 characters.")]
        public string? SourceStation { get; set; }

        [Required(ErrorMessage = "DestinationStation is required.")]
        [StringLength(100, ErrorMessage = "DestinationStation can't be longer than 100 characters.")]
        public string? DestinationStation { get; set; }

        [Required(ErrorMessage = "DepartureTime is required.")]
        [DataType(DataType.DateTime)]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "ArrivalTime is required.")]
        [DataType(DataType.DateTime)]
        public DateTime ArrivalTime { get; set; }

        [Required(ErrorMessage = "TotalSeats is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "TotalSeats must be at least 1.")]
        public int TotalSeats { get; set; }

        [Required(ErrorMessage = "TrainClass is required.")]
        [MinLength(1, ErrorMessage = "At least one TrainClass is required.")]
        public List<TrainClassDTO>? TrainClass { get; set; }
    }
}
