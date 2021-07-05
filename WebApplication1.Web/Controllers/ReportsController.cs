using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication1.Web.DAL;
using WebApplication1.Web.Models;

namespace WebApplication1.Web.Controllers
{
    public class ReportsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Query() 
        {
            ViewBag.AllMovies = GetAllMovies();
            ViewBag.AllCustomers = GetAllCustomers();
            return View();
        }

        [HttpPost]

        public ActionResult Query([Bind(Include = "ReportID,StartDate,EndDate,DisplaySeats,DisplayRevenue,MovieFilter,RatingFilter,CustomerFilter")] Report report, Int32 MovieID, String CustomerFilterID)
        {
            if (CustomerFilterID != "0")
            {
                AppUser FilterUser = db.Users.Find(CustomerFilterID);
                report.CustomerFilter = FilterUser;
            }
            if (MovieID != 0)
            {
                Movie movie = db.Movies.Find(MovieID);
                report.MovieFilter = movie;
            }




            var query = from t in db.Tickets select t;
            if (report.StartDate != null)
                query = query.Where(t => t.Order.OrderDate > report.StartDate);
            if (report.EndDate != null)
                query = query.Where(t => t.Order.OrderDate < report.EndDate);
            if (MovieID != 0)
                query = query.Where(t => t.Showing.Movie.MovieID == MovieID);
            if (report.MovieFilter != null && MovieID != 0)
                query = query.Where(t => t.Showing.Movie.MovieID == report.MovieFilter.MovieID);
            if (report.RatingFilter != MPAARating.None)
                query = query.Where(t => t.Showing.Movie.MPAARating == report.RatingFilter);
            if (CustomerFilterID != null && CustomerFilterID != "0")
                query = query.Where(t => t.Order.AppUser.Id == CustomerFilterID.ToString());

            int intNumSeats = query.Count();
            Decimal decRevenue = 0;

            if (query.Count() > 0)
                decRevenue = query.Sum(t => t.TicketPrice);

            String TheCustomerFirstName; String TheCustomerLastName;
            if (report.CustomerFilter != null)
            {
                TheCustomerFirstName = report.CustomerFilter.FirstName;
                TheCustomerLastName = report.CustomerFilter.LastName;
            }
            else
            {
                TheCustomerFirstName = "All";
                TheCustomerLastName = "Customers";
            }

            String TheMovieTitle;
            if (report.MovieFilter != null)
                TheMovieTitle = report.MovieFilter.Title;
            else
                TheMovieTitle = "All Movies";

            if (ModelState.IsValid)
            {
                db.Reports.Add(report);
                db.SaveChanges();
                return RedirectToAction("Results", new { ReportID = report.ReportID, NumSeats = query.Count(), Revenue = decRevenue, CustomerName = TheCustomerFirstName + " " + TheCustomerLastName, MovieName = TheMovieTitle });
            }

            ViewBag.AllMovies = GetAllMovies();
            ViewBag.AllCustomers = GetAllCustomers();

            return View(report);
        }

        public ActionResult Results(int ReportID, int NumSeats, Decimal Revenue, String CustomerName, String MovieName)
        {
            Report report = db.Reports.Find(ReportID);
            if (report == null)
            {
                return HttpNotFound();
            }

            ViewBag.NumSeats = NumSeats;
            ViewBag.Revenue = Revenue;
            ViewBag.CustomerName = CustomerName;
            ViewBag.MovieName = MovieName;

            return View(report);

        }

        public SelectList GetAllMovies()
        {
            Movie custom = new Movie("All Movies");
            List<Movie> Movies = new List<Movie> { custom };
            SelectList AllMovies = new SelectList(Movies.OrderBy(m => m.MovieID), "MovieID", "Title");
            Movies.AddRange((IEnumerable<Movie>)db.Movies);

            return AllMovies;
        }

        public SelectList GetAllCustomers()
        {


            SelectListItem s = new SelectListItem() { Text = "All Customers", Value = "0" };

            List<SelectListItem> ItemList = new List<SelectListItem>();
            ItemList.Add(s);
            List<AppUser> Customers = db.Users.ToList();
            foreach (AppUser c in Customers)
            {
                ItemList.Add(new SelectListItem() { Text = c.FirstName + " " + c.LastName, Value = c.Id });
            }


            return new SelectList(ItemList, "Value", "Text", null);
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

