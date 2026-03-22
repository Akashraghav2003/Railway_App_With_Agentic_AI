using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using BusinessLayer.Interface;
using Microsoft.Extensions.Logging;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service;

public class RailwayBusinessLayer : IRailwayBusinessLayer
{
    private readonly IRailwayRL _railwayRL;
    private readonly ILogger<RailwayBusinessLayer> _logger;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly IJwtToken _jWTToken;

    public RailwayBusinessLayer(IRailwayRL railwayRL, ILogger<RailwayBusinessLayer> logger, IEmailService emailService, IMapper mapper, IJwtToken jWTToken)
    {
        _railwayRL = railwayRL;
        _logger = logger;
        _emailService = emailService;
        _mapper = mapper;
        _jWTToken = jWTToken;
    }

    

    public async Task<bool> ForgetPassword(string Email)
    {
        try
        {
            var user = await _railwayRL.ForgetPassword(Email);
            if (user == null)
            {
                throw new InvalidOperationException("Email not mached.");
            }
            string token = _jWTToken.GenerateToken(user);

            var emailModel = new EmailModel
            {
                To = Email,
                Subject = "Token for reset password",
                Body = $"Here is your token = {token}"
            };
            _emailService.SendEmail(emailModel);
            return true;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex.Message);
            throw; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw; 
        }
    }

    public async Task<string> LogIn(LoginDTO loginDTO)
    {
        try
        {
            var user = await _railwayRL.LogIn(loginDTO);
            if (user == null)
            {
                throw new InvalidOperationException("Email or username is not correct.");
            }

            bool result = BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password);
            if (!result)
            {
                throw new ArgumentException("Enter the correct password.");
            }

            string token = _jWTToken.GenerateToken(user);
            return token;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<User> RegisterUserAsync(UserDTO userDto)
    {
        try
        {
          
            var user = await _railwayRL.GetUserAsync(userDto);
            if (user != null)
            {
                throw new InvalidOperationException("User already registered.");
            }

            userDto.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);


            var newUser = _mapper.Map<User>(userDto);
            newUser.Role = "User";

         
            var result = await _railwayRL.RegisterUserAsync(newUser);

           
            var emailModel = new EmailModel
            {
                To = userDto.Email,
                Subject = "Registration Confirmation",
                Body = $"Dear {result.UserName},\n\nYour registration is successful. Your User ID is {result.UserId}.\n\nThank you!"
            };
            _emailService.SendEmail(emailModel);

            return result;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex.Message);
            throw; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw; 
        }
    }

    public async Task<bool> ResetPassword(ResetModel resetModel)
    {
        try
        {
            var email = _jWTToken.ValidateToken(resetModel.Token);

            if (resetModel.NewPassword == resetModel.ConfirmPassword && email != null)
            {
                string newPassword = BCrypt.Net.BCrypt.HashPassword(resetModel.NewPassword);
                var user = await _railwayRL.ResetPassword(email, newPassword);
                return true;
            }
            else
            {
                throw new InvalidOperationException("Password do not match or email not found.");
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _railwayRL.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            return user;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<List<ReservationResponse>> GetAllReservationsByUserIdAsync(int userId)
{
    try
    {
        var reservations = await _railwayRL.GetAllReservationsByUserIdAsync(userId);

        
        var reservationResponses = _mapper.Map<List<ReservationResponse>>(reservations);

        return reservationResponses;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching reservations for user ID: {UserId}", userId);
        throw;
    }
}

    public async Task<bool> GetAllReservationsByUserId(int userId)
    {
        throw new NotImplementedException();
    }
}
