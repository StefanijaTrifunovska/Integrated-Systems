
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication1.Web.App_Start;
using WebApplication1.Web.DAL;
using WebApplication1.Web.Models;

namespace WebApplication1.Web.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private AppUserManager _userManager;
        private AppDbContext db = new AppDbContext();

        public AccountsController()
        {
        }

        public AccountsController(AppUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set
            {
                _signInManager = value;
            }
        }

        public AppUserManager UserManager
        {
            get
            {
                return UserManager1 ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
            private set
            {
                UserManager1 = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View("Error", new string[] { "Access Denied" });
            }
            AuthenticationManager.SignOut(); 
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }


        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid && ((DateTime.Now.Year - model.Birthday.Year) >= 13))
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Birthday = model.Birthday,
                    City = model.City,
                };
                var result = await UserManager.CreateAsync(user, model.Password);

                await UserManager.AddToRoleAsync(user.Id, "Customer");
                

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    Utilities.EmailMessaging.SendEmail(model.Email, "Team 5: LonghornCinema Account Creation Confirmation",
                        "Thanks for creating an account with Longhorn Cinema!\n" +
                        "This email confirms your account thas been made with email " + model.Email);

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            ViewBag.BirthdayError = "You must be at least 13 years old to create an account";
            return View(model);
        }

        public ActionResult Index()
        {
            IndexViewModel ivm = new IndexViewModel();

            String id = User.Identity.GetUserId();
            AppUser user = db.Users.Find(id);

            ivm.Email = user.Email;
            ivm.HasPassword = true;
            ivm.UserID = user.Id;
            ivm.UserName = user.UserName;


            return View(ivm);
        }



        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", "Home");
            }
            AddErrors(result);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }


         [Authorize]
        public ActionResult UserProfile()
        {
            String id = User.Identity.GetUserId();
            AppUser user = db.Users.Find(id);

            return View(user);
        }

        [Authorize]
        public ActionResult Edit()
        {
            String id = User.Identity.GetUserId();
            AppUser user = db.Users.Find(id);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Email,FirstName,LastName,Birthday,PhoneNumber,City")] AppUser user, String PhoneNumber, String StreetAddress, String City, String State, Int32 ZipCode)
        {
            String id = User.Identity.GetUserId();
            AppUser user2 = db.Users.Find(id);
            user2.PhoneNumber = PhoneNumber;
            user2.City = City;
            if (ModelState.IsValid)
            {
                db.Entry(user2).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (UserManager1 != null)
                {
                    UserManager1.Dispose();
                    UserManager1 = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region 

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public AppUserManager UserManager1 { get => _userManager; set => _userManager = value; }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion
        }
    }    

