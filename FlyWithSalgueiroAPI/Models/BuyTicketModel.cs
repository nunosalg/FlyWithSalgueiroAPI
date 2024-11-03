using System.ComponentModel.DataAnnotations;

namespace FlyWithSalgueiroAPI.Models
{
    public class BuyTicketModel
    {
        [Required]
        public int FlightId { get; set; }


        [Required(ErrorMessage = "Please select a seat.")]
        [Display(Name = "Select Seat")]
        public string? Seat { get; set; }


        [Required]
        [Display(Name = "Passenger Name")]
        public string? PassengerName { get; set; }


        [Required]
        [Display(Name = "Passenger ID")]
        [StringLength(8)]
        public string? PassengerId { get; set; }


        [Required]
        [Display(Name = "Passenger Birthdate")]
        public DateTime PassengerBirthDate { get; set; }
    }
}
