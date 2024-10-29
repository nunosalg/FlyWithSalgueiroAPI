using FlyWithSalgueiroAPI.Data.Entities;
using FlyWithSalgueiroAPI.Models;

namespace FlyWithSalgueiroAPI.Helpers
{
    public interface ITicketHelper
    {
        decimal TicketPrice(Flight flight);

        Task<Ticket> ToTicketAsync(BuyTicketModel model, User clientUser, int flightId);
    }
}
