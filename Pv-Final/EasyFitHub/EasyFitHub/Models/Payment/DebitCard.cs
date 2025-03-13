using EasyFitHub.Models.Profile;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Payment
{
    public class DebitCard
    {
		[Key]
        public int DebitCardId { get; set; }
        
        [ForeignKey("ClientId")]
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        
        public string? StripeCustomerId { get; set; }
        public string? StripePaymentMethodId { get; set; }
        public ICollection<Buyable> Buyables { get; set; } = new List<Buyable>();

    }
}