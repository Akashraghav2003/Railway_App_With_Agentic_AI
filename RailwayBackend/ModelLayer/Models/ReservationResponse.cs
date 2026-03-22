using System;


namespace ModelLayer.Models;

public class ReservationResponse
{

    public int ReservationId { get; set; }
    public string TrainName { get; set; }
    public int TrainId { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public DateTime TravelDate { get; set; }
    public int NoOfSeats { get; set; }
    public int PNRNumber { get; set; }
    public string? BookingStatus { get; set; }
    public double TotalFare { get; set; }

    public List<PassengerResponse>? Passengers { get; set; }

}
