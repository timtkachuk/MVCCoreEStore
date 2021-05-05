using Microsoft.AspNetCore.Http;
using MVCCoreEStore.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStore.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ShoppingCartService(
            IHttpContextAccessor httpContextAccessor
            )
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public void AddToCart(int id, int quantity = 1)
        {
            var shoppingCart = new List<ShoppingCartItemModel>();

            var cartCookie = httpContextAccessor.HttpContext.Request.Cookies["shoppingCart"];
            if (!string.IsNullOrEmpty(cartCookie))
                shoppingCart = JsonConvert.DeserializeObject<List<ShoppingCartItemModel>>(cartCookie);

            var shoppingCartItemModel = shoppingCart.SingleOrDefault(p => p.ProductId == id);
            if (shoppingCartItemModel != null)
            {
                shoppingCartItemModel.Quantity += quantity;
            }
            else
            {
                shoppingCartItemModel = new ShoppingCartItemModel { ProductId = id, Quantity = quantity };
                shoppingCart.Add(shoppingCartItemModel);
            }

            var options = new CookieOptions();
            options.Expires = DateTime.Today.AddDays(7);
            httpContextAccessor.HttpContext.Response.Cookies.Append("shoppingCart", JsonConvert.SerializeObject(shoppingCart));
        }

        public int ItemCount()
        {
            var shoppingCart = new List<ShoppingCartItemModel>();

            var cartCookie = httpContextAccessor.HttpContext.Request.Cookies["shoppingCart"];
            if (!string.IsNullOrEmpty(cartCookie))
                shoppingCart = JsonConvert.DeserializeObject<List<ShoppingCartItemModel>>(cartCookie);

            return shoppingCart.Sum(p => p.Quantity);
        }
    }
}
