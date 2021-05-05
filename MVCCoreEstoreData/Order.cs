using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStoreData
{
    public enum OrderStates 
    {
        New, Shipped, Cancelled
    }
    public class Order : BaseEntity
    {
        public OrderStates OrderState { get; set; } = OrderStates.New;
        public string ShippingNumber { get; set; }

        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Order>(entity =>
            {

            });
        }
    }
}
