using Microsoft.AspNetCore.Mvc;
using MVCCoreEStoreData;
using System.Linq;

namespace MVCCoreEStore.Components
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly AppDbContext context;

        public CategoryMenuViewComponent(AppDbContext context)
        {
            this.context = context;
        }

        public IViewComponentResult Invoke()
        {
            var model = context.Rayons.Where(p => p.Enabled).OrderBy(p => p.SortOrder).ToList();
            return View(model);
        }
    }
}
