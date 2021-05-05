using Microsoft.AspNetCore.Mvc;
using MVCCoreEStoreData;
using System.Linq;

namespace MVCCoreEStore.Components
{
    public class BrandsBarViewComponent : ViewComponent
    {
        private readonly AppDbContext context;

        public BrandsBarViewComponent(AppDbContext context)
        {
            this.context = context;
        }

        public IViewComponentResult Invoke()
        {
            var model = context.Brands.Where(p => p.Enabled).ToList();
            return View(model);
        }

    }
}
