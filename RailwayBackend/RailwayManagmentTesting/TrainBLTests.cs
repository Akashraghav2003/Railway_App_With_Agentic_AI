using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BusinessLayer.Service;
using BusinessLayer.Interface;
using ModelLayer.Models;
using RepositoryLayer.Interface;
using RepositoryLayer.Entity;
using AutoMapper;

[TestFixture]
public class TrainBLTests
{
    private Mock<ITrainRL> _mockTrainRL;
    private Mock<ILogger<RailwayBusinessLayer>> _mockLogger;
    private Mock<IEmailService> _mockEmailService;
    private Mock<IMapper> _mockMapper;
    private Mock<IJwtToken> _mockJWTToken;
    private TrainBL _trainBL;

    [SetUp]
    public void Setup()
    {
        _mockTrainRL = new Mock<ITrainRL>();
        _mockLogger = new Mock<ILogger<RailwayBusinessLayer>>();
        _mockEmailService = new Mock<IEmailService>();
        _mockMapper = new Mock<IMapper>();
        _mockJWTToken = new Mock<IJwtToken>();
        _trainBL = new TrainBL(
            _mockTrainRL.Object,
            _mockLogger.Object,
            _mockEmailService.Object,
            _mockMapper.Object,
            _mockJWTToken.Object
        );
    }

    [Test]
    public async Task AddTrainAsync_ValidTrain_ReturnsTrue()
    {
        // Arrange
        var trainDTO = new TrainDTO
        {
            TrainName = "Express",
            SourceStation = "Station A",
            DestinationStation = "Station B",
            DepartureTime = DateTime.Now.AddHours(1),
            ArrivalTime = DateTime.Now.AddHours(5),
            TotalSeats = 100
        };

        var train = new Train
        {
            TrainId = 1,
            TrainName = "Express",
            SourceStation = "Station A",
            DestinationStation = "Station B",
            DepartureTime = DateTime.Now.AddHours(1),
            ArrivalTime = DateTime.Now.AddHours(5),
            TotalSeats = 100
        };

        _mockTrainRL.Setup(rl => rl.GetTrainAsync(trainDTO)).ReturnsAsync((Train)null);
        _mockMapper.Setup(m => m.Map<Train>(trainDTO)).Returns(train);
        _mockTrainRL.Setup(rl => rl.AddTrainAsync(train)).ReturnsAsync(1);

        // Act
        var result = await _trainBL.AddTrainAsync(trainDTO);

        // Assert
        Assert.That(result, Is.True);
        _mockTrainRL.Verify(rl => rl.AddTrainAsync(It.IsAny<Train>()), Times.Once);
    }

    [Test]
    public async Task UpdateTrainAsync_ValidData_ReturnsTrue()
    {
        // Arrange
        int trainId = 1;
        var trainDTO = new TrainDTO
        {
            TrainName = "Express Updated",
            SourceStation = "Station C",
            DestinationStation = "Station D",
            DepartureTime = DateTime.Now.AddHours(2),
            ArrivalTime = DateTime.Now.AddHours(6),
            TotalSeats = 120,
            TrainClass = new List<TrainClassDTO>
            {
                new TrainClassDTO { ClassName = "First Class", TotalSeat = 60, Fare = 3500 }
            }
        };

        var existingTrain = new Train
        {
            TrainId = trainId,
            TrainName = "Express",
            SourceStation = "Station A",
            DestinationStation = "Station B"
        };

        var trainClassEntities = new List<TrainClass>
        {
            new TrainClass { ClassId = 1, ClassName = "First Class", TotalSeat = 60, Fare = 3500, TrainId = trainId }
        };

        _mockTrainRL.Setup(rl => rl.GetTrainByIdAsync(trainId)).ReturnsAsync(existingTrain);
        _mockMapper.Setup(m => m.Map(trainDTO, existingTrain));
        _mockMapper.Setup(m => m.Map<List<TrainClass>>(trainDTO.TrainClass)).Returns(trainClassEntities);
        _mockTrainRL.Setup(rl => rl.UpdateTrainAsync(existingTrain)).Returns(Task.CompletedTask);
        _mockTrainRL.Setup(rl => rl.UpdateTrainClassesAsync(trainId, trainClassEntities)).Returns(Task.CompletedTask);

        // Act
        var result = await _trainBL.UpdateTrainAsync(trainId, trainDTO);

        // Assert
        Assert.That(result, Is.True);
        _mockTrainRL.Verify(rl => rl.UpdateTrainAsync(existingTrain), Times.Once);
        _mockTrainRL.Verify(rl => rl.UpdateTrainClassesAsync(trainId, It.IsAny<List<TrainClass>>()), Times.Once);
    }

    [Test]
    public void UpdateTrainAsync_TrainNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        int trainId = 999;
        var trainDTO = new TrainDTO
        {
            TrainName = "Express Updated",
            SourceStation = "Station C",
            DestinationStation = "Station D"
        };

