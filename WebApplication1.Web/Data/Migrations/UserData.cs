using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Web.DAL;
using WebApplication1.Web.Models;

namespace WebApplication1.Web.Data.Migrations
{
    public class UserData
    {
        public void SeedUsers(AppDbContext db)
        {
            //create a user manager and a role manager to use for this method
            AppUserManager UserManager = new AppUserManager(new UserStore<AppUser>(db));

            AppUser c1 = db.Users.FirstOrDefault(u => u.Email == "cbaker@example.com");
            if (c1 == null)
            {
                c1 = new AppUser();
                c1.UserName = "cbaker@example.com";
                c1.LastName = "Baker";
                c1.FirstName = "Christopher";
                c1.Email = "cbaker@example.com";
                c1.Birthday = new DateTime(1949, 11, 23);
                c1.City = "Austin";
                c1.PhoneNumber = "5125550180";
                var result = UserManager.Create(c1, "hello1");
                db.SaveChanges();
                c1 = db.Users.First(u => u.UserName == "cbaker@example.com");
            }

            if (UserManager.IsInRole(c1.Id, "Customer") == false)
            {
                UserManager.AddToRole(c1.Id, "Customer");
            }

            db.SaveChanges();



            AppUser c2 = db.Users.FirstOrDefault(u => u.Email == "banker@longhorn.net");
            if (c2 == null)
            {
                c2 = new AppUser();
                c2.UserName = "banker@longhorn.net";
                c2.LastName = "Banks";
                c2.FirstName = "Michelle";
                c2.Email = "banker@longhorn.net";
                c2.Birthday = new DateTime(1962, 11, 27);
                c2.City = "Austin";
                c2.PhoneNumber = "5125550183";
                var result = UserManager.Create(c2, "potato");
                db.SaveChanges();
                c2 = db.Users.First(c => c.UserName == "banker@longhorn.net");
            }
            if (UserManager.IsInRole(c2.Id, "Customer") == false)
            {
                UserManager.AddToRole(c2.Id, "Customer");
            }
            db.SaveChanges();

            AppUser c3 = db.Users.FirstOrDefault(u => u.Email == "franco@example.com");
            if (c3 == null)
            {
                c3 = new AppUser();
                c3.UserName = "franco@example.com";
                c3.LastName = "Broccolo";
                c3.FirstName = "Franco";
                c3.Email = "franco@example.com";
                c3.Birthday = new DateTime(1992, 10, 11);
                c3.City = "Austin";
                c3.PhoneNumber = "5125550128";
                var result = UserManager.Create(c3, "painting");
                db.SaveChanges();
                c3 = db.Users.First(c => c.UserName == "franco@example.com");
            }
            if (UserManager.IsInRole(c3.Id, "Customer") == false)
            {
                UserManager.AddToRole(c3.Id, "Customer");
            }
            db.SaveChanges();

            AppUser c4 = db.Users.FirstOrDefault(u => u.Email == "wchang@example.com");
            if (c4 == null)
            {
                c4 = new AppUser();
                c4.UserName = "wchang@example.com";
                c4.LastName = "Chang";
                c4.FirstName = "Wendy";
                c4.Email = "wchang@example.com";
                c4.Birthday = new DateTime(1997, 5, 16);
                c4.City = "Round Rock";
                c4.PhoneNumber = "5125550133";
                var result = UserManager.Create(c4, "texas1");
                db.SaveChanges();
                c4 = db.Users.First(c => c.UserName == "wchang@example.com");
            }
            if (UserManager.IsInRole(c4.Id, "Customer") == false)
            {
                UserManager.AddToRole(c4.Id, "Customer");
            }
            db.SaveChanges();

            AppUser c5 = db.Users.FirstOrDefault(u => u.Email == "limchou@gogle.com");
            if (c5 == null)
            {
                c5 = new AppUser();
                c5.UserName = "limchou@gogle.com";
                c5.LastName = "Chou";
                c5.FirstName = "Lim";
                c5.Email = "limchou@gogle.com";
                c5.Birthday = new DateTime(1970, 4, 6);
                c5.City = "Austin";
                c5.PhoneNumber = "5125550102";
                var result = UserManager.Create(c5, "Anchorage");
                db.SaveChanges();
                c5 = db.Users.First(c => c.UserName == "limchou@gogle.com");
            }
            if (UserManager.IsInRole(c5.Id, "Customer") == false)
            {
                UserManager.AddToRole(c5.Id, "Customer");
            }
            db.SaveChanges();

            AppUser c6 = db.Users.FirstOrDefault(u => u.Email == "shdixon@aoll.com");
            if (c6 == null)
            {
                c6 = new AppUser();
                c6.UserName = "shdixon@aoll.com";
                c6.LastName = "Dixon";
                c6.FirstName = "Shan";
                c6.Email = "shdixon@aoll.com";
                c6.Birthday = new DateTime(1984, 1, 12);
                c6.City = "Austin";
                c6.PhoneNumber = "5125550146";
                var result = UserManager.Create(c6, "pepperoni");
                db.SaveChanges();
                c6 = db.Users.First(c => c.UserName == "shdixon@aoll.com");
            }
            if (UserManager.IsInRole(c6.Id, "Customer") == false)
            {
                UserManager.AddToRole(c6.Id, "Customer");
            }
            db.SaveChanges();

            AppUser c7 = db.Users.FirstOrDefault(u => u.Email == "j.b.evans@aheca.org");
            if (c7 == null)
            {
                c7 = new AppUser();
                c7.UserName = "j.b.evans@aheca.org";
                c7.LastName = "Evans";
                c7.FirstName = "Jim Bob";
                c7.Email = "j.b.evans@aheca.org";
                c7.Birthday = new DateTime(1959, 9, 9);
                c7.City = "Georgetown";
                c7.PhoneNumber = "5125550170";
                var result = UserManager.Create(c7, "longhorns");
                db.SaveChanges();
                c7 = db.Users.First(c => c.UserName == "j.b.evans@aheca.org");
            }
            if (UserManager.IsInRole(c7.Id, "Customer") == false)
            {
                UserManager.AddToRole(c7.Id, "Customer");
            }
            db.SaveChanges();

            AppUser c8 = db.Users.FirstOrDefault(u => u.Email == "feeley@penguin.org");
            if (c8 == null)
            {
                c8 = new AppUser();
                c8.UserName = "feeley@penguin.org";
                c8.LastName = "Feeley";
                c8.FirstName = "Lou Ann";
                c8.Email = "feeley@penguin.org";
                c8.Birthday = new DateTime(2001, 1, 12);
                c8.City = "Austin";
                c8.PhoneNumber = "5125550105";
                var result = UserManager.Create(c8, "aggies");
                db.SaveChanges();
                c8 = db.Users.First(c => c.UserName == "feeley@penguin.org");
            }
            if (UserManager.IsInRole(c8.Id, "Customer") == false)
            {
                UserManager.AddToRole(c8.Id, "Customer");
            }
            db.SaveChanges();

            AppUser c9 = db.Users.FirstOrDefault(u => u.Email == "tfreeley@minnetonka.ci.us");
            if (c9 == null)
            {
                c9 = new AppUser();
                c9.UserName = "tfreeley@minnetonka.ci.us";
                c9.LastName = "Freeley";
                c9.FirstName = "Tesa";
                c9.Email = "tfreeley@minnetonka.ci.us";
                c9.Birthday = new DateTime(1991, 2, 4);
                c9.City = "Horseshoe Bay";
                c9.PhoneNumber = "5125550114";
                var result = UserManager.Create(c9, "raiders");
                db.SaveChanges();
                c9 = db.Users.First(c => c.UserName == "tfreeley@minnetonka.ci.us");
            }
            if (UserManager.IsInRole(c9.Id, "Customer") == false)
            {
                UserManager.AddToRole(c9.Id, "Customer");
            }
            db.SaveChanges();

            AppUser c10 = db.Users.FirstOrDefault(u => u.Email == "mgarcia@gogle.com");
            if (c10 == null)
            {
                c10 = new AppUser();
                c10.UserName = "mgarcia@gogle.com";
                c10.LastName = "Garcia";
                c10.FirstName = "Margaret";
                c10.Email = "mgarcia@gogle.com";
                c10.Birthday = new DateTime(1991, 10, 2);
                c10.City = "Austin";
                c10.PhoneNumber = "5125550155";
                var result = UserManager.Create(c10, "mustangs");
                db.SaveChanges();
                c10 = db.Users.First(c => c.UserName == "mgarcia@gogle.com");
            }
            if (UserManager.IsInRole(c10.Id, "Customer") == false)
            {
                UserManager.AddToRole(c10.Id, "Customer");
            }
            db.SaveChanges();
        }
    }
}
