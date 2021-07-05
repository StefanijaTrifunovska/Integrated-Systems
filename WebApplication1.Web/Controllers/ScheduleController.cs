using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication1.Web.DAL;
using WebApplication1.Web.Models;

namespace WebApplication1.Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ScheduleController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Scheduling(int? ShowingID)
        {

            return View();
        }
        [HttpPost]
        public ActionResult Scheduling(string TargetDate, string TargetTheatre, string CopiedDate)
        {


            DateTime copieddate = DateTime.Now;
            DateTime targetdate = DateTime.Now;
            Theatre targettheatre = Theatre.TheatreOne;
            try
            {
                copieddate = DateTime.Parse(CopiedDate);
                targetdate = DateTime.Parse(TargetDate);
                targettheatre = (Theatre)Theatre.Parse(typeof(Theatre), TargetTheatre);
            }
            catch
            {
                return RedirectToAction("Scheduling");
            }

            if (copieddate > targetdate)
            {
                return RedirectToAction("Scheduling");
            }

            var query = from r in db.Showings select r;
            query = query.Where(sh => sh.StartTime.Day == copieddate.Day);
            List<Showing> CopyShowings = query.ToList();

            TimeSpan TimeBetween = (targetdate - copieddate);
            
            int limit = CopyShowings.Count;
            var query2 = from t in db.Showings select t;
            query2 = query2.Where(sh => sh.StartTime.Day == targetdate.Day);
            List<Showing> TestShowings = query2.ToList();
            if (TestShowings.Count == 0)
            {

                for (int i = 0; i < limit; i++)
                {
                    if ((CopyShowings[i].StartTime.Day != targetdate.Day) || ((CopyShowings[i].StartTime.Month != targetdate.Month)))
                    {
                        Showing show = new Showing();
                        Showing copyshow = db.Showings.Find(CopyShowings[i].ShowingID);
                        show.SpecialEventStatus = copyshow.SpecialEventStatus;
                        show.Movie = db.Movies.Find(copyshow.Movie.MovieID);
                        show.StartTime = copyshow.StartTime + TimeBetween;
                        show.EndTime = copyshow.EndTime + TimeBetween;
                        show.TheatreNum = targettheatre;
                        db.Showings.Add(show);
                        //db.Entry(show).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                //Create a list for all the movies in date
                ViewBag.Confirm = "Dates have been successfully copied!";
                return View("Confirmation");
            }
            ViewBag.ErrorMessage = "Movies are already scheduled for this day";
            return View("Scheduling");
        }
        public ActionResult Confirmation()
        {
            String strconfirm = "Dates have been copied";
            ViewBag.Confirm = strconfirm;
            return View();
        }


    }
}
