using FlyWithSalgueiroAPI.Data.Entities;

namespace FlyWithSalgueiroAPI.Data.Repositories
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {
        IEnumerable<Flight> GetAvailableWithAircraftsAndCities();

        Task<IEnumerable<Flight>> GetFlightsByCriteriaAsync(int? originId, int? destinationId, DateTime? departureDate);

        Task<Flight?> GetByIdWithAircraftAndCities(int flightId);

        Task<Flight?> GetByIdWithUsersAircraftsAndCitiesAsync(int id);
    }
}
