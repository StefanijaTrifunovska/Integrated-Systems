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
    public class ShowingsController : Controller
    {
        private AppDbContext db = new AppDbContext();

      
        public ActionResult Index(int? id)
        {
            if (id == null || id == -1)
            {
                return View(db.Showings.ToList());
            }
            if (id == 0)
            {
            int Day = DateTime.Now.Day;
                return View(db.Showings.Where(u => u.StartTime.Day == Day).ToList());
            }
            Movie m = db.Movies.Find(id);
            if (m == null)
            {
                return HttpNotFound();
            }

            var query = from r in db.Showings select r;
            if (m != null)
            {
                query = query.Where(r => r.Movie.MovieID == id);
            }
            List<Showing> ShowingsToDisplay = query.ToList();

            ViewBag.SelectedShowingsCount = ShowingsToDisplay.Count();
            ViewBag.TotalMovieShowingsCount = db.Showings.ToList().Count();

            return View(ShowingsToDisplay.OrderBy(r => r.StartTime));
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Showing showing = db.Showings.Find(id);
            if (showing == null)
            {
                return HttpNotFound();
            }
            return View(showing);
        }


        [Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            ViewBag.AllMoviesList = GetAllMovies();
            return View();
        }

      [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult Create([Bind(Include = "ShowingID,StartTime,SpecialEvent,TheatreNum,SeatList,MovieID")] Showing showing, Int32 SearchMovieID)
        {

            if (ModelState.IsValid && showing.StartTime > DateTime.Now)
            {
                Movie m = db.Movies.FirstOrDefault(x => x.MovieID == SearchMovieID);
                showing.EndTime = showing.StartTime.AddMinutes(m.Runtime);
                showing.Movie = m;
                db.Showings.Add(showing);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AllMoviesList = GetAllMovies();
            ViewBag.ShowingInPastError = "You've scheduled this Showing in the past. Pick a future start time.";
            return View(showing);
        }

         [Authorize(Roles = "Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Showing showing = db.Showings.Find(id);
            if (showing == null)
            {
                return HttpNotFound();
            }
            ViewBag.AllMoviesList = GetAllMovies();
            return View(showing);
        }

      [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult Edit([Bind(Include = "ShowingID,StartTime,SpecialEvent,TheatreNum,SeatList")] Showing showing)
        {
            if (ModelState.IsValid)
            {
                db.Entry(showing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(showing);
        }

     
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Showing showing = db.Showings.Find(id);
            if (showing == null)
            {
                return HttpNotFound();
            }
            return View(showing);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Showing showing = db.Showings.Find(id);


            foreach (Ticket tic in showing.Tickets)
            {
                Utilities.EmailMessaging.SendEmail(tic.Order.AppUser.Email, "Team 5: Showing Cancellation", "We apologize but we cancelled your showing, " + showing.Movie.Title);
            }


            db.Showings.Remove(showing);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public SelectList GetAllMovies()
        {
            List<Movie> Movies = db.Movies.ToList();

            SelectList AllMovies = new SelectList(Movies.OrderBy(m => m.Title), "MovieID", "Title");
            return AllMovies;

        }

        public ActionResult CopyMovies(int id)
        {

            return View();
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
