using EasyFitHub.Models.Inventory;

namespace EasyFitHub.Models.Payment
{
    public class Cart : Buyable
    {
        public Cart() : base(BuyableType.CART) { }

        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public override DebitCard GetBuyer()
        {
            return ClientDebitCard;
        }

        public override double GetCost()
        {
            double sum = 0;
            foreach (var item in Items)
            {
                sum += item.Item.Price;
            }

            return sum;
        }

        public override BankAccount GetSeller()
        {
            return GymBank;
        }
    }
}