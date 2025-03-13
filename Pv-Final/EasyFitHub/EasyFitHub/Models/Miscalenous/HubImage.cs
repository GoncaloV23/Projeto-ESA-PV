using System.ComponentModel.DataAnnotations;

namespace EasyFitHub.Models.Miscalenous
{
    public class HubImage
    {
        [Key]
        public int HubImageId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
        [Required]
        public string? Path { get; set; }
        [Required]
        public int Width { get; set; }
        [Required]
        public int Height { get; set; }

        public int? GymId { get; set; }
        public EasyFitHub.Models.Gym.Gym? Gym { get; set; }
    }
}
