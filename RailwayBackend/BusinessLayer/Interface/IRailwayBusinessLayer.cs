using System;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface;

public interface IRailwayBusinessLayer
{
    Task<User> RegisterUserAsync(UserDTO userDto);
    Task<string> LogIn(LoginDTO loginDTO);

    Task<bool> ForgetPassword(string Email);
    Task<bool> ResetPassword(ResetModel resetModel);
    Task<User> GetUserByIdAsync(int userId);
    Task<List<ReservationResponse>> GetAllReservationsByUserIdAsync(int userId);
    
}
