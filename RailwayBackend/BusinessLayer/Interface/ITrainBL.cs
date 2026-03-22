using System;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface;

public interface ITrainBL
{
    Task<bool> AddTrainAsync(TrainDTO trainDTO);

    Task<bool> UpdateTrainAsync(int trainID, TrainDTO trainDTO);

    Task<List<TrainResponseDTO>> GetAllTrainsAsync();
    Task<List<TrainResponseDTO>> SearchTrainsAsync(string source, string destination);
    Task<string> AddReservationAsync(ReservationDTO dto);
    Task<bool> CancelReservationAsync(CancellationDTO dto);
    Task<List<ReservationResponse>> GetReservationsByUserIdAsync(int userId);
    Task<bool> CancelTrainAsync(int trainId);
}
