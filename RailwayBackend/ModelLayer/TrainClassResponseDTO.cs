using System;

namespace ModelLayer;

public class TrainClassResponseDTO
{
    public int ClassId {get; set;}
    public string? ClassName { get; set; }
    public int TotalSeat { get; set; }
    public decimal Fare { get; set; }
}
