using System;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface;

public interface IRailwayRL
{
    Task<User> RegisterUserAsync(User newUser);
    Task<User> GetUserAsync(UserDTO userDTO);
    Task<User> LogIn(LoginDTO loginDTO);

    Task<User> ForgetPassword(string Email);
    Task<User> ResetPassword(string email, string password);
    Task<User> GetUserByIdAsync(int userId);
    Task<List<Reservation>> GetAllReservationsByUserIdAsync(int userId);
    
}
