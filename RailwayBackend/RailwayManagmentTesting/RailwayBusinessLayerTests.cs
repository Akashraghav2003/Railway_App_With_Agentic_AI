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
public class RailwayBusinessLayerTests
{
    private Mock<IRailwayRL> _mockRailwayRL;
    private Mock<ILogger<RailwayBusinessLayer>> _mockLogger;
    private Mock<IEmailService> _mockEmailService;
    private Mock<IMapper> _mockMapper;
    private Mock<IJwtToken> _mockJWTToken;
    private RailwayBusinessLayer _businessLayer;

    [SetUp]
    public void Setup()
    {
        _mockRailwayRL = new Mock<IRailwayRL>();
        _mockLogger = new Mock<ILogger<RailwayBusinessLayer>>();
        _mockEmailService = new Mock<IEmailService>();
        _mockMapper = new Mock<IMapper>();
        _mockJWTToken = new Mock<IJwtToken>();
        _businessLayer = new RailwayBusinessLayer(
            _mockRailwayRL.Object,
            _mockLogger.Object,
            _mockEmailService.Object,
            _mockMapper.Object,
            _mockJWTToken.Object
        );
    }

    [Test]
    public async Task RegisterUserAsync_ValidUser_ReturnsUser()
    {
        // Arrange
        var userDto = new UserDTO
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

        _mockRailwayRL.Setup(rl => rl.GetUserAsync(userDto)).ReturnsAsync((User)null);
        _mockMapper.Setup(m => m.Map<User>(userDto)).Returns(user);
        _mockRailwayRL.Setup(rl => rl.RegisterUserAsync(user)).ReturnsAsync(user);

        // Act
        var result = await _businessLayer.RegisterUserAsync(userDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserName, Is.EqualTo(userDto.UserName));
        Assert.That(result.Email, Is.EqualTo(userDto.Email));
        Assert.That(result.Role, Is.EqualTo("User"));
        
        // Verify email was sent
        _mockEmailService.Verify(e => e.SendEmail(It.IsAny<EmailModel>()), Times.Once);
    }

    [Test]
    public void RegisterUserAsync_UserAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var userDto = new UserDTO
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

        var existingUser = new User
        {
            UserId = 1,
            Name = "Existing User",
            Gender = "Female",
            Age = 30,
            Address = "Existing Address",
            Email = "existing@example.com",
            Phone = 9876543210,
            UserName = "ExistingUser",
            Password = "hashedPassword",
            Role = "User"
        };

        _mockRailwayRL.Setup(rl => rl.GetUserAsync(userDto)).ReturnsAsync(existingUser);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _businessLayer.RegisterUserAsync(userDto));
        Assert.That(ex.Message, Is.EqualTo("User already registered."));
    }

    [Test]
    public async Task LogIn_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
             EmailOrUserName= "test@example.com",
            Password = "Password123"
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
            Password = BCrypt.Net.BCrypt.HashPassword("Password123"),
            Role = "User"
        };

        string token = "test-jwt-token";

        _mockRailwayRL.Setup(rl => rl.LogIn(loginDto)).ReturnsAsync(user);
        _mockJWTToken.Setup(j => j.GenerateToken(user)).Returns(token);

        // Act
        var result = await _businessLayer.LogIn(loginDto);

        // Assert
        Assert.That(result, Is.EqualTo(token));
    }

    [Test]
    public void LogIn_InvalidEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
            EmailOrUserName = "invalid@example.com",
            Password = "Password123"
        };

        _mockRailwayRL.Setup(rl => rl.LogIn(loginDto)).ReturnsAsync((User)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _businessLayer.LogIn(loginDto));
        Assert.That(ex.Message, Is.EqualTo("Email or username is not correct."));
    }

    [Test]
    public void LogIn_InvalidPassword_ThrowsArgumentException()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
            EmailOrUserName = "test@example.com",
            Password = "WrongPassword"
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
            Password = BCrypt.Net.BCrypt.HashPassword("Password123"), // Different password
            Role = "User"
        };

        _mockRailwayRL.Setup(rl => rl.LogIn(loginDto)).ReturnsAsync(user);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () => 
            await _businessLayer.LogIn(loginDto));
        Assert.That(ex.Message, Is.EqualTo("Enter the correct password."));
    }

    [Test]
    public async Task ForgetPassword_ValidEmail_ReturnsTrue()
    {
        // Arrange
        string email = "test@example.com";
        var user = new User
        {
            UserId = 1,
            Name = "Test User",
            Gender = "Male",
            Age = 25,
            Address = "Test Address",
            Email = email,
            Phone = 1234567890,
            UserName = "TestUser",
            Password = "hashedPassword",
            Role = "User"
        };
        string token = "reset-token";

        _mockRailwayRL.Setup(rl => rl.ForgetPassword(email)).ReturnsAsync(user);
        _mockJWTToken.Setup(j => j.GenerateToken(user)).Returns(token);

        // Act
        var result = await _businessLayer.ForgetPassword(email);

        // Assert
        Assert.That(result, Is.True);
        
        // Verify email was sent
        _mockEmailService.Verify(e => e.SendEmail(It.Is<EmailModel>(em => 
            em.To == email && 
            em.Subject == "Token for reset password" && 
            em.Body.Contains(token))), Times.Once);
    }

    [Test]
    public void ForgetPassword_InvalidEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        string email = "invalid@example.com";
        _mockRailwayRL.Setup(rl => rl.ForgetPassword(email)).ReturnsAsync((User)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _businessLayer.ForgetPassword(email));
        Assert.That(ex.Message, Is.EqualTo("Email not mached."));
    }

   [Test]
