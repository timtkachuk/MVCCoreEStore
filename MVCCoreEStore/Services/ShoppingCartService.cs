using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCCoreEStore.Controllers;
using MVCCoreEStoreData;
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
        private readonly AppDbContext context;
        private readonly UserManager<User> userManager;

        public ShoppingCartService(
            IHttpContextAccessor httpContextAccessor,
            AppDbContext context,
            UserManager<User> userManager
            )
        {
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
            this.userManager = userManager;
        }

        public async Task AddToCart(int id, int quantity = 1)
        {

            if (string.IsNullOrEmpty(httpContextAccessor.HttpContext.User.Identity.Name))
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
            else
            {
                var user = await userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name);
                var shopingCartItem = user.ShoppingcartItems.SingleOrDefault(p => p.ProductId == id);
                if (shopingCartItem != null)
                {
                    shopingCartItem.Quantity += quantity;
                    context.Entry(shopingCartItem).State = EntityState.Modified;
                }
                else
                {
                    shopingCartItem = new ShoppingCartItem
                    {
                        ProductId = id,
                        Quantity = quantity,
                        UserId = user.Id
                    };
                    context.Entry(shopingCartItem).State = EntityState.Added;
                }
                await context.SaveChangesAsync();
            }
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
