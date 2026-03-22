using NUnit.Framework;
using Moq;
using BusinessLayer.Service;
using ModelLayer.Models;
using Microsoft.Extensions.Configuration;

[TestFixture]
public class EmailServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private EmailService _emailService;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["SMTP:Host"]).Returns("smtp.test.com");
        _mockConfiguration.Setup(c => c["SMTP:Port"]).Returns("587");
        _mockConfiguration.Setup(c => c["SMTP:Username"]).Returns("test@example.com");
        _mockConfiguration.Setup(c => c["SMTP:Password"]).Returns("testpassword");
        
        _emailService = new EmailService(_mockConfiguration.Object);
    }

    [Test]
    public void SendEmail_ValidEmailModel_ThrowsTimeoutException()
    {
        // Arrange
        var emailModel = new EmailModel
        {
            To = "test@example.com",
            Subject = "Test Subject",
            Body = "Test Body"
        };

        // Act & Assert
        Assert.Throws<Exception>(() => _emailService.SendEmail(emailModel));
    }

    [Test]
    public void SendEmail_NullEmailModel_ThrowsException()
    {
        // Arrange
        EmailModel emailModel = null;

        // Act & Assert
        Assert.Throws<Exception>(() => _emailService.SendEmail(emailModel));
    }

    [Test]
    public void SendEmail_EmptyToAddress_ThrowsException()
    {
        // Arrange
        var emailModel = new EmailModel
        {
            To = "",
            Subject = "Test Subject",
            Body = "Test Body"
        };

        // Act & Assert
        Assert.Throws<Exception>(() => _emailService.SendEmail(emailModel));
    }

    [Test]
    public void SendEmail_InvalidEmailFormat_ThrowsException()
    {
        // Arrange
        var emailModel = new EmailModel
        {
            To = "invalid-email",
            Subject = "Test Subject",
            Body = "Test Body"
        };

        // Act & Assert
        Assert.Throws<Exception>(() => _emailService.SendEmail(emailModel));
    }
}