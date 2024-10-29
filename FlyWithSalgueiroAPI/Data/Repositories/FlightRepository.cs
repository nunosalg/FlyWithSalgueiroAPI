using FlyWithSalgueiroAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlyWithSalgueiroAPI.Data.Repositories
{
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        private readonly DataContext _context;

        public FlightRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Flight> GetAvailableWithAircraftsAndCities()
        {
            return _context.Flights
                .Where(f => f.DepartureDateTime >= DateTime.UtcNow)
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .OrderBy(f => f.DepartureDateTime);
        }

        public async Task<IEnumerable<Flight>> GetFlightsByCriteriaAsync(int? originId, int? destinationId, DateTime? departureDate)
        {
            var query = _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .AsQueryable();

            if (originId.HasValue)
            {
                query = query.Where(f => f.Origin.Id == originId.Value);
            }

            if (destinationId.HasValue)
            {
                query = query.Where(f => f.Destination.Id == destinationId.Value);
            }

            if (departureDate.HasValue)
            {
                query = query.Where(f => f.DepartureDateTime.Date == departureDate.Value.Date);
            }

            return await query.ToListAsync();
        }

        public async Task<Flight?> GetByIdWithAircraftAndCities(int flightId)
        {
            return await _context.Flights
                .Where(f => f.DepartureDateTime >= DateTime.UtcNow && f.Id == flightId)
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .FirstOrDefaultAsync();
        }
    }
}
