using FlyWithSalgueiroAPI.Data.Entities;

namespace FlyWithSalgueiroAPI.Data.Repositories
{
    public interface ITicketHistoryRepository : IGenericRepository<TicketHistory>
    {
        IQueryable<TicketHistory> GetByUserEmail(string userEmail);
    }
}
