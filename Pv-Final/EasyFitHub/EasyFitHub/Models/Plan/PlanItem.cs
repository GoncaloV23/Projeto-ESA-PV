using EasyFitHub.Models.Miscalenous;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Plan
{
    public abstract class PlanItem
    {
        public PlanItem() { }
        [Key]
        public int PlanItemId { get; set; }
        public PlanItem(PlanType type) { PlanType = type; }
        public PlanType PlanType { get; set; } = PlanType.UNDEFINED;
        public string Name { get; set; } = "Plan has no Name!";
        public string Description { get; set; } = "Plan has no description!";

        [ForeignKey("PlanId")]
        public int PlanId { get; set; }
        public Plan Plan { get; set; }

        public abstract HubImage? GetImage();
    }
}
