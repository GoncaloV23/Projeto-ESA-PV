using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Payment
{
    public abstract class Buyable
    {
        public Buyable() { }
        public Buyable(BuyableType type) { BuyableType = type; }

        [Key]
        public int BuyableId { get; set; }
        
        public BuyableType BuyableType{ get; set; } = BuyableType.UNDEFINED;

        [ForeignKey("BankAccountId")]
        public int BankAccountId { get; set; }
        public BankAccount GymBank { get; set; }


        [ForeignKey("DebitCardId")]
        public int DebitCardId { get; set; }
        public DebitCard ClientDebitCard { get; set; }


        public abstract double GetCost();
        public abstract DebitCard GetBuyer();
        public abstract BankAccount GetSeller();
        

    }
}