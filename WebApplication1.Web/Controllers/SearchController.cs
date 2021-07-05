using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication1.Web.DAL;
using WebApplication1.Web.Models;

namespace WebApplication1.Web.Controllers
{
    public enum Classification { GreaterThan, LessThan }
    public enum StarFilter { Greater, Less }
    public enum YearFilter { After, Before }

    public class SearchController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return Content("Bad Request");
            }

            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return Content("The resource you are looking for has been removed, had its name changed, or is temporarily unavailable.");
            }
            return View(movie);
        }

        public ActionResult DetailedSearch()
        {
            ViewBag.AllGenres = GetAllGenres();
            ViewBag.AllMPAARatings = GetAllMPAARatings();
            return View();
        }

        public ActionResult DisplaySearchResults(String SearchTitle, String SearchTagline, Int32 SearchGenreID, MPAARating SearchMPAARating, String SearchStars, StarFilter SearchStarFilter, String SearchActor, YearFilter SearchYearFilter, String SearchYear)
        {
            var query = from m in db.Movies
                        select m;
            Boolean SomethingWrong = false;

            if (SearchTitle != null)
            {
                query = query.Where(m => m.Title.Contains(SearchTitle));
            }

           
            if (SearchTagline != null)
            {
                query = query.Where(m => m.Tagline.Contains(SearchTagline));
            }

           
            if (SearchGenreID != 0)
            {
              
                query = query.Where(m => m.Genres.Any(g => g.GenreID == SearchGenreID));


            } 


          
            if (SearchYear != null && SearchYear != "")
            {
                DateTime dtSearchYear;
                try
                {
                    dtSearchYear = new DateTime(Convert.ToInt32(SearchYear), 1, 1);
                }
                catch
                {
                    ViewBag.YearMessage = SearchYear + " is not a valid year. Please try again";

                    ViewBag.AllGenres = GetAllGenres();
                    ViewBag.AllMPAARatings = GetAllMPAARatings();

                    SomethingWrong = true;
                    dtSearchYear = DateTime.Now;

                }

          
                if (SearchYearFilter == YearFilter.After && !SomethingWrong)
                {
                    System.Collections.IList list = query.ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        Movie m = (Movie)list[i];
                        if (m.ReleaseDate < dtSearchYear)
                            query = query.Where(x => x.Title != m.Title);
                    }
                }
                else if (SearchYearFilter == YearFilter.Before)
                {
                    System.Collections.IList list = query.ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        Movie m = (Movie)list[i];
                        if (m.ReleaseDate > dtSearchYear)
                            query = query.Where(x => x.Title != m.Title);
                    }
                }
            }




            if (SearchMPAARating != MPAARating.None)
            {
                query = query.Where(m => m.MPAARating == SearchMPAARating);
            }


            if (SearchStars != null && SearchStars != "")
            {
                Decimal decCustomerRating;
                try
                {
                    decCustomerRating = Convert.ToDecimal(SearchStars);
                }
                catch
                {
                    ViewBag.Message = SearchStars + " is not a valid number. Please try again";

                    ViewBag.AllGenres = GetAllGenres();
                    ViewBag.AllMPAARatings = GetAllMPAARatings();

                    SomethingWrong = true;
                    decCustomerRating = -1;
                }

                if (SearchStarFilter == StarFilter.Greater && !SomethingWrong)
                {
                    System.Collections.IList list = query.ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        Movie m = (Movie)list[i];
                        if (m.RatingAverage < decCustomerRating)
                            query = query.Where(x => x.Title != m.Title);
                    }
                }
                else if (SearchStarFilter == StarFilter.Less)
                {
                    System.Collections.IList list = query.ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        Movie m = (Movie)list[i];
                        if (m.RatingAverage > decCustomerRating)
                            query = query.Where(x => x.Title != m.Title);
                    }
                }
            }


            if (SearchActor != null)
            {
                query = query.Where(m => m.Actors.Contains(SearchActor));
            }

            List<Movie> MoviesToDisplay = query.ToList();

            MoviesToDisplay.OrderBy(m => m.Title);

            ViewBag.SelectedMoviesCount = MoviesToDisplay.Count();
            ViewBag.TotalMoviesCount = db.Movies.ToList().Count();

            if (!SomethingWrong)
                return View("~/Views/Movies/Index.cshtml", MoviesToDisplay);
            else
                return View("DetailedSearch");
        }

        public SelectList GetAllGenres()
        {
            List<Genre> Genres = db.Genres.ToList();

            Genre SelectNone = new Genre("None") { GenreID = 0, Name = "All Genres" };
            Genres.Add(SelectNone);

            SelectList AllGenres = new SelectList(Genres.OrderBy(g => g.GenreID), "GenreID", "Name");
            return AllGenres;
        }

        public SelectList GetAllMPAARatings()
        {
            List<MPAARating> MPAARatings = new List<MPAARating>();

            MPAARating SelectNone = MPAARating.None; 
            MPAARatings.Add(SelectNone);

            System.Collections.IList list = db.Movies.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                Movie m = (Movie)list[i];
                if (!MPAARatings.Contains(m.MPAARating))
                    MPAARatings.Add(m.MPAARating);
            }

            SelectList AllRatings = new SelectList(MPAARatings);
            return AllRatings;
        }

    }
}
