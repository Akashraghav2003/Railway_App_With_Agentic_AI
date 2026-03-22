using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using RailwayManagmentSystem.Controllers;
using BusinessLayer.Interface;
using ModelLayer.Models;
using ModelLayer;

[TestFixture]
public class TrainControllerTests
{
    private Mock<ITrainBL> _mockTrainBL;
    private Mock<ILogger<RailwayTicketBooking>> _mockLogger;
    private TrainController _controller;

    [SetUp]
    public void Setup()
    {
        _mockTrainBL = new Mock<ITrainBL>();
        _mockLogger = new Mock<ILogger<RailwayTicketBooking>>();
        _controller = new TrainController(_mockTrainBL.Object, _mockLogger.Object);
    }

    [Test]
    public async Task AddTrain_ValidTrain_ReturnsOkResult()
    {
        // Arrange
        var trainDTO = new TrainDTO
        {
            TrainName = "Express",
            SourceStation = "Station A",
            DestinationStation = "Station B",
            DepartureTime = DateTime.Now.AddHours(1),
            ArrivalTime = DateTime.Now.AddHours(5),
            TotalSeats = 100,
            TrainClass = new List<TrainClassDTO> { new TrainClassDTO { ClassName = "First Class", TotalSeat = 50, Fare = 3000 } }
        };
        _mockTrainBL.Setup(bl => bl.AddTrainAsync(trainDTO)).ReturnsAsync(true);

        // Act
        var result = await _controller.AddTrain(trainDTO) as OkObjectResult;
        var responseModel = result.Value as ResponseModel<string>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(responseModel.Success, Is.True);
        Assert.That(responseModel.Message, Is.EqualTo("Train added successfully"));
        Assert.That(responseModel.Data, Is.EqualTo("Train Added Successfully"));
    }

