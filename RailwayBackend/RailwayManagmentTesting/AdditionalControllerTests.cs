using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interface;
using RailwayManagmentSystem.Controllers;
using ModelLayer.Models;
using RepositoryLayer.Entity;

[TestFixture]
public class AdditionalControllerTests
{
    private Mock<IRailwayBusinessLayer> _mockRailwayBL;
    private Mock<ITrainBL> _mockTrainBL;
    private Mock<ILogger<RailwayTicketBooking>> _mockLogger;
    private RailwayTicketBooking _railwayController;
    private TrainController _trainController;

    [SetUp]
    public void Setup()
    {
        _mockRailwayBL = new Mock<IRailwayBusinessLayer>();
        _mockTrainBL = new Mock<ITrainBL>();
        _mockLogger = new Mock<ILogger<RailwayTicketBooking>>();
        _railwayController = new RailwayTicketBooking(_mockRailwayBL.Object, _mockLogger.Object);
        _trainController = new TrainController(_mockTrainBL.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetUserDetailsById_ValidId_ReturnsOk()
    {
        // Arrange
        int userId = 1;
        var user = new User { UserId = userId, Name = "Test User", Email = "test@example.com" };
        _mockRailwayBL.Setup(bl => bl.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _railwayController.GetUserDetailsById(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetType().Name, Is.EqualTo("OkObjectResult"));
        _mockRailwayBL.Verify(bl => bl.GetUserByIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task GetReservationsByUserId_ValidId_ReturnsOk()
    {
        // Arrange
        int userId = 1;
        var reservations = new List<ReservationResponse>
        {
            new ReservationResponse { ReservationId = 1, TrainId = 1 }
        };
        _mockTrainBL.Setup(bl => bl.GetReservationsByUserIdAsync(userId)).ReturnsAsync(reservations);

        // Act
        var result = await _trainController.GetReservationsByUserId(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetType().Name, Is.EqualTo("ActionResult`1"));
        _mockTrainBL.Verify(bl => bl.GetReservationsByUserIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task UpdateTrain_ValidData_ReturnsOk()
    {
        // Arrange
        int trainId = 1;
        var trainDTO = new TrainDTO
        {
            TrainName = "Updated Express",
            SourceStation = "Station A",
            DestinationStation = "Station B",
            TotalSeats = 150
        };
        _mockTrainBL.Setup(bl => bl.UpdateTrainAsync(trainId, trainDTO)).ReturnsAsync(true);

        // Act
        var result = await _trainController.UpdateTrain(trainId, trainDTO);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetType().Name, Is.EqualTo("OkObjectResult"));
        _mockTrainBL.Verify(bl => bl.UpdateTrainAsync(trainId, trainDTO), Times.Once);
    }

    [Test]
    public async Task CancelTrain_ValidId_ReturnsOk()
    {
        // Arrange
        int trainId = 1;
        _mockTrainBL.Setup(bl => bl.CancelTrainAsync(trainId)).ReturnsAsync(true);

        // Act
        var result = await _trainController.CancelTrain(trainId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetType().Name, Is.EqualTo("OkObjectResult"));
        _mockTrainBL.Verify(bl => bl.CancelTrainAsync(trainId), Times.Once);
    }
}