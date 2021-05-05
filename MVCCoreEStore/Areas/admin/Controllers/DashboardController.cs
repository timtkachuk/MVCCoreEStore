using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCCoreEStoreData;
using System.Linq;

namespace MVCCoreEStore.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Administrators, ProductAdministrators, OrderAdministrators")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext context;

        public DashboardController(AppDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Comments = context.ProductComments.Where(p => !p.Enabled).ToList();
            return View();
        }
    }
}
