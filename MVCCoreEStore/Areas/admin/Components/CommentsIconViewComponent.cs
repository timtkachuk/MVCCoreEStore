using Microsoft.AspNetCore.Mvc;
using MVCCoreEStoreData;
using System.Linq;

namespace MVCCoreEStore.Areas.admin.Components
{
    public class CommentsIconViewComponent : ViewComponent
    {
        private readonly AppDbContext context;

        public CommentsIconViewComponent(AppDbContext context)
        {
            this.context = context;
        }

        public IViewComponentResult Invoke()
        {
            var model = context.ProductComments.Where(p => !p.Enabled).ToList();
            return View(model);
        }

    }
}
