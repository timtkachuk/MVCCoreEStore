﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MVCCoreEStore.Models;
using MVCCoreEStore.Services;
using MVCCoreEStoreData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MVCCoreEStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext context;
        private readonly SignInManager<User> signInManager;
        private readonly IShoppingCartService shoppingCartService;
        private readonly UserManager<User> userManager;

        public HomeController(
            ILogger<HomeController> logger,
            AppDbContext context,
            SignInManager<User> signInManager,
            IShoppingCartService shoppingCartService,
            UserManager<User> userManager
            )
        {
            _logger = logger;
            this.context = context;
            this.signInManager = signInManager;
            this.shoppingCartService = shoppingCartService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.FeaturedProducts = await context.Products.Where(p => p.Enabled).OrderBy(p => Guid.NewGuid()).Take(16).ToListAsync();
            ViewBag.TopSellers = await context.Products.Where(p => p.Enabled).OrderByDescending(p => p.OrderItems.Sum(q => q.Quantity)).Take(8).ToListAsync();
            return View();
        }

        public async Task<IActionResult> Category(int id)
        {
            var category = await context.Categories.FindAsync(id);
            return View(category);
        }

        public async Task<IActionResult> Brand(int id)
        {
            var brand = await context.Brands.FindAsync(id);
            return View(brand);
        }

        public async Task<IActionResult> Product(int id)
        {
            if (id == 0)
            {
                return default;
            }
            var product = await context.Products.FindAsync(id);

            var oldJson = HttpContext.Session.GetString("productReviews");
            //var viewedProducts = oldJson == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(oldJson);
            var viewedProducts = JsonConvert.DeserializeObject<List<int>>(oldJson ?? "[]");

            if (!viewedProducts.Any(p => p == id))
            {
                viewedProducts.Add(id);

                var json = JsonConvert.SerializeObject(viewedProducts);
                HttpContext.Session.SetString("productReviews", json);

                product.Reviews++;
                context.Entry(product).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }

            var carousel = product.ProductPictures.Select(p => p.Picture).ToList();
            carousel.Insert(0, product.Picture);
            ViewBag.Carousel = carousel;

            ViewBag.CanAddComment = false;
            if (User.Identity.Name != null)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.CanAddComment = context.OrderItems.Any(p => p.ProductId == id && p.Order.User.Id == user.Id);
            }

            return View(product);
        }

        public async Task<IActionResult> AddToCart(AddToCartViewModel model)
        {
            if (!signInManager.IsSignedIn(User))
            {
                shoppingCartService.AddToCart(model.ProductId, model.Quantity);
            }
            else
            {
                var userId = int.Parse(userManager.GetUserId(User));
                var item = await context.ShoppingCartItems.SingleOrDefaultAsync(p => p.UserId == userId && p.ProductId == model.ProductId);
                if (item == null)
                {
                    item = new ShoppingCartItem { ProductId = model.ProductId, Quantity = model.Quantity, UserId = userId };
                    context.Entry(item).State = EntityState.Added;
                }
                else
                {
                    item.Quantity += model.Quantity;
                    context.Entry(item).State = EntityState.Modified;
                }
                await context.SaveChangesAsync();
            }
            return Redirect("/");
        }

        public IActionResult Search(string Keywords)
        {
            var keywords = Regex.Split(Keywords.ToLower(CultureInfo.CreateSpecificCulture("tr-TR")), @"\s+").ToList();

            var model = context.Products.AsEnumerable().Where(p =>
                keywords.Any(q => p.Name.ToLower().Contains(q)) ||
                keywords.Any(q => p.Code.ToLower().Contains(q)) ||
                keywords.Any(q => p.Descriptions?.ToLower().Contains(q) ?? false) ||
                keywords.Any(q => p.Brand.Name.ToLower().Contains(q))
            ).ToList();

            ViewBag.Keywords = Keywords;
            ViewBag.Results = model.Count;
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Comment(ProductComment model)
        {
            model.Date = DateTime.Now;
            model.Enabled = false;
            model.UserId = int.Parse(userManager.GetUserId(User));

            context.Entry(model).State = EntityState.Added;
            await context.SaveChangesAsync();

            return RedirectToAction("Product", new { id = model.ProductId });
        }
    }
}
