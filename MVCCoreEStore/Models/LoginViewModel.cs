using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStore
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        [Display (Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
