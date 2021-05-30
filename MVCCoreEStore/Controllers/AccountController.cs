using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCCoreEStore.Models;
using MVCCoreEStore.Services;
using MVCCoreEStoreData;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStore.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly IMailMessageService mailMessageService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AppDbContext context;
        private readonly IShoppingCartService shoppingCartService;

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IMailMessageService mailMessageService,
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            AppDbContext context,
            IShoppingCartService shoppingCartService
            )
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.mailMessageService = mailMessageService;
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
            this.shoppingCartService = shoppingCartService;
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
            await userManager.AddToRoleAsync(newUser, "Members");

            if (result.Succeeded)
            {
                var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var messageBody =
                    string.Format(
                    System.IO
                    .File
                    .ReadAllText(System.IO.Path.Combine(webHostEnvironment.WebRootPath, "Content", "EMailConfirmationTemplate.html"))
                    , model.Name
                    , Url.Action("ConfirmEmail", "Account", new { id = newUser.Id, token = emailConfirmationToken }, httpContextAccessor.HttpContext.Request.Scheme)
                    );

                await mailMessageService.Send(model.UserName, "E-Posta Doğrulama Mesajı", messageBody);
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
        public async Task<IActionResult> ConfirmEmail(int id, string token)
        {
            var user = context.Users.Find(id);
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View("EMailSuccess");
            }
            else
            {
                return View("EmailFail");
            }
        }

        public IActionResult ResetPasswordForm()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPasswordForm(ResetPasswordViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                var messageBody =
                    string.Format(
                    System.IO
                    .File.ReadAllText(System.IO.Path.Combine(webHostEnvironment.WebRootPath, "Content", "ResetPasswordTemplate.html"))
                    , user.Name
                    , Url.Action("ResetPassword", "Account", new { id = user.Id, token = passwordResetToken }, httpContextAccessor.HttpContext.Request.Scheme)
                    );
                await mailMessageService.Send(user.UserName, "Parola Yenileme Mesajı", messageBody);
                return View("ResetPasswordFormSuccess");
            }
            else
            {
                ModelState.AddModelError("", "Belirttiğiniz e-posta adresiyle kayıtlı üyemiz bulunmamaktadır!");
                return View(model);
            }
        }

        public IActionResult ResetPassword(string id, string token)
        {
            return View(new NewPasswordViewModel { Id = id, Token = token });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(NewPasswordViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            return View("ResetPasswordSuccess");
        }

        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            await shoppingCartService.TransferCookieToDatabaseAsync();
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user.ShoppingCartItems.Any(p => p.Product == null))
            {
                return RedirectToAction("CheckOut");
            }
            return View(user);
        }
    }
}
