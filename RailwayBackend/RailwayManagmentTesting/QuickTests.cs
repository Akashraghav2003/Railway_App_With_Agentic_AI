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
public class QuickTests
{
    [Test]
    public void SimpleTest_Always_Passes()
    {
        // Arrange
        int expected = 2;
        
        // Act
        int actual = 1 + 1;
        
        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void UserDTO_Creation_Works()
    {
        // Arrange & Act
        var userDTO = new UserDTO
        {
            Name = "Test User",
            Email = "test@example.com",
            UserName = "testuser"
        };

        // Assert
        Assert.That(userDTO.Name, Is.EqualTo("Test User"));
        Assert.That(userDTO.Email, Is.EqualTo("test@example.com"));
        Assert.That(userDTO.UserName, Is.EqualTo("testuser"));
    }

    [Test]
    public void TrainDTO_Creation_Works()
    {
        // Arrange & Act
        var trainDTO = new TrainDTO
        {
            TrainName = "Express",
            SourceStation = "Station A",
            DestinationStation = "Station B",
            TotalSeats = 100
        };

        // Assert
        Assert.That(trainDTO.TrainName, Is.EqualTo("Express"));
        Assert.That(trainDTO.TotalSeats, Is.EqualTo(100));
    }

    [Test]
    public void Mock_Framework_Works()
    {
        // Arrange
        var mockRepository = new Mock<IRailwayRL>();
        mockRepository.Setup(r => r.GetUserAsync(It.IsAny<UserDTO>())).ReturnsAsync((User)null);

        // Act
        var result = mockRepository.Object.GetUserAsync(new UserDTO()).Result;

        // Assert
        Assert.That(result, Is.Null);
        mockRepository.Verify(r => r.GetUserAsync(It.IsAny<UserDTO>()), Times.Once);
    }
}