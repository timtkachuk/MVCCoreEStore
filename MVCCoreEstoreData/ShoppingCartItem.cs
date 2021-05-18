using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCCoreEStoreData
{
    public class ShoppingCartItem : AppEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public virtual User User { get; set; }
        public virtual Product Product { get; set; }

        [NotMapped]
        public decimal Amount => Product.DiscountedPrice * Quantity;

        public override void Build(ModelBuilder builder)
        {

        }
    }
}
