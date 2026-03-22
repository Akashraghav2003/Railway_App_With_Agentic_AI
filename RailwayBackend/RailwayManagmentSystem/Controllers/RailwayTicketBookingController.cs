using System.Data.Common;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using ModelLayer.Models;
using RepositoryLayer.Entity;


namespace RailwayManagmentSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class RailwayTicketBooking : ControllerBase
{
      private readonly IRailwayBusinessLayer _railwayBL;

      private readonly ILogger<RailwayTicketBooking> _logger;

      public RailwayTicketBooking(IRailwayBusinessLayer railwayBL, ILogger<RailwayTicketBooking> logger)
      {
            _railwayBL = railwayBL;
            _logger = logger;

      }



      [HttpPost("UserRegister")]

      public async Task<IActionResult> UserRegister(UserDTO userDTO)
      {
            ResponseModel<string> responseModel = new ResponseModel<string>();
            if (!ModelState.IsValid)
            {
                  return BadRequest(ModelState);
            }

            try
            {
                  _logger.LogInformation("Index page visited at {DT}", DateTime.UtcNow);

                  if (userDTO == null)
                  {
                        throw new ArgumentException("Give correct input.");
                  }
                  var result = await _railwayBL.RegisterUserAsync(userDTO);
                  responseModel.Success = true;
                  responseModel.Message = "User register successfully";
                  responseModel.Data = $"User Id for the {result.UserName} is {result.UserId}";

                  return Ok(responseModel);

            }
            catch (InvalidOperationException ex)
            {
                  responseModel.Success = false;
                  responseModel.Message = "User does not register.";
                  responseModel.Data = ex.Message;

                  _logger.LogError(ex.Message);

                  return StatusCode(500, responseModel);
            }
            catch (ArgumentException ex)
            {
                  _logger.LogError(ex.Message);

                  return BadRequest(ex.Message);
            }
      }



      [HttpPost("login")]
      public async Task<IActionResult> Login([FromBody] LoginDTO loginModel)
      {

            var responseModel = new ResponseModel<string>();
            try
            {
                  var token = await _railwayBL.LogIn(loginModel);
                  responseModel.Success = true;
                  responseModel.Message = "Login successful";
                  responseModel.Data = token;
                  return Ok(responseModel);
            }
            catch (InvalidOperationException ex)
            {
                  _logger.LogError(ex.Message);
                  responseModel.Success = false;
                  responseModel.Message = ex.Message;
                  return Unauthorized(responseModel);
            }
            catch (ArgumentException ex)
            {
                  _logger.LogError(ex.Message);
                  responseModel.Success = false;
                  responseModel.Message = ex.Message;
                  return BadRequest(responseModel);
            }

      }



      [HttpPost("ForgetPassword")]
      public async Task<IActionResult> ForgetPassword([FromQuery] string email)
      {
            try
            {
                  var result = await _railwayBL.ForgetPassword(email);
                  if (result)
                  {
                        return Ok(new ResponseModel<bool>
                        {
                              Success = true,
                              Message = "Token sent to your email.",
                              Data = true
                        });
                  }
                  else
                  {
                        return BadRequest(new ResponseModel<bool>
                        {
                              Success = false,
                              Message = "Failed to send token.",
                              Data = false
                        });
                  }
            }
            catch (InvalidOperationException ex)
            {
                  _logger.LogError(ex.Message);
                  return BadRequest(new ResponseModel<bool>
                  {
                        Success = false,
                        Message = ex.Message,
                        Data = false
                  });
            }
            catch (Exception ex)
            {
                  _logger.LogError(ex.Message);
                  return StatusCode(500, new ResponseModel<bool>
                  {
                        Success = false,
                        Message = "An error occurred while processing your request.",
                        Data = false
                  });
            }
      }


      [HttpPost("ResetPassword")]
      [Authorize]
      public async Task<IActionResult> ResetPassword([FromBody] ResetModel resetModel)
      {

            try
            {
                  var result = await _railwayBL.ResetPassword(resetModel);
                  if (result)
                  {
                        return Ok(new ResponseModel<bool>
                        {
                              Success = true,
                              Message = "Password reset successfully.",
                              Data = result
                        });
                  }
                  else
                  {
                        return BadRequest(new ResponseModel<bool>
                        {
                              Success = false,
                              Message = "Failed to reset password.",
                              Data = result
                        });
                  }
            }
            catch (InvalidOperationException ex)
            {
                  _logger.LogError(ex.Message);
                  return BadRequest(new ResponseModel<bool>
                  {
                        Success = false,
                        Message = ex.Message,
                        Data = false
                  });
            }
            catch (Exception ex)
            {
                  _logger.LogError(ex.Message);
                  return StatusCode(500, new ResponseModel<bool>
                  {
                        Success = false,
                        Message = $"{ex.Message}",
                        Data = false
                  });
            }
      }

      [HttpGet("GetUserDetailsById/{userId}")]
      [Authorize]
      public async Task<IActionResult> GetUserDetailsById(int userId)
      {
            var responseModel = new ResponseModel<User>();
            try
            {
                  _logger.LogInformation("GetUserDetailsById called for UserId: {UserId} at {DT}", userId, DateTime.UtcNow);
                  
                  var user = await _railwayBL.GetUserByIdAsync(userId);
                  responseModel.Success = true;
                  responseModel.Message = "User details retrieved successfully";
                  responseModel.Data = user;
                  
                  return Ok(responseModel);
            }
            catch (InvalidOperationException ex)
            {
                  _logger.LogError(ex.Message);
                  responseModel.Success = false;
                  responseModel.Message = ex.Message;
                  return NotFound(responseModel);
            }
            catch (Exception ex)
            {
                  _logger.LogError(ex.Message);
                  responseModel.Success = false;
                  responseModel.Message = "An error occurred while retrieving user details.";
                  return StatusCode(500, responseModel);
            }
      }

      [HttpGet("GetAllReservationsByUserId/{userId}")]
      [Authorize]
      public async Task<IActionResult> GetAllReservationsByUserId(int userId)
      {
            var responseModel = new ResponseModel<List<ReservationResponse>>();
            try
            {
                  _logger.LogInformation("GetAllReservationsByUserId called for UserId: {UserId} at {DT}", userId, DateTime.UtcNow);
                  
                  var reservations = await _railwayBL.GetAllReservationsByUserIdAsync(userId);
                  responseModel.Success = true;
                  responseModel.Message = "Reservations retrieved successfully";
                  responseModel.Data = reservations;
                  
                  return Ok(responseModel);
            }
            catch (Exception ex)
            {
                  _logger.LogError(ex.Message);
                  responseModel.Success = false;
                  responseModel.Message = "An error occurred while retrieving reservations.";
                  return StatusCode(500, responseModel);
            }
      }

      
}

