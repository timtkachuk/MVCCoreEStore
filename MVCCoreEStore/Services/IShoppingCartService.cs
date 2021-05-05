using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStore.Services
{
    public interface IShoppingCartService
    {
        void AddToCart(int id, int quantity = 1);

        int ItemCount();
    }
}
