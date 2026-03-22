using System;
using AutoMapper;
using BusinessLayer.Interface;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service;

public class TrainBL : ITrainBL
{
    private readonly ITrainRL _trainRL;
    private readonly ILogger<RailwayBusinessLayer> _logger;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly IJwtToken _jWTToken;

    public TrainBL(ITrainRL trainRL, ILogger<RailwayBusinessLayer> logger, IEmailService emailService, IMapper mapper, IJwtToken jWTToken)
    {
        _trainRL = trainRL;
        _logger = logger;
        _emailService = emailService;
        _mapper = mapper;
        _jWTToken = jWTToken;
    }

    public async Task<bool> AddTrainAsync(TrainDTO trainDTO)
    {

        try
        {
            var train = await _trainRL.GetTrainAsync(trainDTO);

            var newTrain = _mapper.Map<Train>(trainDTO);

            var generatedTrainId = await _trainRL.AddTrainAsync(newTrain);
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

    public async Task<bool> UpdateTrainAsync(int trainId, TrainDTO trainDTO)
    {
        var train = await _trainRL.GetTrainByIdAsync(trainId);
        if (train == null)
            throw new InvalidOperationException("TrainID not present");

        _mapper.Map(trainDTO, train);
        await _trainRL.UpdateTrainAsync(train);

        var trainClassEntities = _mapper.Map<List<TrainClass>>(trainDTO.TrainClass);

        foreach (var cls in trainClassEntities)
            cls.TrainId = trainId;

        await _trainRL.UpdateTrainClassesAsync(trainId, trainClassEntities);

        return true;
    }

    public async Task<List<TrainResponseDTO>> GetAllTrainsAsync()
    {
        var trains = await _trainRL.GetAllTrainsAsync();
        return _mapper.Map<List<TrainResponseDTO>>(trains);
    }

    public async Task<List<TrainResponseDTO>> SearchTrainsAsync(string source, string destination)
    {
        var trains = await _trainRL.SearchTrainsAsync(source, destination);
        return _mapper.Map<List<TrainResponseDTO>>(trains);
    }

    public async Task<string> AddReservationAsync(ReservationDTO dto)
    {
        try
        {
            var (trainClass, train) = await _trainRL.CheckSeatAvailabilityAsync(dto.TrainId, dto.ClassId, dto.NoOfSeats);

            var reservation = _mapper.Map<Reservation>(dto);
            reservation.TotalFare = dto.NoOfSeats * (double)trainClass.Fare;
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            reservation.PNRNumber = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 900000000 + 100000000;
            reservation.BookingStatus = "Booked";

            string email = await _trainRL.GetEmailAsync(dto.UserID);


            await _trainRL.DeductSeatsAsync(trainClass, train, dto.NoOfSeats);

            var savedReservation = await _trainRL.AddReservationAsync(reservation);



            var emailModel = new EmailModel()
            {
                To = email,
                Subject = "Reservation successfully",
                Body = $"Ticket bokked Your Reservation Id == {savedReservation.ReservationId} PNR Number = {savedReservation.PNRNumber} and total fare = {savedReservation.TotalFare}  or booking status = {savedReservation.BookingStatus} with train Id = {savedReservation.TrainId}"
            };

            _emailService.SendEmail(emailModel);

            return $"Ticket booked Your Reservation Id == {savedReservation.ReservationId} PNR Number = {savedReservation.PNRNumber} and total fare = {savedReservation.TotalFare} or booking status = {savedReservation.BookingStatus} with train Id = {savedReservation.TrainId}";
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }


    }


    public async Task<List<ReservationResponse>> GetReservationsByUserIdAsync(int userId)
    {
        var reservations = await _trainRL.GetReservationsByUserIdAsync(userId);

        
        var responses = reservations.Select(reservation =>
         {
             var response = _mapper.Map<ReservationResponse>(reservation);
             return response;
         }).ToList();

        return responses;
    }


    public async Task<bool> CancelReservationAsync(CancellationDTO dto)
    {
        try
        {

            var reservation = await _trainRL.GetReservationByIdAsync(dto.ReservationId);
            if (reservation == null) return false;
            _logger.LogInformation(reservation.ToString());
            await _trainRL.RestoreSeatsAsync(reservation);


            var cancellation = _mapper.Map<Cancellation>(dto);
            cancellation.ReservationId = reservation.ReservationId;
            await _trainRL.LogCancellationAsync(cancellation);
            await _trainRL.CancelReservationAsync(reservation);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<bool> CancelTrainAsync(int trainId)
    {
        try
        {
            var train = await _trainRL.GetTrainByIdAsync(trainId);
            if (train == null)
                throw new InvalidOperationException("Train not found");

            var result = await _trainRL.CancelTrainAsync(trainId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
}
