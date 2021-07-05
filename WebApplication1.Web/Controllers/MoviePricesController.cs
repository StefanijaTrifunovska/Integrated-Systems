using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication1.Web.DAL;
using WebApplication1.Web.Models;

namespace WebApplication1.Web.Controllers
{
    public class MoviePricesController : Controller
    {
        private AppDbContext db = new AppDbContext();


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MoviePrice moviePrice = db.MoviePrices.Find(id);
            if (moviePrice == null)
            {
                return HttpNotFound();
            }
            return View(moviePrice);
        }


        public ActionResult Edit()
        {
            MoviePrice moviePrice = db.MoviePrices.Find(2);
            if (moviePrice == null)
            {
                return HttpNotFound();
            }
            return View(moviePrice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MoviePriceID,decMatineePrice,decTuesdayPrice,decWeekendPrice,decWeekPrice")] MoviePrice moviePrice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(moviePrice).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Movies");
            }
            return View(moviePrice);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
