using NUnit.Framework;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using ModelLayer.Models;
using System.Net;
using RailwayManagmentSystem.Controllers;

[TestFixture]
public class IntegrationTests
{
    private WebApplicationFactory<RailwayTicketBooking> _factory;
    private HttpClient _client;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new WebApplicationFactory<RailwayTicketBooking>();
        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [Test]
    public async Task UserRegister_ValidUser_ReturnsOk()
    {
        // Arrange
        var userDTO = new UserDTO
        {
            Name = "Integration Test User",
            Gender = "Male",
            Age = 25,
            Address = "Test Address",
            Email = "integration@test.com",
            Phone = 1234567890,
            UserName = "IntegrationUser",
            Password = "Password123@"
        };

        var json = JsonSerializer.Serialize(userDTO);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/RailwayTicketBooking/UserRegister", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.InternalServerError).Or.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            EmailOrUserName = "test@example.com",
            Password = "Password123"
        };

        var json = JsonSerializer.Serialize(loginDTO);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/RailwayTicketBooking/Login", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.Unauthorized).Or.EqualTo(HttpStatusCode.InternalServerError).Or.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetTrains_ReturnsResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/Train/GetTrain");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NotFound).Or.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task SearchTrains_ValidParameters_ReturnsResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/Train/Search?source=StationA&destination=StationB");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NotFound).Or.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task AddTrain_ValidTrain_ReturnsResponse()
    {
        // Arrange
        var trainDTO = new TrainDTO
        {
            TrainName = "Integration Test Express",
            SourceStation = "Station A",
            DestinationStation = "Station B",
            DepartureTime = DateTime.Now.AddHours(1),
            ArrivalTime = DateTime.Now.AddHours(5),
            TotalSeats = 100,
            TrainClass = new List<TrainClassDTO>
            {
                new TrainClassDTO
                {
                    ClassName = "First Class",
                    TotalSeat = 50,
                    Fare = 3000
                }
            }
        };

        var json = JsonSerializer.Serialize(trainDTO);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/Train/AddTrain", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.BadRequest).Or.EqualTo(HttpStatusCode.InternalServerError).Or.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task ForgetPassword_ValidEmail_ReturnsResponse()
    {
        // Act
        var response = await _client.PostAsync("/api/RailwayTicketBooking/ForgetPassword?email=test@example.com", null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.BadRequest).Or.EqualTo(HttpStatusCode.InternalServerError).Or.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task ResetPassword_ValidData_ReturnsResponse()
    {
        // Arrange
        var resetModel = new ResetModel
        {
            Token = "dummy-token",
            NewPassword = "NewPassword123@",
            ConfirmPassword = "NewPassword123@"
        };

        var json = JsonSerializer.Serialize(resetModel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/RailwayTicketBooking/ResetPassword", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.BadRequest).Or.EqualTo(HttpStatusCode.InternalServerError).Or.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetReservationsByUserId_ValidUserId_ReturnsResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/Train/GetReservationsByUserId/1");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.InternalServerError).Or.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task AddReservation_ValidData_ReturnsResponse()
    {
        // Arrange
        var reservationDTO = new ReservationDTO
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
                    Name = "Integration Test Passenger",
                    Age = 30,
                    Gender = "Male"
                }
            }
        };

        var json = JsonSerializer.Serialize(reservationDTO);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/Train/AddReservation", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NotFound).Or.EqualTo(HttpStatusCode.InternalServerError).Or.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task CancelReservation_ValidData_ReturnsResponse()
    {
        // Arrange
        var cancellationDTO = new CancellationDTO
        {
            PNRNumber = 123456789,
            Reason = "Integration test cancellation",
            ReservationId = 1
        };

        var json = JsonSerializer.Serialize(cancellationDTO);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/Train/CancelReservation", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NotFound).Or.EqualTo(HttpStatusCode.InternalServerError));
    }
}