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
            ViewBag.CommentsCount = context.ProductComments.Where(p => !p.Enabled).Count();
            ViewBag.UsersCount = context.Users.Count();
            ViewBag.OrdersCount = context.Orders.Where(p => !p.Enabled && p.OrderState == OrderStates.New).Count();
            ViewBag.ReviewsCount = context.Products.Sum(p => p.Reviews);
            return View();
        }
    }
}
