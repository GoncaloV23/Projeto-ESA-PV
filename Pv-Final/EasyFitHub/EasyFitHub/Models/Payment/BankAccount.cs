using Stripe;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Payment
{
    public class BankAccount
    {
        [Key]
        public int BankAccountId { get; set; }


        [ForeignKey("Id")]
        public int? GymId { get; set; }
        public Gym.Gym? Gym { get; set; }


        public string GymSubscriptionName { get; set; } = "Subscription Plan";
        public double GymSubscriptionPrice { get; set; } = 0;

        public string? StripeBankId { get; set; }
        public string? StripePlanId { get; set; }
        public ICollection<Buyable> Buyables { get; set; } = new List<Buyable>();
    }
}