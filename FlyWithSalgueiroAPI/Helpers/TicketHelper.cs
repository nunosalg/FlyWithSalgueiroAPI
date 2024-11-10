using FlyWithSalgueiroAPI.Data.Entities;
using FlyWithSalgueiroAPI.Data.Repositories;
using FlyWithSalgueiroAPI.Models;

namespace FlyWithSalgueiroAPI.Helpers
{
    public class TicketHelper : ITicketHelper
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IUserHelper _userHelper;

        public TicketHelper(IFlightRepository flightRepository, IUserHelper userHelper)
        {
            _flightRepository = flightRepository;
            _userHelper = userHelper;
        }

        public decimal TicketPrice(Flight flight)
        {
            decimal costPerMinute = 0.5m;
            decimal distanceInMinutes = (decimal)flight.FlightDuration.TotalMinutes;
            decimal ocupationCost = flight.TicketsSold * 0.1m;

            decimal ticketPrice = costPerMinute * distanceInMinutes + ocupationCost;

            return ticketPrice;
        }

        public async Task<Ticket> ToTicketAsync(BuyTicketModel model, User clientUser, int flightId)
        {
            var user = await _userHelper.GetUserByEmailAsync(clientUser.UserName);

            var flight = await _flightRepository.GetByIdWithAircraftAndCities(flightId);

            return new Ticket
            {
                Flight = flight,
                Seat = model.Seat,
                TicketBuyer = user,
                PassengerId = model.PassengerId,
                PassengerName = model.PassengerName,
                PassengerBirthDate = model.PassengerBirthDate,
                Price = model.Price,
            };
        }
    }
}
