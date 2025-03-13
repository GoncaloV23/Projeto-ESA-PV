using EasyFitHub.Models.Miscalenous;
using EasyFitHub.Models.Profile;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Plan
{
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }
        public string Title { get; set; } = "Plan has no Title!";
        public string Description { get; set; } = "Plan has no Description!";
        public IList<PlanItem> Items { get; set; } = new List<PlanItem>();
        public PlanType PlanType { get; set; }
        public HubImage? HubImage { get; set; }

        [ForeignKey("ClientId")]
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
