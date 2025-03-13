using EasyFitHub.Models.Miscalenous;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Plan
{
    public class PlanMeal : PlanItem
    {
        public PlanMeal() : base(PlanType.NUTRITION) { }
        [ForeignKey("HubImageId")]
        public int? HubImageId { get; set; }
        public HubImage? HubImage { get; set; }

        public override HubImage? GetImage()
        {
            return HubImage;
        }
    }
}
