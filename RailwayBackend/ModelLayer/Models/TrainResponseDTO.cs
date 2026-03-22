using System;

namespace ModelLayer.Models;

public class TrainResponseDTO
{
    public int TrainID {get; set;}
    public string? TrainName { get; set; }
    public string? SourceStation { get; set; }
    public string? DestinationStation { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int TotalSeats { get; set; }

    public List<TrainClassResponseDTO>? TrainClasses { get; set; }
}
