﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCCoreEStoreData;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MVCCoreEStore.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Administrators, ProductAdministrators")]
    public class RayonsController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<User> userManager;

        private readonly string name = "Reyon";

        public RayonsController(AppDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await context.Rayons.OrderBy(p => p.SortOrder).ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new Rayon { Enabled = true });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Rayon model)
        {

            model.Date = DateTime.Now;
            model.Enabled = false;
            model.UserId = (await userManager.FindByNameAsync(HttpContext.User.Identity.Name)).Id;
            model.SortOrder = ((await context.Rayons.OrderByDescending(p => p.SortOrder).FirstOrDefaultAsync())?.SortOrder ?? 0) + 1;

            context.Entry(model).State = EntityState.Added;

            try
            {
                await context.SaveChangesAsync();

                TempData["success"] = $"{name} ekleme işlemi başarıyla tamamlanmıştır.";

                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                TempData["error"] = $"{name} ekleme işlemi tamamlanamıyor. {model.Name} isimli kayıt zaten mevcut!";

                return View(model);

            }

        }

        public IActionResult Edit(int id)
        {
            return View(context.Rayons.Find(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Rayon model)
        {

            model.UserId = (await userManager.FindByNameAsync(HttpContext.User.Identity.Name)).Id;

            context.Entry(model).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();

                TempData["success"] = $"{name} güncelleme işlemi başarıyla tamamlanmıştır.";

                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                TempData["error"] = $"{name} güncelleme işlemi tamamlanamıyor. {model.Name} isimli kayıt zaten mevcut!";

                return View(model);

            }

        }


        public async Task<IActionResult> Delete(int id)
        {
            var model = await context.Rayons.FindAsync(id);
            context.Entry(model).State = EntityState.Deleted;
            try
            {
                await context.SaveChangesAsync();
                
            }
            catch (DbUpdateException)
            {
                TempData["error"] = $"{model.Name} isimli reyon ya da daha fazla kayıt ile ilişkili olduğundan silme işlemi tamamlanamıyor!";

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SortOrders()
        {
            var form = HttpContext.Request.Form;
            foreach (var item in form)
            {
                if (item.Key != "__RequestVerificationToken")
                {
                    var id = int.Parse(Regex.Split(item.Key, "_")[1]);
                    var model = await context.Rayons.FindAsync(id);
                    model.SortOrder = int.Parse(item.Value);
                    context.Entry(model).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }

            }
            return RedirectToAction("Index");
        }
    }
}
