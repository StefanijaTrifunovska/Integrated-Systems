using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Web.DAL;
using WebApplication1.Web.Models;

namespace WebApplication1.Web.Data.Migrations
{
    //add identity data
    public class SeedIdentity
    {
        public void AddAdmin(AppDbContext db)
        {
            //create a user manager and a role manager to use for this method
            AppUserManager UserManager = new AppUserManager(new UserStore<AppUser>(db));

            //create a role manager
            AppRoleManager RoleManager = new AppRoleManager(new RoleStore<AppRole>(db));

            //check to see if the manager has been added
            AppUser um1 = db.Users.FirstOrDefault(u => u.Email == "admin@example.com");

            //if manager hasn't been created, then add them
            if (um1 == null)
            {
                //TODO: Add any additional fields for user here
                um1 = new AppUser();
                um1.UserName = "admin@example.com";
                um1.FirstName = "Admin";
                um1.LastName = "Istrator";
                um1.Email = "admin@example.com";
                um1.PhoneNumber = "(512)555-5555";
                um1.Birthday = new DateTime(2000, 1, 1);
                um1.City = "Austin";
               
                var result = UserManager.Create(um1, "Abc123!");
                db.SaveChanges();
                um1 = db.Users.First(u => u.UserName == "admin@example.com");
            }

            //DONE: Add the needed roles
            //if role doesn't exist, add it
            if (RoleManager.RoleExists("Manager") == false)
            {
                RoleManager.Create(new AppRole("Manager"));
            }
            if (RoleManager.RoleExists("Employee") == false)
            {
                RoleManager.Create(new AppRole("Employee"));
            }
            if (RoleManager.RoleExists("Customer") == false)
            {
                RoleManager.Create(new AppRole("Customer"));
            }

            //make sure user is in role
            if (UserManager.IsInRole(um1.Id, "Manager") == false)
            {
                UserManager.AddToRole(um1.Id, "Manager"); //do this every time
            }

            //save changes
            db.SaveChanges();
        }

    }
}

