namespace FlyWithSalgueiroAPI.Models
{
    public class AvailableSeatsDto
    {
        public int FlightId { get; set; }

        public List<string> AvailableSeats { get; set; } = new List<string>();
    }
}
