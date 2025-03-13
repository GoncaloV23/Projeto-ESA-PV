using Stripe;
using System.ComponentModel.DataAnnotations;

namespace EasyFitHub.Models.Payment
{
    public class Subscription : Buyable
    {
        public Subscription() : base(BuyableType.SUBSCRIPTION) { }
        public string? StripeSubscriptionId { get; set; }

        public override DebitCard GetBuyer()
        {
            return ClientDebitCard;
        }

        public override double GetCost()
        {
            return GymBank.GymSubscriptionPrice;
        }

        public override BankAccount GetSeller()
        {
            return GymBank;
        }
    }
}