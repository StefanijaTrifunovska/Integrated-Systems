using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Web.DAL;
using WebApplication1.Web.Models;

namespace WebApplication1.Web.App_Start
{
    public void Configuration(IAppBuilder app)
    {
        
        app.CreatePerOwinContext(AppDbContext.Create);
        app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
        app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

        app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);

        app.UseCookieAuthentication(new CookieAuthenticationOptions
        {
            AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,

         
            LoginPath = new PathString("/Accounts/Login"),
            Provider = new CookieAuthenticationProvider
            {
         
                OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager, AppUser>(
                    validateInterval: TimeSpan.FromMinutes(30),
                    regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
            }
        });
        app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);


    }

}
