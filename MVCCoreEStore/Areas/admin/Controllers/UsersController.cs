using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCCoreEStoreData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStore.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles ="Administrators")]
    public class UsersController : Controller
    {
        private readonly AppDbContext context;

        public UsersController(
            AppDbContext context
            )
            
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Roles"] = new SelectList(await context.Roles.ToListAsync(), "Name", "FriendlyName");
            var model = await context.Users.OrderBy(p => p.Name).ToListAsync();
            return View(model);
        }
    }
}
