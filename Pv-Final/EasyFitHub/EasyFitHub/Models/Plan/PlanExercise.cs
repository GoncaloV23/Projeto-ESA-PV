using EasyFitHub.Models.Inventory;
using EasyFitHub.Models.Miscalenous;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Plan
{
    public class PlanExercise : PlanItem
    {
        public PlanExercise() : base(PlanType.EXERCISE) { }

        [ForeignKey("ExerciseId")]
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }

        public override HubImage? GetImage()
        {
            return (Exercise == null) ? null : Exercise.Image;
        }
    }
}
