using EasyFitHub.Models.Miscalenous;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Inventory
{
    public class Exercise
    {
        [Key]
        public int ExerciseId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; } = "It has no description!";

        [ForeignKey("HubImageId")]
        public int? ImageId { get; set; }
        public HubImage? Image { get; set; }

        [ForeignKey("MachineId")]
        public int MachineId { get; set; }
        public GymMachine Machine { get; set; }
    }
}
