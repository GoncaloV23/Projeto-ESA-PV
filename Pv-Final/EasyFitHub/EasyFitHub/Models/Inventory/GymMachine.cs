using EasyFitHub.Models.Miscalenous;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Inventory
{
    public class GymMachine
    {
        [Key]
        public int MachineId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; } = "It has no description!";
        public int Quantity { get; set; }

        [ForeignKey("HubImageId")]
        public int? ImageId { get; set; }
        public HubImage? Image { get; set; }

        public IList<Exercise> Exercise { get; set; } = new List<Exercise>();

        [ForeignKey("Id")]
        public int GymId { get; set; }
        public EasyFitHub.Models.Gym.Gym Gym { get; set; }
    }
}
