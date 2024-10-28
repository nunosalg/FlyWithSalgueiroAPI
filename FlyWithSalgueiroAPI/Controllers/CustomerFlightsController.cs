using FlyWithSalgueiroAPI.Data.Repositories;
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

        public CustomerFlightsController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        [HttpGet("customerflights")]
        public IActionResult GetCustomerFlights()
        {
            try
            {
                var emailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
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
                        t.TicketBuyer,
                        t.Seat,
                        t.Price,
                        Flight = new
                        {
                            t.Flight.Id,
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
    }
}
