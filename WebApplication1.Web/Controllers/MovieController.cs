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
    public class MoviesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        
        public ActionResult Index(String BasicSearchString)
        {
            List<Movie> MoviesToDisplay = new List<Movie>();

            var query = from r in db.Movies select r;
            if (BasicSearchString != null)
            {
                query = query.Where(r => r.Title.Contains(BasicSearchString) || r.Tagline.Contains(BasicSearchString));
            }
            MoviesToDisplay = query.ToList();

            ViewBag.SelectedMoviesCount = MoviesToDisplay.Count();
            ViewBag.TotalMoviesCount = db.Movies.ToList().Count();

            return View(MoviesToDisplay.OrderBy(r => r.Title));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        [Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            ViewBag.AllGenres = GetAllGenres();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult Create([Bind(Include = "MovieID,Title,Overview,ReleaseDate,Revenue,Runtime,Tagline,Actors,MPAARating")] Movie movie, int[] SelectedGenres)
        {
            if (ModelState.IsValid)
            {
                foreach (int i in SelectedGenres)
                {
                    Genre gen = db.Genres.Find(i);
                    movie.Genres.Add(gen);
                    db.SaveChanges();
                }

                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AllGenres = GetAllGenres();
            return View(movie);
        }

        [Authorize(Roles = "Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            ViewBag.AllGenres = GetAllGenres();
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult Edit([Bind(Include = "MovieID,Title,Overview,ReleaseDate,Revenue,Runtime,Tagline,Actors,MPAARating")] Movie movie/*, String Title, String Overview, DateTime ReleaseDate, Int32 Revenue, Int32 Runtime, String Tagline, String Actors, MPAARating MPAARating*/, int[] SelectedGenres)
        {
            if (ModelState.IsValid)
            {
                Movie movieToChange = db.Movies.Find(movie.MovieID);
                movieToChange.Genres.Clear();

                foreach (int i in SelectedGenres)
                {
                    Genre gen = db.Genres.Find(i);
                    movieToChange.Genres.Add(gen);
                    gen.Movies.Add(movieToChange);
                }

                movieToChange.Title = movie.Title;
                movieToChange.Overview = movie.Overview;
                movieToChange.ReleaseDate = movie.ReleaseDate;
                movieToChange.Revenue = movie.Revenue;
                movieToChange.Runtime = movie.Runtime;
                movieToChange.Tagline = movie.Tagline;
                movieToChange.Actors = movie.Actors;
                movieToChange.MPAARating = movie.MPAARating;

                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AllGenres = GetAllGenres();
            return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public MultiSelectList GetAllGenres()
        {
            List<Genre> allGenres = db.Genres.OrderBy(g => g.Name).ToList();

            MultiSelectList selGenres = new MultiSelectList(allGenres, "GenreID", "Name");

            return selGenres;
        }

    }
}
