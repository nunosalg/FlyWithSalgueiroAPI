using FlyWithSalgueiroAPI.Data.Entities;

namespace FlyWithSalgueiroAPI.Data.Repositories
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        private readonly DataContext _context;

        public CityRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
