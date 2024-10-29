using FlyWithSalgueiroAPI.Data.Repositories;
using FlyWithSalgueiroAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlyWithSalgueiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomerFlightsController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketHistoryRepository _ticketHistoryRepository;
        private readonly IUserHelper _userHelper;

        public CustomerFlightsController(
            ITicketRepository ticketRepository,
            ITicketHistoryRepository ticketHistoryRepository,
            IUserHelper userHelper)
        {
            _ticketRepository = ticketRepository;
            _ticketHistoryRepository = ticketHistoryRepository;
            _userHelper = userHelper;
        }

        [HttpGet("FutureFlights")]
        public IActionResult GetFutureFlights()
        {
            try
            {
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var userEmail = emailClaim?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    return NotFound("User not found.");
                }

                var tickets = _ticketRepository.GetTicketsByUserEmail(userEmail)
                    .Select(t => new
                    {
                        t.Id,
                        t.PassengerName,
                        t.PassengerId,
                        t.PassengerBirthDate,
                        t.Seat,
                        t.Price,
                        Flight = new
                        {
                            t.Flight.FlightNumber,
                            t.Flight.DepartureDateTime,
                            FlightDuration = t.Flight.FlightDuration.ToString(),
                            t.Flight.ArrivalTime,
                            Origin = t.Flight.Origin.Name,
                            t.Flight.OriginAirport,
                            Destination = t.Flight.Destination.Name,
                            t.Flight.DestinationAirport,
                            Aircraft = t.Flight.Aircraft.Data,
                        }
                    })
                    .ToList();

                if (tickets == null || tickets.Count == 0)
                {
                    return NotFound("No future flights found for this user.");
                }

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("FlightsHistory")]
        public IActionResult GetFlightsHistory()
        {
            try
            {
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var userEmail = emailClaim?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    return NotFound("User not found.");
                }

                var ticketsHistory = _ticketHistoryRepository.GetByUserEmail(userEmail).ToList();

                if (ticketsHistory == null || ticketsHistory.Count == 0)
                {
                    return NotFound("No past flights have been found for this user.");
                }

                return Ok(ticketsHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
