using Microsoft.AspNet.Identity;
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
    public class MovieReviewsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                var query1 = from r in db.MovieReviews select r;
                if (User.IsInRole("Customer"))
                    query1 = query1.Where(r => r.ApprovalStatus == ApprovalStatus.Approved);
                ViewBag.SelectedMovieReviewsCount = query1.Count();
                ViewBag.TotalMovieReviewsCount = db.MovieReviews.ToList().Count();
                return View(query1.OrderByDescending(mr => mr.Votes).ToList());
            }
            Movie m = db.Movies.Find(id);
            if (m == null)
            {
                return HttpNotFound();
            }

            var query = from r in db.MovieReviews select r;
            if (m != null)
            {
                query = query.Where(r => r.Movie.MovieID == id);
                query = query.Where(r => r.ApprovalStatus == ApprovalStatus.Approved);
            }
            List<MovieReview> MovieReviewsToDisplay = query.ToList();

            ViewBag.SelectedMovieReviewsCount = MovieReviewsToDisplay.Count();
            ViewBag.TotalMovieReviewsCount = db.MovieReviews.ToList().Count();

            return View(MovieReviewsToDisplay.OrderByDescending(r => r.Votes));
        }

      
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieReview movieReview = db.MovieReviews.Find(id);
            if (movieReview == null)
            {
                return HttpNotFound();
            }
            return View(movieReview);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult Upvote(int? id)
        {
            MovieReview movieReview = db.MovieReviews.Find(id);
            AppUser user = db.Users.Find(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                if (!movieReview.UsersThatUpVoted.Contains(user) && !movieReview.UsersThatDownVoted.Contains(user)) // user never voted for this
                {
                    movieReview.Votes += 1;
                    movieReview.UsersThatUpVoted.Add(user);
                    db.Entry(movieReview).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                    db.SaveChanges();
                    return View("VoteSuccess", movieReview);
                }
                else if (movieReview.UsersThatUpVoted.Contains(user))
                {
                    return View("VoteFailure", movieReview);
                }
                else 
                {
                    movieReview.Votes += 2; 
                    movieReview.UsersThatDownVoted.Remove(user);
                    movieReview.UsersThatUpVoted.Add(user);
                    db.Entry(movieReview).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                    db.SaveChanges();
                    return View("VoteSuccess", movieReview);
                }

            }
            return View(movieReview);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult Downvote(int? id)
        {
            MovieReview movieReview = db.MovieReviews.Find(id);
            AppUser user = db.Users.Find(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                if (!movieReview.UsersThatUpVoted.Contains(user) && !movieReview.UsersThatDownVoted.Contains(user)) 
                {
                    movieReview.Votes -= 1;
                    movieReview.UsersThatDownVoted.Add(user);
                    db.Entry(movieReview).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                    db.SaveChanges();
                    return View("VoteSuccess", movieReview);
                }
                else if (movieReview.UsersThatUpVoted.Contains(user)) 
                {
                    movieReview.Votes -= 2; 
                    movieReview.UsersThatUpVoted.Remove(user);
                    movieReview.UsersThatDownVoted.Add(user);
                    db.Entry(movieReview).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                    db.SaveChanges();
                    return View("VoteSuccess", movieReview);

                }
                else 
                {
                    return View("VoteFailure", movieReview);
                }

            }
            return View(movieReview);
        }

        [Authorize(Roles = "Customer")]
        
        public ActionResult Create()
        {
            ViewBag.AllMoviesList = GetAllMovies();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult Create([Bind(Include = "MovieReviewID,ReviewText,NumStars,ApprovalStatus")] MovieReview movieReview, Int32 SearchMovieID)
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            Movie thisMovie = db.Movies.Find(SearchMovieID);
            Boolean UserBoughtMovie = false;

            foreach (Order o in user.Orders)
            {
                foreach (Ticket t in o.Tickets)
                {
                    if (t.Showing.Movie.MovieID == thisMovie.MovieID)
                        UserBoughtMovie = true;
                }
            }

            if (UserBoughtMovie) 
            {
                movieReview.Movie = db.Movies.First(m => m.MovieID == SearchMovieID);
                String UserID = User.Identity.GetUserId();
                movieReview.AppUser = db.Users.First(u => u.Id == UserID);
                movieReview.ApprovalStatus = ApprovalStatus.NotApproved;

                if (ModelState.IsValid)
                {
                    db.MovieReviews.Add(movieReview);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.AllMoviesList = GetAllMovies();
                return View(movieReview);
            }
            else
            {
                return View("Error", new string[] { "You haven't bought a ticket to this Movie!" });
            }
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieReview movieReview = db.MovieReviews.Find(id);
            if (movieReview == null)
            {
                return HttpNotFound();
            }
            ViewBag.AllMoviesList = GetAllMovies();

            if (movieReview.AppUser.Id == User.Identity.GetUserId() || User.IsInRole("Manager") || User.IsInRole("Employee"))
                return View(movieReview);
            else
                return View("Error", new string[] { "This is not your Movie Review!!" });
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "MovieReviewID,ReviewText,NumStars,ApprovalStatus")] MovieReview movieReview)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movieReview).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AllMoviesList = GetAllMovies();

            if (movieReview.AppUser.Id == User.Identity.GetUserId() || User.IsInRole("Manager") || User.IsInRole("Employee"))
                return View(movieReview);
            else
                return View("Error", new string[] { "This is not your Movie Review!!" });
        }

     
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieReview movieReview = db.MovieReviews.Find(id);
            if (movieReview == null)
            {
                return HttpNotFound();
            }

            if (movieReview.AppUser.Id == User.Identity.GetUserId() || User.IsInRole("Manager") || User.IsInRole("Employee"))
                return View(movieReview);
            else
                return View("Error", new string[] { "This is not your Movie Review!!" });
        }

        // POST: MovieReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            MovieReview movieReview = db.MovieReviews.Find(id);
            db.MovieReviews.Remove(movieReview);
            db.SaveChanges();

            if (movieReview.AppUser.Id == User.Identity.GetUserId() || User.IsInRole("Manager") || User.IsInRole("Employee"))
                return RedirectToAction("Index");
            else
                return View("Error", new string[] { "This is not your Movie Review!!" });


        }

        public SelectList GetAllMovies()
        {
            List<Movie> Movies = db.Movies.ToList();

            SelectList AllMovies = new SelectList(Movies.OrderBy(m => m.Title), "MovieID", "Title");
            return AllMovies;
        }

        public ActionResult ApproveReview(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            MovieReview m = db.MovieReviews.Find(id);
            if (m == null)
            {
                return HttpNotFound();
            }
            m.ApprovalStatus = ApprovalStatus.Approved;
            db.Entry(m).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UnapproveReview(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            MovieReview m = db.MovieReviews.Find(id);
            if (m == null)
            {
                return HttpNotFound();
            }
            m.ApprovalStatus = ApprovalStatus.NotApproved;
            db.Entry(m).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
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
    }
}
