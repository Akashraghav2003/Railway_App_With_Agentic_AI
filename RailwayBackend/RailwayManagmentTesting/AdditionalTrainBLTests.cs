using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using BusinessLayer.Service;
using BusinessLayer.Interface;
using ModelLayer.Models;
using RepositoryLayer.Interface;
using RepositoryLayer.Entity;
using AutoMapper;

[TestFixture]
public class AdditionalTrainBLTests
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
        _trainBL = new TrainBL(_mockTrainRL.Object, _mockLogger.Object, _mockEmailService.Object, _mockMapper.Object, _mockJWTToken.Object);
    }


    [Test]
    public async Task CancelTrainAsync_ValidId_ReturnsTrue()
    {
        
        int trainId = 1;
        var existingTrain = new Train { TrainId = trainId, TrainName = "Test Train" };
        _mockTrainRL.Setup(rl => rl.GetTrainByIdAsync(trainId)).ReturnsAsync(existingTrain);
        _mockTrainRL.Setup(rl => rl.CancelTrainAsync(trainId)).ReturnsAsync(true);

        
        var result = await _trainBL.CancelTrainAsync(trainId);

        
        Assert.That(result, Is.True);
        _mockTrainRL.Verify(rl => rl.CancelTrainAsync(trainId), Times.Once);
    }

    [Test]
    public async Task GetReservationsByUserIdAsync_ValidId_ReturnsReservations()
    {
        // Arrange
        int userId = 1;
        var reservations = new List<Reservation>
        {
            new Reservation { ReservationId = 1, UserID = userId, TrainId = 1 }
        };
        var reservationResponses = new List<ReservationResponse>
        {
            new ReservationResponse { ReservationId = 1, TrainId = 1 }
        };
        _mockTrainRL.Setup(rl => rl.GetReservationsByUserIdAsync(userId)).ReturnsAsync(reservations);
        _mockMapper.Setup(m => m.Map<List<ReservationResponse>>(reservations)).Returns(reservationResponses);

        
        var result = await _trainBL.GetReservationsByUserIdAsync(userId);

        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        _mockTrainRL.Verify(rl => rl.GetReservationsByUserIdAsync(userId), Times.Once);
    }


}