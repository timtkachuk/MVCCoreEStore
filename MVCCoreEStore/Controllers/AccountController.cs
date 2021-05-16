using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCCoreEStore.Models;
using MVCCoreEStore.Services;
using MVCCoreEStoreData;
using System.Threading.Tasks;

namespace MVCCoreEStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly IMailMessageService mailMessageService;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IMailMessageService mailMessageService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.mailMessageService = mailMessageService;
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel { RememberMe = true, ReturnUrl = HttpContext.Request.Query["ReturnUrl"] });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            if (result.Succeeded)
                return Redirect(model.ReturnUrl ?? "/");
            else
            {
                ModelState.AddModelError("", "Geçersiz kullanıcı girişi");
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel { });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var newUser = new User
            {
                UserName = model.UserName,
                Name = model.Name,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender
            };
            var result = await userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                await mailMessageService.Send(model.UserName, "E-Posta Doğrulama Mesajı", "deneme : Link");
                return View("RegisterSuccess");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    switch (error.Code)
                    {
                        case "DuplicateUserName":
                            ModelState.AddModelError("", "E-posta adresi zaten kayıtlı");
                            break;
                        case "PasswordTooShort":
                            ModelState.AddModelError("", "Parolanız en az 6(altı) karakter uzunluğunda olmalıdır");
                            break;
                        case "PasswordRequiresNonAlphaNumeric":
                            ModelState.AddModelError("", "Parolanız en az bir adet sembol içermelidir");
                            break;
                        case "PasswordRequiresLower":
                            ModelState.AddModelError("", "Parolanız en az bir adet küçük harf içermelidir");
                            break;
                        case "PasswordRequiresUpper":
                            ModelState.AddModelError("", "Parolanız en az bir adet büyük harf içermelidir");
                            break;
                        case "PasswordRequiresDigit":
                            ModelState.AddModelError("", "Parolanız en az bir adet rakam içermelidir");
                            break;

                    }
                }
                return View(model);
            }
        }
    }
}
