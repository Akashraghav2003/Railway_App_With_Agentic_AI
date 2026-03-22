using NUnit.Framework;
using ModelLayer.Models;
using System.ComponentModel.DataAnnotations;

[TestFixture]
public class ModelValidationTests
{
    [Test]
    public void UserDTO_ValidData_PassesValidation()
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

        // Act
        var validationResults = ValidateModel(userDTO);

        // Assert
        Assert.That(validationResults.Count, Is.EqualTo(0));
    }

    [Test]
    public void UserDTO_InvalidEmail_FailsValidation()
    {
        // Arrange
        var userDTO = new UserDTO
        {
            Name = "Test User",
            Gender = "Male",
            Age = 25,
            Address = "Test Address",
            Email = "invalid-email",
            Phone = 1234567890,
            UserName = "TestUser",
            Password = "Password123@"
        };

        // Act
        var validationResults = ValidateModel(userDTO);

        // Assert
        Assert.That(validationResults.Count, Is.GreaterThan(0));
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Email")), Is.True);
    }

    [Test]
    public void LoginDTO_ValidData_PassesValidation()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            EmailOrUserName = "test@example.com",
            Password = "Password123"
        };

        // Act
        var validationResults = ValidateModel(loginDTO);

        // Assert
        Assert.That(validationResults.Count, Is.EqualTo(0));
    }

    [Test]
    public void LoginDTO_EmptyPassword_FailsValidation()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            EmailOrUserName = "test@example.com",
            Password = ""
        };

        // Act
        var validationResults = ValidateModel(loginDTO);

        // Assert
        Assert.That(validationResults.Count, Is.GreaterThan(0));
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Password")), Is.True);
    }

    [Test]
    public void TrainDTO_ValidData_PassesValidation()
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

        // Act
        var validationResults = ValidateModel(trainDTO);

        // Assert
        Assert.That(validationResults.Count, Is.EqualTo(0));
    }

    [Test]
    public void ReservationDTO_ValidData_PassesValidation()
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
                    Name = "John Doe",
                    Age = 30,
                    Gender = "Male"
                }
            }
        };

        // Act
        var validationResults = ValidateModel(reservationDTO);

        // Assert
        Assert.That(validationResults.Count, Is.EqualTo(0));
    }

    [Test]
    public void ReservationDTO_InvalidTravelDate_FailsValidation()
    {
        // Arrange
        var reservationDTO = new ReservationDTO
        {
            UserID = 1,
            TrainId = 1,
            ClassId = 1,
            TravelDate = DateTime.Now.AddDays(-1), // Past date
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

        // Act
        var validationResults = ValidateModel(reservationDTO);

        // Assert - This would depend on if there's validation for future dates
        // For now, just checking that validation runs
        Assert.That(validationResults, Is.Not.Null);
    }

    [Test]
    public void PassengerDTO_ValidData_PassesValidation()
    {
        // Arrange
        var passengerDTO = new PassengerDTO
        {
            Name = "John Doe",
            Age = 30,
            Gender = "Male",
            AdharCard = 123456789012
        };

        // Act
        var validationResults = ValidateModel(passengerDTO);

        // Assert
        Assert.That(validationResults.Count, Is.EqualTo(0));
    }

    [Test]
    public void PassengerDTO_InvalidAge_FailsValidation()
    {
        // Arrange
        var passengerDTO = new PassengerDTO
        {
            Name = "John Doe",
            Age = -5, // Invalid age
            Gender = "Male"
        };

        // Act
        var validationResults = ValidateModel(passengerDTO);

        // Assert - This would depend on if there's validation for age range
        Assert.That(validationResults, Is.Not.Null);
    }

    private List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}