using System;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface;

public interface ITrainRL
{
    Task<Train> GetTrainAsync(TrainDTO trainDTO);
    Task<Train> GetTrainByIdAsync(int trainId);
    Task<int> AddTrainAsync(Train train);
    Task<List<Reservation>> GetReservationsByUserIdAsync(int userId);
    Task UpdateTrainAsync(Train train);
    Task UpdateTrainClassesAsync(int trainId, List<TrainClass> newClasses);
    Task<List<Train>> GetAllTrainsAsync();
    Task<List<Train>> SearchTrainsAsync(string source, string destination);

    Task DeductSeatsAsync(TrainClass trainClass, Train train, int noOfSeats);
    Task<Reservation> AddReservationAsync(Reservation reservation);
    Task<(TrainClass, Train)> CheckSeatAvailabilityAsync(int trainId, int classId, int requestedSeats);

    Task<string?> GetEmailAsync(int UserID);

    Task LogCancellationAsync(Cancellation cancellation);
    Task RestoreSeatsAsync(Reservation reservation);
    Task CancelReservationAsync(Reservation reservation);
    Task<Reservation?> GetReservationByIdAsync(int id);
    Task<bool> CancelTrainAsync(int trainId);
}
