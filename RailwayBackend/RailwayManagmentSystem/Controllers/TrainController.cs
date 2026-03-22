using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;

namespace RailwayManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainController : ControllerBase
    {
        private readonly ITrainBL _trainBL;
        private readonly ILogger<RailwayTicketBooking> _logger;

       
        public TrainController(ITrainBL trainBL, ILogger<RailwayTicketBooking> logger)
        {
            _trainBL = trainBL;
            _logger = logger;
        }


        
        [HttpPost("AddTrain")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTrain([FromBody] TrainDTO trainDTO)
        {
            ResponseModel<string> responseModel = new ResponseModel<string>();
            try
            {
                var result = await _trainBL.AddTrainAsync(trainDTO);
                if (result)
                {
                    responseModel.Success = true;
                    responseModel.Message = "Train added successfully";
                    responseModel.Data = "Train Added Successfully";

                    return Ok(responseModel);
                }

                responseModel.Success = false;
                responseModel.Message = "Facing the problem";
                responseModel.Data = "Train not Added";
                return BadRequest(responseModel);

            }
            catch (InvalidOperationException ex)
            {
                responseModel.Success = false;
                responseModel.Message = ex.Message;
                responseModel.Data = "Train not Added";
                _logger.LogError(ex.Message);
                return BadRequest(responseModel);
            }
        }

        [HttpPut("UpdateTrain")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdateTrain(int trainID, TrainDTO trainDTO)
        {
            ResponseModel<string> responseModel = new ResponseModel<string>();
            try
            {
                var result = await _trainBL.UpdateTrainAsync(trainID, trainDTO);
                if (result)
                {
                    responseModel.Success = true;
                    responseModel.Message = "Train update successfully";
                    responseModel.Data = "Train update Successfully";

                    return Ok(responseModel);
                }

                responseModel.Success = true;
                responseModel.Message = "Facing the problem";
                responseModel.Data = "Train not update";
                return BadRequest(responseModel);

            }
            catch (InvalidOperationException ex)
            {
                responseModel.Success = true;
                responseModel.Message = ex.Message;
                responseModel.Data = "Train not update";
                _logger.LogError(ex.Message);
                return BadRequest(responseModel);
            }
        }

        [HttpGet("GetAllTrains")]
        public async Task<IActionResult> GetAllTrains()
        {
            ResponseModel<List<TrainResponseDTO>> responseModel = new ResponseModel<List<TrainResponseDTO>>();
            try
            {
                var trains = await _trainBL.GetAllTrainsAsync();
                responseModel.Success = true;
                responseModel.Message = "All trains retrieved successfully";
                responseModel.Data = trains;
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                responseModel.Success = false;
                responseModel.Message = "An error occurred while fetching trains: " + ex.Message;
                return StatusCode(500, responseModel);
            }
        }

        [HttpGet("GetTrains")]
        public async Task<IActionResult> GetTrainAsync()
        {
            ResponseModel<List<TrainResponseDTO>> responseModel = new ResponseModel<List<TrainResponseDTO>>();
            try
            {
                var allTrains = await _trainBL.GetAllTrainsAsync();
                var currentDate = DateTime.Today;
                var trains = allTrains.Where(t => t.DepartureTime.Date == currentDate).ToList();
                
                if (trains.Any())
                {
                    responseModel.Success = true;
                    responseModel.Message = "Train details are as follows:";
                    responseModel.Data = trains;

                    return Ok(responseModel);
                }
                responseModel.Success = false;
                responseModel.Message = "No trains are available for today";
                responseModel.Data = null;

                return NotFound(responseModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                responseModel.Success = false;
                responseModel.Message = "An error occurred while fetching the data " + ex.Message;

                return StatusCode(500, responseModel);
            }
        }


        [HttpGet("Reservatrion/{userId}")]
        public async Task<ActionResult<List<ReservationResponse>>> GetReservationsByUserId(int userId)
        {
            try
            {
                var reservations = await _trainBL.GetReservationsByUserIdAsync(userId);
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("search")]
        public async Task<IActionResult> Search(string source, string destination)
        {
            ResponseModel<List<TrainResponseDTO>> responseModel = new ResponseModel<List<TrainResponseDTO>>();
            try
            {
                var trains = await _trainBL.SearchTrainsAsync(source, destination);
                if (trains.Any())
                {
                    responseModel.Success = true;
                    responseModel.Message = "Train details are as follows:";
                    responseModel.Data = trains;

                    return Ok(responseModel);
                }
                responseModel.Success = false;
                responseModel.Message = "No trains are available";
                responseModel.Data = null;

                return NotFound(responseModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                responseModel.Success = false;
                responseModel.Message = "An error occurred while fetching the data " + ex.Message;


                return StatusCode(500, responseModel);
            }
        }

        
        [HttpPost("AddReservation")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> AddReservation([FromBody] ReservationDTO reservationDto)
        {
            var responseModel = new ResponseModel<string>();
            try
            {
                var result = await _trainBL.AddReservationAsync(reservationDto);
                responseModel.Success = true;
                responseModel.Message = "Reservation added successfully.";
                responseModel.Data = result;

                return Ok(responseModel);
            }
            catch (KeyNotFoundException ex)
            {
                responseModel.Success = false;
                responseModel.Message = ex.Message;
                responseModel.Data = null;
                _logger.LogError(ex.Message);
                return NotFound(responseModel);
            }
            catch (InvalidOperationException ex)
            {
                responseModel.Success = false;
                responseModel.Message = ex.Message;
                responseModel.Data = null;
                _logger.LogError(ex.Message);
                return BadRequest(responseModel);
            }
            catch (Exception ex)
            {
                responseModel.Success = false;
                responseModel.Message = "Internal server error." + ex.Message;
                responseModel.Data = null;
                _logger.LogError(ex.Message);
                return StatusCode(500, responseModel);
            }
        }

      

        [HttpPost("CancelReservation")]
        // [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CancelReservation([FromBody] CancellationDTO dto)
        {
            var responseModel = new ResponseModel<string>();
            try
            {
                var result = await _trainBL.CancelReservationAsync(dto);
                responseModel.Success = result;
                responseModel.Message = result ? "Reservation cancelled successfully." : "Reservation not found.";
                responseModel.Data = result ? "Reservation cancelled successfully." : "Reservation not found.";

                return result ? Ok(responseModel) : NotFound(responseModel);
            }
            catch (ArgumentException ex)
            {
                responseModel.Success = false;
                responseModel.Message = "Internal server error. error aa rhi hai " + ex.Message;
                responseModel.Data = null;
                _logger.LogError(ex.Message);
                return StatusCode(500, responseModel);
            }
          
        }

        [HttpDelete("CancelTrain/{trainId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelTrain(int trainId)
        {
            var responseModel = new ResponseModel<string>();
            try
            {
                var result = await _trainBL.CancelTrainAsync(trainId);
                if (result)
                {
                    responseModel.Success = true;
                    responseModel.Message = "Train cancelled successfully";
                    responseModel.Data = "Train has been cancelled";
                    return Ok(responseModel);
                }
                
                responseModel.Success = false;
                responseModel.Message = "Train not found";
                responseModel.Data = "Unable to cancel train";
                return NotFound(responseModel);
            }
            catch (InvalidOperationException ex)
            {
                responseModel.Success = false;
                responseModel.Message = ex.Message;
                responseModel.Data = "Train cancellation failed";
                _logger.LogError(ex.Message);
                return BadRequest(responseModel);
            }
            catch (Exception ex)
            {
                responseModel.Success = false;
                responseModel.Message = "Internal server error";
                responseModel.Data = ex.Message;
                _logger.LogError(ex.Message);
                return StatusCode(500, responseModel);
            }
        }

    }
}
