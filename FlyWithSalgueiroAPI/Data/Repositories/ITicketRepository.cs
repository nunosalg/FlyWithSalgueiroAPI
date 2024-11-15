﻿using FlyWithSalgueiroAPI.Data.Entities;

namespace FlyWithSalgueiroAPI.Data.Repositories
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<bool> PassengerAlreadyHasTicketInFlight(int flightId, string passengerId);

        IQueryable<Ticket> GetTicketsByUserEmail(string userEmail);
    }
}
