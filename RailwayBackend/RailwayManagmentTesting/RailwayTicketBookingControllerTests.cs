using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RailwayManagmentSystem.Controllers;
using BusinessLayer.Interface;
using ModelLayer.Models;
using RepositoryLayer.Entity;

[TestFixture]
public class RailwayTicketBookingControllerTests
{
    private Mock<IRailwayBusinessLayer> _mockRailwayBL;
    private Mock<ILogger<RailwayTicketBooking>> _mockLogger;
    private RailwayTicketBooking _controller;

    [SetUp]
    public void Setup()
    {
        _mockRailwayBL = new Mock<IRailwayBusinessLayer>();
        _mockLogger = new Mock<ILogger<RailwayTicketBooking>>();
        _controller = new RailwayTicketBooking(_mockRailwayBL.Object, _mockLogger.Object);
    }

    [Test]
    public async Task UserRegister_ValidUser_ReturnsOkResult()
    {
        // Arrange
        var userDTO = new UserDTO
        {
            Name = "Test User",
            Gender = "Male",
            Age = 25,
            Address = "Test Address",
            Email = "test@example.com",
            Phone = 1234567890,
            UserName = "TestUser",
            Password = "Password123@"
        };

        var user = new User
        {
            UserId = 1,
            Name = "Test User",
            Gender = "Male",
            Age = 25,
            Address = "Test Address",
            Email = "test@example.com",
            Phone = 1234567890,
            UserName = "TestUser",
            Password = "hashedPassword",
            Role = "User"
        };

        _mockRailwayBL.Setup(bl => bl.RegisterUserAsync(userDTO)).ReturnsAsync(user);

        // Act
        var result = await _controller.UserRegister(userDTO) as OkObjectResult;
        var responseModel = result.Value as ResponseModel<string>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(responseModel.Success, Is.True);
        Assert.That(responseModel.Message, Is.EqualTo("User register successfully"));
        Assert.That(responseModel.Data, Is.EqualTo($"User Id for the {user.UserName} is {user.UserId}"));
    }

    [Test]
    public async Task UserRegister_NullUser_ReturnsBadRequest()
    {
        // Arrange
        UserDTO userDTO = null;

        // Act
        var result = await _controller.UserRegister(userDTO);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UserRegister_UserAlreadyExists_ReturnsStatusCode500()
    {
        // Arrange
        var userDTO = new UserDTO
        {
            Name = "Existing User",
            Gender = "Female",
            Age = 30,
            Address = "Existing Address",
            Email = "existing@example.com",
            Phone = 9876543210,
            UserName = "ExistingUser",
            Password = "Password123@"
        };

        _mockRailwayBL.Setup(bl => bl.RegisterUserAsync(userDTO))
            .ThrowsAsync(new InvalidOperationException("User already registered."));

        // Act
        var result = await _controller.UserRegister(userDTO) as ObjectResult;
        var responseModel = result.Value as ResponseModel<string>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(500));
        Assert.That(responseModel.Success, Is.False);
        Assert.That(responseModel.Message, Is.EqualTo("User does not register."));
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            EmailOrUserName = "test@example.com",
            Password = "Password123"
        };

        string token = "test-jwt-token";
        _mockRailwayBL.Setup(bl => bl.LogIn(loginDTO)).ReturnsAsync(token);

        // Act
        var result = await _controller.Login(loginDTO) as OkObjectResult;
        var responseModel = result.Value as ResponseModel<string>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(responseModel.Success, Is.True);
        Assert.That(responseModel.Message, Is.EqualTo("Login successful"));
        Assert.That(responseModel.Data, Is.EqualTo(token));
    }

    [Test]
    public async Task Login_InvalidEmail_ReturnsUnauthorized()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            EmailOrUserName = "invalid@example.com",
            Password = "Password123"
        };

        _mockRailwayBL.Setup(bl => bl.LogIn(loginDTO))
            .ThrowsAsync(new InvalidOperationException("Email or username is not correct."));

        // Act
        var result = await _controller.Login(loginDTO) as UnauthorizedObjectResult;
        var responseModel = result.Value as ResponseModel<string>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(401));
        Assert.That(responseModel.Success, Is.False);
        Assert.That(responseModel.Message, Is.EqualTo("Email or username is not correct."));
    }

    [Test]
    public async Task Login_InvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            EmailOrUserName = "test@example.com",
            Password = "WrongPassword"
        };

        _mockRailwayBL.Setup(bl => bl.LogIn(loginDTO))
            .ThrowsAsync(new ArgumentException("Enter the correct password."));

        // Act
        var result = await _controller.Login(loginDTO) as BadRequestObjectResult;
        var responseModel = result.Value as ResponseModel<string>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(400));
        Assert.That(responseModel.Success, Is.False);
        Assert.That(responseModel.Message, Is.EqualTo("Enter the correct password."));
    }

    [Test]
    public async Task ForgetPassword_ValidEmail_ReturnsOkResult()
    {
        // Arrange
        string email = "test@example.com";
        _mockRailwayBL.Setup(bl => bl.ForgetPassword(email)).ReturnsAsync(true);

        // Act
        var result = await _controller.ForgetPassword(email) as OkObjectResult;
        dynamic response = result.Value;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(response.Message.ToString(), Is.EqualTo("Token sent to your email."));
    }


    [Test]
    public async Task ForgetPassword_InvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        string email = "invalid@example.com";
        _mockRailwayBL.Setup(bl => bl.ForgetPassword(email))
            .ThrowsAsync(new InvalidOperationException("Email not mached."));

        // Act
        var result = await _controller.ForgetPassword(email) as BadRequestObjectResult;
        dynamic response = result.Value;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(400));
        Assert.That(response.Message.ToString(), Is.EqualTo("Email not mached."));
    }

    [Test]
    public async Task ResetPassword_ValidData_ReturnsOkResult()
    {
        // Arrange
        var resetModel = new ResetModel
        {
            Token = "valid-token",
            NewPassword = "NewPassword123@",
            ConfirmPassword = "NewPassword123@"
        };

        _mockRailwayBL.Setup(bl => bl.ResetPassword(resetModel)).ReturnsAsync(true);

        // Act
        var result = await _controller.ResetPassword(resetModel) as OkObjectResult;
        var responseModel = result.Value as ResponseModel<bool>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(responseModel.Success, Is.True);
        Assert.That(responseModel.Message, Is.EqualTo("Password reset successfully."));
        Assert.That(responseModel.Data, Is.True);
    }


    [Test]
    public async Task ResetPassword_PasswordMismatch_ReturnsBadRequest()
    {
        // Arrange
        var resetModel = new ResetModel
        {
            Token = "valid-token",
            NewPassword = "NewPassword123",
            ConfirmPassword = "DifferentPassword"
        };

        _mockRailwayBL.Setup(bl => bl.ResetPassword(resetModel))
            .ThrowsAsync(new InvalidOperationException("Password do not match or email not found."));

        // Act
        var result = await _controller.ResetPassword(resetModel) as BadRequestObjectResult;
        var responseModel = result.Value as ResponseModel<bool>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(400));
        Assert.That(responseModel.Success, Is.False);
        Assert.That(responseModel.Message, Is.EqualTo("Password do not match or email not found."));
        Assert.That(responseModel.Data, Is.False);
    }
}