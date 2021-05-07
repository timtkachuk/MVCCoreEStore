using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCCoreEStoreData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStore.Components
{
    public class UserBarViewComponent : ViewComponent
    {
        private readonly UserManager<User> userManager;

        public UserBarViewComponent(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            var user = User.Identity.Name != null ? userManager.FindByNameAsync(User.Identity.Name).Result : null;
            return View(user);
        }
    }
}
