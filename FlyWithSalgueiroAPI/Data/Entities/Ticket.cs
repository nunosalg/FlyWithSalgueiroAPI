﻿using System.ComponentModel.DataAnnotations;

namespace FlyWithSalgueiroAPI.Data.Entities
{
    public class Ticket : IEntity
    {
        public int Id { get; set; }


        [Required]
        public Flight? Flight { get; set; }


        [Required]
        public string? Seat { get; set; }


        [Required]
        [Display(Name = "Ticket Buyer")]
        public User? TicketBuyer { get; set; }


        [Required]
        [Display(Name = "Passenger Name")]
        public string? PassengerName { get; set; }


        [Required]
        [Display(Name = "Passenger ID")]
        public string? PassengerId { get; set; }


        [Required]
        [Display(Name = "Passenger Birthdate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime PassengerBirthDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }
    }
}