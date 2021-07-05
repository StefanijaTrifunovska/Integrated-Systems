using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Web.DAL;
using WebApplication1.Web.Models;

namespace WebApplication1.Web.Data.Migrations
{
    public class MoviePriceData
    {
        public void SeedMoviePrice(AppDbContext db)
        {
            MoviePrice mp1 = new MoviePrice();
            mp1.decMatineePrice = 5;
            mp1.decTuesdayPrice = 8;
            mp1.decWeekendPrice = 12;
            mp1.decWeekPrice = 10;

            db.MoviePrices.AddOrUpdate(mp => mp.decMatineePrice, mp1);
            db.SaveChanges();
        }
    }
}
