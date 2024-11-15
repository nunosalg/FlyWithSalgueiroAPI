﻿using System.ComponentModel.DataAnnotations;

namespace FlyWithSalgueiroAPI.Data.Entities
{
    public class Flight : IEntity
    {
        public int Id { get; set; }


        [Display(Name = "Flight Number")]
        public string FlightNumber => Id > 9 ? $"FWS{Id}" : $"FWS0{Id}";


        [Required]
        [Display(Name = "Departure")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DepartureDateTime { get; set; }


        [Required]
        [Display(Name = "Duration")]
        public TimeSpan FlightDuration { get; set; }


        [Display(Name = "Arrival")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime ArrivalTime => DepartureDateTime + FlightDuration;


        [Required]
        public City? Origin { get; set; }


        [Required]
        public City? Destination { get; set; }


        [Required]
        [Display(Name = "Origin Airport")]
        public string? OriginAirport { get; set; }


        [Required]
        [Display(Name = "Destination Airport")]
        public string? DestinationAirport { get; set; }


        [Required]
        public Aircraft? Aircraft { get; set; }


        public List<string> AvailableSeats { get; set; } = new List<string>();


        [Display(Name = "Available Seats")]
        public int AvailableSeatsNumber => AvailableSeats.Count;


        public List<Ticket> TicketsList { get; set; } = new List<Ticket>();


        [Display(Name = "Tickets Sold")]
        public int TicketsSold => TicketsList.Count;


        [Required]
        public User? User { get; set; }
    }
}
