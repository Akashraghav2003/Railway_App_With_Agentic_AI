using System;

namespace ModelLayer.Models;

public class PassengerResponse
{

    public int PassengerId { get; set; }
    public string? Name { get; set; }
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public long? AdharCard { get; set; }

}