    [Test]
    public async Task AddTrain_NotValid_ResultBadRequest()
    {
        // Arrange
        var trainDTO = new TrainDTO
        {
            TrainName = "Express",
            SourceStation = "Station A",
            DestinationStation = "Station B",
            DepartureTime = DateTime.Now.AddHours(1),
            ArrivalTime = DateTime.Now.AddHours(5),
            TotalSeats = 100,
            TrainClass = new List<TrainClassDTO> { new TrainClassDTO { ClassName = "First Class", TotalSeat = 50, Fare = 3000 } }
        };
        _mockTrainBL.Setup(bl => bl.AddTrainAsync(trainDTO)).ReturnsAsync(false);

        // Act
        var result = await _controller.AddTrain(trainDTO);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task AddTrain_ThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var trainDTO = new TrainDTO
        {
            TrainName = "Express",
            SourceStation = "Station A",
            DestinationStation = "Station B",
            DepartureTime = DateTime.Now.AddHours(1),
            ArrivalTime = DateTime.Now.AddHours(5),
            TotalSeats = 100,
            TrainClass = new List<TrainClassDTO> { new TrainClassDTO { ClassName = "First Class", TotalSeat = 50, Fare = 3000 } }
        };
        _mockTrainBL.Setup(bl => bl.AddTrainAsync(trainDTO))
            .ThrowsAsync(new InvalidOperationException("Error adding train"));

        // Act
        var result = await _controller.AddTrain(trainDTO) as BadRequestObjectResult;
        var responseModel = result.Value as ResponseModel<string>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(400));
        Assert.That(responseModel.Success, Is.False);
        Assert.That(responseModel.Message, Is.EqualTo("Error adding train"));
    }

    [Test]
    public async Task UpdateTrain_ValidData_ReturnsOkResult()
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
            TrainClass = new List<TrainClassDTO> { new TrainClassDTO { ClassName = "First Class", TotalSeat = 60, Fare = 3500 } }
        };
        _mockTrainBL.Setup(bl => bl.UpdateTrainAsync(trainId, trainDTO)).ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateTrain(trainId, trainDTO) as OkObjectResult;
        var responseModel = result.Value as ResponseModel<string>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(responseModel.Success, Is.True);
        Assert.That(responseModel.Message, Is.EqualTo("Train update successfully"));
        Assert.That(responseModel.Data, Is.EqualTo("Train update Successfully"));
    }

    [Test]
    public async Task UpdateTrain_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        int trainId = 999; // Non-existent train ID
        var trainDTO = new TrainDTO
        {
            TrainName = "Express Updated",
            SourceStation = "Station C",
            DestinationStation = "Station D",
            DepartureTime = DateTime.Now.AddHours(2),
            ArrivalTime = DateTime.Now.AddHours(6),
            TotalSeats = 120,
            TrainClass = new List<TrainClassDTO> { new TrainClassDTO { ClassName = "First Class", TotalSeat = 60, Fare = 3500 } }
        };
        _mockTrainBL.Setup(bl => bl.UpdateTrainAsync(trainId, trainDTO)).ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateTrain(trainId, trainDTO) as BadRequestObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task GetTrainAsync_HasTrains_ReturnsOkResult()
    {
        // Arrange
        var trains = new List<TrainResponseDTO>
        {
            new TrainResponseDTO
            {
                TrainID = 1,
                TrainName = "Express",
                SourceStation = "Station A",
                DestinationStation = "Station B",
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(5),
                TotalSeats = 100,
                TrainClasses = new List<TrainClassResponseDTO>
                {
                    new TrainClassResponseDTO
                    {
                        ClassId = 1,
                        ClassName = "First Class",
                        TotalSeat = 50,
                        Fare = 3000
                    }
                }
            }
        };
        _mockTrainBL.Setup(bl => bl.GetAllTrainsAsync()).ReturnsAsync(trains);

        // Act
        var result = await _controller.GetTrainAsync() as OkObjectResult;
        var responseModel = result.Value as ResponseModel<List<TrainResponseDTO>>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(responseModel.Success, Is.True);
        Assert.That(responseModel.Message, Is.EqualTo("Train details are as follows:"));
        Assert.That(responseModel.Data, Is.EqualTo(trains));
    }

    [Test]
    public async Task GetTrainAsync_NoTrains_ReturnsNotFound()
    {
        // Arrange
        var trains = new List<TrainResponseDTO>();
        _mockTrainBL.Setup(bl => bl.GetAllTrainsAsync()).ReturnsAsync(trains);

        // Act
        var result = await _controller.GetTrainAsync() as NotFoundObjectResult;
        var responseModel = result.Value as ResponseModel<List<TrainResponseDTO>>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(404));
        Assert.That(responseModel.Success, Is.False);
        Assert.That(responseModel.Message, Is.EqualTo("No trains are available"));
        Assert.That(responseModel.Data, Is.Null);
    }

    [Test]
    public async Task GetTrainAsync_ThrowsException_ReturnsStatusCode500()
    {
        // Arrange
        _mockTrainBL.Setup(bl => bl.GetAllTrainsAsync())
            .ThrowsAsync(new Exception("Database connection error"));

        // Act
        var result = await _controller.GetTrainAsync() as ObjectResult;
        var responseModel = result.Value as ResponseModel<List<TrainResponseDTO>>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(500));
        Assert.That(responseModel.Success, Is.False);
        Assert.That(responseModel.Message, Does.Contain("An error occurred while fetching the data"));
    }

    [Test]
    public async Task GetReservationsByUserId_ValidId_ReturnsOkResult()
    {
        // Arrange
        int userId = 1;
        var reservations = new List<ReservationResponse>
        {
            new ReservationResponse
            {
                ReservationId = 1,
                TravelDate = DateTime.Now.AddDays(1),
                NoOfSeats = 2,
                PNRNumber = 123456,
                BookingStatus = "Confirmed",
                TotalFare = 6000,
                Passengers = new List<PassengerResponse>
                {
                    new PassengerResponse
                    {
                        PassengerId = 1,
                        Name = "John Doe",
                        Age = 30,
                        Gender = "Male"
                    }
                }
            }
        };
        _mockTrainBL.Setup(bl => bl.GetReservationsByUserIdAsync(userId)).ReturnsAsync(reservations);

        // Act
        var actionResult = await _controller.GetReservationsByUserId(userId);
        var result = actionResult.Result as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Value, Is.EqualTo(reservations));
    }

    [Test]
    public async Task GetReservationsByUserId_ThrowsException_ReturnsStatusCode500()
    {
        // Arrange
        int userId = 999;
        _mockTrainBL.Setup(bl => bl.GetReservationsByUserIdAsync(userId))
            .ThrowsAsync(new Exception("User not found"));

        // Act
        var actionResult = await _controller.GetReservationsByUserId(userId);
        var result = actionResult.Result as ObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(500));
        Assert.That(result.Value.ToString(), Is.EqualTo("User not found"));
    }

    [Test]
    public async Task Search_HasTrains_ReturnsOkResult()
    {
        // Arrange
        string source = "Station A";
        string destination = "Station B";
        var trains = new List<TrainResponseDTO>
        {
            new TrainResponseDTO
            {
                TrainID = 1,
                TrainName = "Express",
                SourceStation = source,
                DestinationStation = destination,
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(5),
                TotalSeats = 100,
                TrainClasses = new List<TrainClassResponseDTO>
                {
                    new TrainClassResponseDTO
                    {
                        ClassId = 1,
                        ClassName = "First Class",
                        TotalSeat = 50,
                        Fare = 3000
                    }
                }
            }
        };
        _mockTrainBL.Setup(bl => bl.SearchTrainsAsync(source, destination)).ReturnsAsync(trains);

        // Act
        var result = await _controller.Search(source, destination) as OkObjectResult;
        var responseModel = result.Value as ResponseModel<List<TrainResponseDTO>>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(responseModel.Success, Is.True);
        Assert.That(responseModel.Message, Is.EqualTo("Train details are as follows:"));
        Assert.That(responseModel.Data, Is.EqualTo(trains));
    }

    [Test]
    public async Task Search_NoTrains_ReturnsNotFound()
    {
        // Arrange
        string source = "Station X";
        string destination = "Station Y";
        var trains = new List<TrainResponseDTO>();
        _mockTrainBL.Setup(bl => bl.SearchTrainsAsync(source, destination)).ReturnsAsync(trains);

        // Act
        var result = await _controller.Search(source, destination) as NotFoundObjectResult;
        var responseModel = result.Value as ResponseModel<List<TrainResponseDTO>>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(404));
        Assert.That(responseModel.Success, Is.False);
        Assert.That(responseModel.Message, Is.EqualTo("No trains are available"));
        Assert.That(responseModel.Data, Is.Null);
    }

    [Test]
    public async Task AddReservation_ValidData_ReturnsOkResult()
    {
        // Arrange
        var reservationDto = new ReservationDTO
        {
            UserID = 1,
            TrainId = 1,
            ClassId = 1,
            TravelDate = DateTime.Now.AddDays(1),
            NoOfSeats = 2,
            Quota = "General",
            BankName = "Test Bank",
            Passenger = new List<PassengerDTO>
            {
                new PassengerDTO
                {
                    Name = "John Doe",
                    Age = 30,
                    Gender = "Male"
                }
            }
        };
        string confirmationId = "RES12345";
        _mockTrainBL.Setup(bl => bl.AddReservationAsync(reservationDto)).ReturnsAsync(confirmationId);

        // Act
        var result = await _controller.AddReservation(reservationDto) as OkObjectResult;
        var responseModel = result.Value as ResponseModel<string>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(responseModel.Success, Is.True);
        Assert.That(responseModel.Message, Is.EqualTo("Reservation added successfully."));
        Assert.That(responseModel.Data, Is.EqualTo(confirmationId));
    }

    [Test]
    public async Task AddReservation_TrainNotFound_ReturnsNotFound()
    {
        // Arrange
        var reservationDto = new ReservationDTO
        {
            UserID = 1,
            TrainId = 999, // Non-existent train ID
            ClassId = 1,
            TravelDate = DateTime.Now.AddDays(1),
            NoOfSeats = 2,
            Quota = "General",
            BankName = "Test Bank",
            Passenger = new List<PassengerDTO>
            {
                new PassengerDTO
                {
                    Name = "John Doe",
                    Age = 30,
                    Gender = "Male"
                }
            }
        };
        _mockTrainBL.Setup(bl => bl.AddReservationAsync(reservationDto))
            .ThrowsAsync(new KeyNotFoundException("Train not found"));

        // Act
        var result = await _controller.AddReservation(reservationDto) as NotFoundObjectResult;
        var responseModel = result.Value as ResponseModel<string>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(404));
        Assert.That(responseModel.Success, Is.False);
        Assert.That(responseModel.Message, Is.EqualTo("Train not found"));
    }

    [Test]
    public async Task CancelReservation_ValidData_ReturnsOkResult()
    {
        // Arrange
        var cancellationDto = new CancellationDTO
        {
            PNRNumber = 123456,
            Reason = "Change of plans",
            ReservationId = 1
        };
        _mockTrainBL.Setup(bl => bl.CancelReservationAsync(cancellationDto)).ReturnsAsync(true);

        // Act
        var result = await _controller.CancelReservation(cancellationDto) as OkObjectResult;
        var responseModel = result.Value as ResponseModel<string>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(responseModel.Success, Is.True);
        Assert.That(responseModel.Message, Is.EqualTo("Reservation cancelled successfully."));
        Assert.That(responseModel.Data, Is.EqualTo("Reservation cancelled successfully."));
    }

    [Test]
    public async Task CancelReservation_InvalidData_ReturnsNotFound()
    {
        // Arrange
        var cancellationDto = new CancellationDTO
        {
            PNRNumber = 999999, // Non-existent PNR
            Reason = "Change of plans",
            ReservationId = 999 // Non-existent reservation ID
        };
        _mockTrainBL.Setup(bl => bl.CancelReservationAsync(cancellationDto)).ReturnsAsync(false);

        // Act
        var result = await _controller.CancelReservation(cancellationDto) as NotFoundObjectResult;
        var responseModel = result.Value as ResponseModel<string>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(404));
        Assert.That(responseModel.Success, Is.False);
        Assert.That(responseModel.Message, Is.EqualTo("Reservation not found."));
        Assert.That(responseModel.Data, Is.EqualTo("Reservation not found."));
    }
}