        _mockTrainRL.Setup(rl => rl.GetTrainByIdAsync(trainId)).ReturnsAsync((Train)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _trainBL.UpdateTrainAsync(trainId, trainDTO));
        Assert.That(ex.Message, Is.EqualTo("TrainID not present"));
    }

    [Test]
    public async Task AddReservationAsync_ValidData_ReturnsConfirmationMessage()
    {
        // Arrange
        var reservationDTO = new ReservationDTO
        {
            UserID = 1,
            TrainId = 1,
            ClassId = 1,
            NoOfSeats = 2,
            TravelDate = DateTime.Now.AddDays(1)
        };

        var trainClass = new TrainClass
        {
            ClassId = 1,
            ClassName = "First Class",
            TotalSeat = 50,
            Fare = 3000
        };

        var train = new Train
        {
            TrainId = 1,
            TrainName = "Express",
            TotalSeats = 100
        };

        var reservation = new Reservation
        {
            ReservationId = 1,
            UserID = 1,
            TrainId = 1,
            ClassId = 1,
            NoOfSeats = 2,
            TotalFare = 6000,
            PNRNumber = 123456789,
            BookingStatus = "Booked"
        };

        string userEmail = "test@example.com";

        _mockTrainRL.Setup(rl => rl.CheckSeatAvailabilityAsync(reservationDTO.TrainId, reservationDTO.ClassId, reservationDTO.NoOfSeats))
                   .ReturnsAsync((trainClass, train));
        _mockMapper.Setup(m => m.Map<Reservation>(reservationDTO)).Returns(reservation);
        _mockTrainRL.Setup(rl => rl.GetEmailAsync(reservationDTO.UserID)).ReturnsAsync(userEmail);
        _mockTrainRL.Setup(rl => rl.DeductSeatsAsync(trainClass, train, reservationDTO.NoOfSeats)).Returns(Task.CompletedTask);
        _mockTrainRL.Setup(rl => rl.AddReservationAsync(reservation)).ReturnsAsync(reservation);

        // Act
        var result = await _trainBL.AddReservationAsync(reservationDTO);

        // Assert
        Assert.That(result, Does.Contain("Ticket booked"));
        Assert.That(result, Does.Contain(reservation.ReservationId.ToString()));
        Assert.That(result, Does.Contain(reservation.PNRNumber.ToString()));
        _mockEmailService.Verify(e => e.SendEmail(It.IsAny<EmailModel>()), Times.Once);
    }

    [Test]
    public void AddReservationAsync_TrainNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var reservationDTO = new ReservationDTO
        {
            UserID = 1,
            TrainId = 999,
            ClassId = 1,
            NoOfSeats = 2
        };

        _mockTrainRL.Setup(rl => rl.CheckSeatAvailabilityAsync(reservationDTO.TrainId, reservationDTO.ClassId, reservationDTO.NoOfSeats))
                   .ThrowsAsync(new KeyNotFoundException("Train not found"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _trainBL.AddReservationAsync(reservationDTO));
        Assert.That(ex.Message, Is.EqualTo("Train not found"));
    }

    [Test]
    public async Task CancelReservationAsync_ValidData_ReturnsTrue()
    {
        // Arrange
        var cancellationDTO = new CancellationDTO
        {
            ReservationId = 1,
            PNRNumber = 123456789,
            Reason = "Change of plans"
        };

        var reservation = new Reservation
        {
            ReservationId = 1,
            UserID = 1,
            TrainId = 1,
            ClassId = 1,
            NoOfSeats = 2,
            PNRNumber = 123456789,
            BookingStatus = "Booked"
        };

        var cancellation = new Cancellation
        {
            ReservationId = 1,
            Reason = "Change of plans"
        };

        _mockTrainRL.Setup(rl => rl.GetReservationByIdAsync(cancellationDTO.ReservationId)).ReturnsAsync(reservation);
        _mockMapper.Setup(m => m.Map<Cancellation>(cancellationDTO)).Returns(cancellation);
        _mockTrainRL.Setup(rl => rl.RestoreSeatsAsync(reservation)).Returns(Task.CompletedTask);
        _mockTrainRL.Setup(rl => rl.LogCancellationAsync(cancellation)).Returns(Task.CompletedTask);
        _mockTrainRL.Setup(rl => rl.CancelReservationAsync(reservation)).Returns(Task.CompletedTask);

        // Act
        var result = await _trainBL.CancelReservationAsync(cancellationDTO);

        // Assert
        Assert.That(result, Is.True);
        _mockTrainRL.Verify(rl => rl.RestoreSeatsAsync(reservation), Times.Once);
        _mockTrainRL.Verify(rl => rl.LogCancellationAsync(It.IsAny<Cancellation>()), Times.Once);
        _mockTrainRL.Verify(rl => rl.CancelReservationAsync(reservation), Times.Once);
    }

    [Test]
    public async Task CancelReservationAsync_ReservationNotFound_ReturnsFalse()
    {
        // Arrange
        var cancellationDTO = new CancellationDTO
        {
            ReservationId = 999,
            PNRNumber = 123456789,
            Reason = "Change of plans"
        };

        _mockTrainRL.Setup(rl => rl.GetReservationByIdAsync(cancellationDTO.ReservationId)).ReturnsAsync((Reservation)null);

        // Act
        var result = await _trainBL.CancelReservationAsync(cancellationDTO);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CancelTrainAsync_ValidTrainId_ReturnsTrue()
    {
        // Arrange
        int trainId = 1;
        var train = new Train
        {
            TrainId = trainId,
            TrainName = "Express"
        };

        _mockTrainRL.Setup(rl => rl.GetTrainByIdAsync(trainId)).ReturnsAsync(train);
        _mockTrainRL.Setup(rl => rl.CancelTrainAsync(trainId)).ReturnsAsync(true);

        // Act
        var result = await _trainBL.CancelTrainAsync(trainId);

        // Assert
        Assert.That(result, Is.True);
        _mockTrainRL.Verify(rl => rl.CancelTrainAsync(trainId), Times.Once);
    }

    [Test]
    public void CancelTrainAsync_TrainNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        int trainId = 999;
        _mockTrainRL.Setup(rl => rl.GetTrainByIdAsync(trainId)).ReturnsAsync((Train)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _trainBL.CancelTrainAsync(trainId));
        Assert.That(ex.Message, Is.EqualTo("Train not found"));
    }
}