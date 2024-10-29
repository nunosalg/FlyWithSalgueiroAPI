using FlyWithSalgueiroAPI.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FlyWithSalgueiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightRepository _flightRepository;

        public FlightsController(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }

        [HttpGet("AvailableFlights")]
        public IActionResult GetAvailableFlights()
        {
            try
            {
                var flights = _flightRepository.GetAvailableWithAircraftsAndCities();
                    
                if (flights == null)
                {
                    return NotFound("No available flights found at the moment.");
                }

                return Ok(flights);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("SearchFlights")]
        public async Task<IActionResult> SearchFlights(int? originId, int? destinationId, DateTime? departure)
        {
            try
            {
                var flights = await _flightRepository.GetFlightsByCriteriaAsync(originId, destinationId, departure);
                if (flights == null)
                {
                    return NotFound("No flights found matching these criteria.");
                }

                return Ok(flights);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
