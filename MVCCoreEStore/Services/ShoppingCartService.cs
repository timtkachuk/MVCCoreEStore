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

        public async Task AddToCartAsync(int id, int quantity = 1)
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
                options.Secure = true;
                httpContextAccessor.HttpContext.Response.Cookies.Append("shoppingCart", JsonConvert.SerializeObject(shoppingCart), options);
            }
            else
            {
                var user = await userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name);
                var shoppingCartItem = user.ShoppingCartItems.SingleOrDefault(p => p.ProductId == id);
                if (shoppingCartItem != null)
                {
                    shoppingCartItem.Quantity += quantity;
                    context.Entry(shoppingCartItem).State = EntityState.Modified;
                }
                else
                {
                    shoppingCartItem = new ShoppingCartItem
                    {
                        ProductId = id,
                        Quantity = quantity,
                        UserId = user.Id
                    };
                    context.Entry(shoppingCartItem).State = EntityState.Added;
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartAsync(int id)
        {
            var item = await context.ShoppingCartItems.FindAsync(id);
            context.Entry(item).State = EntityState.Deleted;
            await context.SaveChangesAsync();
        }

        public async Task<int> ItemCountAsync()
        {
            var userName = httpContextAccessor.HttpContext.User.Identity.Name;
            var itemCount = 0;
            if (userName != null)
            {
                var user = await userManager.FindByNameAsync(userName);
                itemCount = user.ShoppingCartItems.Sum(p => p.Quantity);
            }
            else
            {
                var shoppingCart = new List<ShoppingCartItemModel>();

                var cartCookie = httpContextAccessor.HttpContext.Request.Cookies["shoppingCart"];
                if (!string.IsNullOrEmpty(cartCookie))
                    shoppingCart = JsonConvert.DeserializeObject<List<ShoppingCartItemModel>>(cartCookie);

                itemCount = shoppingCart.Sum(p => p.Quantity);
            }
            return itemCount;
        }

        public async Task ClearCookieAsync()
        {
            await Task.Run(() =>
            {
                httpContextAccessor.HttpContext.Response.Cookies.Delete("shoppingCart", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
            });
        }

        public async Task TransferCookieToDatabaseAsync()
        {
            var userName = httpContextAccessor.HttpContext.User.Identity.Name;
            if (userName == null)
            {
                throw new InvalidOperationException("Kullanıcı olmadan bu method çağırılamaz");
            }
            var user = await userManager.FindByNameAsync(userName);

            var shoppingCart = new List<ShoppingCartItemModel>();

            var cartCookie = httpContextAccessor.HttpContext.Request.Cookies["shoppingCart"];
            if (!string.IsNullOrEmpty(cartCookie))
                shoppingCart = JsonConvert.DeserializeObject<List<ShoppingCartItemModel>>(cartCookie);

            shoppingCart
                .ToList()
                .ForEach(p => AddToCartAsync(p.ProductId, p.Quantity).Wait());

            await context.SaveChangesAsync();

            await ClearCookieAsync();
        }

        public async Task ClearShoppingCartAsync()
        {
            var userName = httpContextAccessor.HttpContext.User.Identity.Name;
            var user = await userManager.FindByNameAsync(userName);
            user.ShoppingCartItems
                .ToList()
                .ForEach(p => context.Entry(p).State = EntityState.Deleted);
            await context.SaveChangesAsync();
        }
    }
}
