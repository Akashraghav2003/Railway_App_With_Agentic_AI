using System;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Services;

public class RailwayRL : IRailwayRL
{
    private RailwayContext _dbContext;


    public RailwayRL(RailwayContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> RegisterUserAsync(User newUser)
    {


        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return newUser;

    }

    public async Task<User?> GetUserAsync(UserDTO userDTO)
    {
        var result = await _dbContext.Users.FirstOrDefaultAsync(e => e.Email == userDTO.Email && e.UserName == userDTO.UserName);
        return result;
    }

    public async Task<User?> LogIn(LoginDTO loginDTO)
    {
        var result = await _dbContext.Users.FirstOrDefaultAsync(e => e.Email == loginDTO.EmailOrUserName || e.UserName == loginDTO.EmailOrUserName);
        return result;
    }

    public async Task<User?> ForgetPassword(string Email)
    {
        var result = await _dbContext.Users.FirstOrDefaultAsync(e => e.Email == Email);
        return result;
    }

    public async Task<User> ResetPassword(string email, string newPassword)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(e => e.Email == email);
        if (user == null)
        {
            throw new Exception("Email not found.");
        }
        user.Password = newPassword;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<List<Reservation>> GetAllReservationsByUserIdAsync(int userId)
    {
        return await _dbContext.Reservations
            .Include(r => r.Train)
            .Include(r => r.Passenger)
            .Where(r => r.UserID == userId)
            .ToListAsync();
    }

    
}
