using FlyWithSalgueiroAPI.Data.Repositories;
using FlyWithSalgueiroAPI.Helpers;
using FlyWithSalgueiroAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlyWithSalgueiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightRepository _flightRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserHelper _userHelper;
        private readonly ITicketHelper _ticketHelper;

        public FlightsController(
            IFlightRepository flightRepository,
            ITicketRepository ticketRepository,
            IUserHelper userHelper,
            ITicketHelper ticketHelper)
        {
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
            _userHelper = userHelper;
            _ticketHelper = ticketHelper;
        }

        [HttpGet("AvailableFlights")]
        public IActionResult GetAvailableFlights()
        {
            try
            {
                var flights = _flightRepository.GetAvailableWithAircraftsAndCities()
                    .Select(f => new
                    {
                        f.Id,
                        f.FlightNumber,
                        f.DepartureDateTime,
                        f.FlightDuration,
                        f.Origin,
                        f.OriginAirport,
                        f.Destination,
                        f.DestinationAirport,
                        f.AvailableSeatsNumber,
                    });

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
                var flightsResult = await _flightRepository.GetFlightsByCriteriaAsync(originId, destinationId, departure);

                var flights = flightsResult.ToList()
                    .Select(f => new
                    {
                        f.Id,
                        f.FlightNumber,
                        f.DepartureDateTime,
                        f.FlightDuration,
                        f.Origin,
                        f.OriginAirport,
                        f.Destination,
                        f.DestinationAirport,
                        f.AvailableSeatsNumber,
                    });

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

        [HttpGet("AvailableSeats")]
        public async Task<IActionResult> GetAvailableSeats(int flightId)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
            {
                return NotFound("Flight not found.");
            }

            var availableSeatsDto = new AvailableSeatsDto
            {
                FlightId = flight.Id,
                AvailableSeats = flight.AvailableSeats
            };

            return Ok(availableSeatsDto);
        }


        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> BuyTicket(BuyTicketModel model)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var flight = await _flightRepository.GetByIdWithAircraftAndCities(model.FlightId);
            if (flight == null)
            {
                return NotFound("Flight not found");
            }

            if (await _ticketRepository.PassengerAlreadyHasTicketInFlight(flight.Id, model.PassengerId))
            {
                return BadRequest($"The passenger with ID {model.PassengerId} already has a ticket for this flight.");
            }

            var ticket = await _ticketHelper.ToTicketAsync(model, user, flight.Id);

            flight.AvailableSeats.Remove(model.Seat.ToUpper());
            flight.TicketsList.Add(ticket);

            try
            {
                await _ticketRepository.CreateAsync(ticket);
                await _flightRepository.UpdateAsync(flight);

                return Ok("Ticket purchased successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
