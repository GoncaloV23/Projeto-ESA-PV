using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Payment
{
    public class PaymentDetails
    {
        [Key]
        public int PaymentDetailsId { get; set; }

        public DateOnly PaymentDate { get; set; } = DateOnly.MaxValue;
        public string Description { get; set; } = "No Description";
        public Status Status { get; set; }

        [ForeignKey("BuyableId")]
        public int BuyableId { get; set; }
        public Buyable Buyable { get; set; }
        
    }
}
