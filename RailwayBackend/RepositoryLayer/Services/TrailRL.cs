using System;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Services;

public class TrainRL : ITrainRL
{
    private RailwayContext _dbContext;

    public TrainRL(RailwayContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Train> GetTrainAsync(TrainDTO trainDTO)
    {
        var train = await _dbContext.Trains.FirstOrDefaultAsync(e => e.TrainName == trainDTO.TrainName);
        if (train != null)
        {
            throw new InvalidOperationException("Train already present");
        }
        return train;
    }


    public async Task<int> AddTrainAsync(Train train)
    {
        // Add the train entity to the context
        _dbContext.Trains.Add(train);

        await _dbContext.SaveChangesAsync();

        // Return the generated TrainId after saving changes
        return train.TrainId;
    }




    public async Task<Train> GetTrainByIdAsync(int trainId)
    {
        return await _dbContext.Trains.FirstOrDefaultAsync(t => t.TrainId == trainId);
    }



    public async Task UpdateTrainAsync(Train train)
    {
        _dbContext.Trains.Update(train);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTrainClassesAsync(int trainId, List<TrainClass> newClasses)
    {
        var existing = await _dbContext.TrainClasses
            .Where(tc => tc.TrainId == trainId)
            .ToListAsync();

        _dbContext.TrainClasses.RemoveRange(existing);
        await _dbContext.SaveChangesAsync();

        foreach (var cls in newClasses)
        {
            cls.TrainId = trainId; // Make sure foreign key is set
        }

        await _dbContext.TrainClasses.AddRangeAsync(newClasses);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Train>> GetAllTrainsAsync()
    {
        return await _dbContext.Trains.Include(t => t.TrainClasses).ToListAsync();
    }

    public async Task<List<Train>> SearchTrainsAsync(string source, string destination)
    {
        return await _dbContext.Trains.Where(t => t.SourceStation == source && t.DestinationStation == destination).Include(t => t.TrainClasses)
            .ToListAsync();
    }


    public async Task DeductSeatsAsync(TrainClass trainClass, Train train, int noOfSeats)
    {
        trainClass.TotalSeat -= noOfSeats;
        train.TotalSeats -= noOfSeats;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Reservation>> GetReservationsByUserIdAsync(int userId)
    {
        return await _dbContext.Reservations
            .Include(r => r.Passenger)
            .Where(r => r.UserID == userId)
            .ToListAsync();
    }


    public async Task<Reservation> AddReservationAsync(Reservation reservation)
    {
        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync();
        return reservation;
    }

    public async Task<(TrainClass, Train)> CheckSeatAvailabilityAsync(int trainId, int classId, int requestedSeats)
    {
        var trainClass = await _dbContext.TrainClasses.FirstOrDefaultAsync(c => c.ClassId == classId && c.TrainId == trainId);

        var train = await _dbContext.Trains.FirstOrDefaultAsync(t => t.TrainId == trainId);

        if (trainClass == null || train == null)
            throw new KeyNotFoundException("Train or class not found");

        if (trainClass.TotalSeat < requestedSeats)
            throw new InvalidOperationException("Not enough seats available in the selected class");

        return (trainClass, train);
    }

    public async Task<string?> GetEmailAsync(int UserID)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(e => e.UserId == UserID);


        if (user == null)
        {
            throw new Exception($"User with ID {UserID} not found.");
        }

        return user.Email;
    }

    public async Task LogCancellationAsync(Cancellation cancellation)
    {
        cancellation.CancellationDate = DateTime.Now;
        _dbContext.Cancellations.Add(cancellation);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RestoreSeatsAsync(Reservation reservation)
    {
        var trainClass = await _dbContext.TrainClasses.FirstOrDefaultAsync(tc => tc.ClassId == reservation.ClassId && tc.TrainId == reservation.TrainId);
        var train = await _dbContext.Trains.FirstOrDefaultAsync(t => t.TrainId == reservation.TrainId);


        if (trainClass == null)
        {
            throw new Exception($"Train class with ID {reservation.ClassId} and Train ID {reservation.TrainId} not found.");
        }
        if (train == null)
        {
            throw new Exception($"Train with ID {reservation.TrainId} not found.");
        }


        if (trainClass != null) trainClass.TotalSeat += reservation.NoOfSeats;
        if (train != null) train.TotalSeats += reservation.NoOfSeats;

        await _dbContext.SaveChangesAsync();
    }

    public async Task CancelReservationAsync(Reservation reservation)
    {
        foreach (var item in _dbContext.Reservations)
        {
            if (item.ReservationId == reservation.ReservationId)
            {
                _dbContext.Reservations.Remove(item);
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Reservation> GetReservationByIdAsync(int id)
    {
        var reservation = await _dbContext.Reservations
           .Include(r => r.Passenger)
           .FirstOrDefaultAsync(r => r.ReservationId == id);

        if (reservation == null)
        {
            throw new Exception($"Reservation with ID {id} not found.");
        }
        return reservation;
    }

    public async Task<bool> CancelTrainAsync(int trainId)
    {
        var train = await _dbContext.Trains.FirstOrDefaultAsync(t => t.TrainId == trainId);
        if (train == null)
            return false;

        _dbContext.Trains.Remove(train);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
