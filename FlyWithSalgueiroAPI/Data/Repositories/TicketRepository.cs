using FlyWithSalgueiroAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlyWithSalgueiroAPI.Data.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        private readonly DataContext _context;

        public TicketRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Ticket> GetTicketsByUserEmail(string userEmail)
        {
            return _context.Tickets
                .Include(t => t.TicketBuyer)
                .Include(t => t.Flight)
                .ThenInclude(f => f.Origin)
                .Include(t => t.Flight)
                .ThenInclude(f => f.Destination)
                .Include(t => t.Flight)
                .ThenInclude(f => f.Aircraft)
                .Where(t => t.TicketBuyer.Email == userEmail && t.Flight.DepartureDateTime > DateTime.Now);
        }

        public async Task<bool> PassengerAlreadyHasTicketInFlight(int flightId, string passengerId)
        {
            return await _context.Tickets
                .AnyAsync(t => t.Flight.Id == flightId && t.PassengerId == passengerId);
        }
    }
}
