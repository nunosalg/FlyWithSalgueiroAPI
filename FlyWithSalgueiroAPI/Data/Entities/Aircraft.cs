using System.ComponentModel.DataAnnotations;

namespace FlyWithSalgueiroAPI.Data.Entities
{
    public class Aircraft : IEntity
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(50)]
        public string? Model { get; set; }


        [Required]
        [MaxLength(50)]
        public string? Airline { get; set; }


        [Required]
        public int Capacity { get; set; }


        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }


        [Display(Name = "Active")]
        public bool IsActive { get; set; }


        public List<string>? Seats { get; set; }


        public User? User { get; set; }


        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return "~/images/noimage.png";
                }

                //return $"https://localhost:44306{ImageUrl.Substring(1)}";
                return $"http://www.flywithsalgueiro.somee.com{ImageUrl.Substring(1)}";
            }
        }


        public string Data => $"{Model} {Airline}";


        public string ModelId => $"{Model} {Airline} {Id}";
    }
}
