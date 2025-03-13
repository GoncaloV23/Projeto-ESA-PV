using EasyFitHub.Models.Inventory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Payment
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [ForeignKey("ItemId")]
        public int ItemId { get; set; }
        public Item Item { get; set; }

        [ForeignKey("BuyableId")]
        public int CartId { get; set;}
        public Cart Cart { get; set; }

    }
}
