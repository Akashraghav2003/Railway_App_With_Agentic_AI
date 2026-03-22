using NUnit.Framework;
using BusinessLayer.Service;
using RepositoryLayer.Entity;
using Microsoft.Extensions.Configuration;
using Moq;

[TestFixture]
public class JWTTokenTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private JWTToken _jwtToken;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyForJWTTokenGeneration123456");
        _mockConfiguration.Setup(c => c["JWT:Key"]).Returns("ThisIsASecretKeyForJWTTokenGeneration123456");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _mockConfiguration.Setup(c => c["JWT:Issuer"]).Returns("TestIssuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
        _mockConfiguration.Setup(c => c["JWT:Audience"]).Returns("TestAudience");
        _mockConfiguration.Setup(c => c["Jwt:ExpireMinutes"]).Returns("60");
        
        _jwtToken = new JWTToken(_mockConfiguration.Object);
    }

    [Test]
    public void GenerateToken_ValidUser_ReturnsToken()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Name = "Test User",
            Email = "test@example.com",
            UserName = "testuser",
            Role = "User"
        };

        // Act
        var token = _jwtToken.GenerateToken(user);

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);
        Assert.That(token.Split('.').Length, Is.EqualTo(3)); // JWT has 3 parts
    }

    [Test]
    public void GenerateToken_NullUser_ThrowsNullReferenceException()
    {
        // Arrange
        User user = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => _jwtToken.GenerateToken(user));
    }

    [Test]
    public void ValidateToken_ValidToken_ReturnsEmail()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Name = "Test User",
            Email = "test@example.com",
            UserName = "testuser",
            Role = "User"
        };

        _mockConfiguration.Setup(c => c["Jwt:ExpireMinutes"]).Returns("60");
        var token = _jwtToken.GenerateToken(user);

        // Act
        var email = _jwtToken.ValidateToken(token);

        // Assert
        Assert.That(email, Is.EqualTo(user.Email));
    }

    [Test]
    public void ValidateToken_InvalidToken_ThrowsException()
    {
        // Arrange
        string invalidToken = "invalid.token.here";

        // Act & Assert
        Assert.Throws<Exception>(() => _jwtToken.ValidateToken(invalidToken));
    }

    [Test]
    public void ValidateToken_EmptyToken_ThrowsException()
    {
        // Arrange
        string emptyToken = "";

        // Act & Assert
        Assert.Throws<Exception>(() => _jwtToken.ValidateToken(emptyToken));
    }

    [Test]
    public void ValidateToken_NullToken_ThrowsException()
    {
        // Arrange
        string nullToken = null;

        // Act & Assert
        Assert.Throws<Exception>(() => _jwtToken.ValidateToken(nullToken));
    }
}