public async Task ResetPassword_ValidData_ReturnsTrue()
{
    // Arrange
    var resetModel = new ResetModel
    {
        Token = "valid-token",
        NewPassword = "NewPassword123@",
        ConfirmPassword = "NewPassword123@"
    };
    string email = "test@example.com";

    _mockJWTToken.Setup(j => j.ValidateToken(resetModel.Token)).Returns(email);
    var user = new User { Email = email };
    _mockRailwayRL.Setup(rl => rl.ResetPassword(email, It.IsAny<string>())).ReturnsAsync(user);

    // Act
    var result = await _businessLayer.ResetPassword(resetModel);

    // Assert
    Assert.That(result, Is.True);

    // Verify
    _mockRailwayRL.Verify(rl => rl.ResetPassword(email, It.IsAny<string>()), Times.Once);
}


    [Test]
    public void ResetPassword_PasswordMismatch_ThrowsInvalidOperationException()
    {
        // Arrange
        var resetModel = new ResetModel
        {
            Token = "valid-token",
            NewPassword = "NewPassword123",
            ConfirmPassword = "DifferentPassword"
        };
        string email = "test@example.com";

        _mockJWTToken.Setup(j => j.ValidateToken(resetModel.Token)).Returns(email);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _businessLayer.ResetPassword(resetModel));
        Assert.That(ex.Message, Is.EqualTo("Password do not match or email not found."));
    }

    [Test]
    public void ResetPassword_InvalidToken_ThrowsInvalidOperationException()
    {
        // Arrange
        var resetModel = new ResetModel
        {
            Token = "invalid-token",
            NewPassword = "NewPassword123",
            ConfirmPassword = "NewPassword123"
        };

        _mockJWTToken.Setup(j => j.ValidateToken(resetModel.Token)).Returns((string)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _businessLayer.ResetPassword(resetModel));
        Assert.That(ex.Message, Is.EqualTo("Password do not match or email not found."));
    }

    [Test]
    public async Task GetUserByIdAsync_ValidId_ReturnsUser()
    {
        // Arrange
        int userId = 1;
        var user = new User { UserId = userId, Name = "Test User", Email = "test@example.com" };
        _mockRailwayRL.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _businessLayer.GetUserByIdAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(userId));
        _mockRailwayRL.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task GetAllReservationsByUserIdAsync_ValidUserId_ReturnsReservations()
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
        _mockRailwayRL.Setup(r => r.GetAllReservationsByUserIdAsync(userId)).ReturnsAsync(reservations);
        _mockMapper.Setup(m => m.Map<List<ReservationResponse>>(reservations)).Returns(reservationResponses);

        // Act
        var result = await _businessLayer.GetAllReservationsByUserIdAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
        _mockRailwayRL.Verify(r => r.GetAllReservationsByUserIdAsync(userId), Times.Once);
    }
}