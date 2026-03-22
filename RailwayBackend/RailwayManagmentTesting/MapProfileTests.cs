using NUnit.Framework;
using AutoMapper;
using BusinessLayer.Service;
using ModelLayer.Models;
using RepositoryLayer.Entity;

[TestFixture]
public class MapProfileTests
{
    private IMapper _mapper;

    [SetUp]
    public void Setup()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapProfile>());
        _mapper = config.CreateMapper();
    }

    [Test]
    public void UserDTO_To_User_MapsCorrectly()
    {
        // Arrange
        var userDTO = new UserDTO
        {
            Name = "Test User",
            Email = "test@example.com",
            UserName = "testuser",
            Password = "password123",
            Age = 25,
            Gender = "Male",
            Address = "Test Address",
            Phone = 1234567890
        };

        // Act
        var user = _mapper.Map<User>(userDTO);

        // Assert
        Assert.That(user.Name, Is.EqualTo(userDTO.Name));
        Assert.That(user.Email, Is.EqualTo(userDTO.Email));
        Assert.That(user.UserName, Is.EqualTo(userDTO.UserName));
    }

    [Test]
    public void Train_To_TrainDTO_MapsCorrectly()
    {
        // Arrange
        var train = new Train
        {
            TrainId = 1,
            TrainName = "Express",
            SourceStation = "Station A",
            DestinationStation = "Station B",
            TotalSeats = 100,
            TrainClasses = new List<TrainClass>
            {
                new TrainClass { ClassId = 1, ClassName = "First Class", Fare = 3000, TotalSeat = 50 }
            }
        };

        // Act
        var trainDTO = _mapper.Map<TrainDTO>(train);

        // Assert
        Assert.That(trainDTO.TrainName, Is.EqualTo(train.TrainName));
        Assert.That(trainDTO.TrainClass.Count, Is.EqualTo(1));
        Assert.That(trainDTO.TrainClass.First().ClassName, Is.EqualTo("First Class"));
    }

    [Test]
    public void Reservation_To_ReservationDTO_MapsCorrectly()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationId = 1,
            UserID = 1,
            TrainId = 1,
            NoOfSeats = 2,
            Passenger = new List<Passenger>
            {
                new Passenger { Name = "John Doe", Age = 30, Gender = "Male" }
            }
        };

        // Act
        var reservationDTO = _mapper.Map<ReservationDTO>(reservation);

        // Assert
        Assert.That(reservationDTO.UserID, Is.EqualTo(reservation.UserID));
        Assert.That(reservationDTO.Passenger.Count, Is.EqualTo(1));
        Assert.That(reservationDTO.Passenger.First().Name, Is.EqualTo("John Doe"));
    }

    [Test]
    public void Passenger_To_PassengerDTO_MapsCorrectly()
    {
        // Arrange
        var passenger = new Passenger
        {
            PassengerId = 1,
            Name = "Jane Doe",
            Age = 25,
            Gender = "Female",
            AdharCard = 123456789012
        };

        // Act
        var passengerDTO = _mapper.Map<PassengerDTO>(passenger);

        // Assert
        Assert.That(passengerDTO.Name, Is.EqualTo(passenger.Name));
        Assert.That(passengerDTO.Age, Is.EqualTo(passenger.Age));
        Assert.That(passengerDTO.Gender, Is.EqualTo(passenger.Gender));
    }

    [Test]
    public void Cancellation_To_CancellationDTO_MapsCorrectly()
    {
        // Arrange
        var cancellation = new Cancellation
        {
            CancellationId = 1,
            PNRNumber = 123456789,
            Reason = "Test cancellation",
            ReservationId = 1
        };

        // Act
        var cancellationDTO = _mapper.Map<CancellationDTO>(cancellation);

        // Assert
        Assert.That(cancellationDTO.PNRNumber, Is.EqualTo(cancellation.PNRNumber));
        Assert.That(cancellationDTO.Reason, Is.EqualTo(cancellation.Reason));
    }
}