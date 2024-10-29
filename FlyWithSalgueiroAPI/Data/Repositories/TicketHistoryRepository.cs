using FlyWithSalgueiroAPI.Data.Entities;

namespace FlyWithSalgueiroAPI.Data.Repositories
{
    public class TicketHistoryRepository : GenericRepository<TicketHistory>, ITicketHistoryRepository
    {
        private readonly DataContext _context;

        public TicketHistoryRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<TicketHistory> GetByUserEmail(string userEmail)
        {
            return _context.TicketsHistory.Where(t => t.TicketBuyer == userEmail);
        }
    }
}
