using EasyFitHub.Models.Miscalenous;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Inventory
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; } = "It has no description!";
        public int Quantity { get; set; }
        public double Price { get; set; }

        [ForeignKey("HubImageId")]
        public int? ImageId { get; set; }
        public HubImage? Image { get; set; }

        [ForeignKey("Id")]
        public int GymId { get; set; }
        public EasyFitHub.Models.Gym.Gym Gym { get; set; }
    }
}
