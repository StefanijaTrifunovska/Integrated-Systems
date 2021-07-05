using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Web.DAL;
using WebApplication1.Web.Models;

namespace WebApplication1.Web.Utilities
{
    public class GenerateTicketPrice
    {
        public static Decimal GetTicketPrice(DateTime ShowDate)

        {
            AppDbContext db = new AppDbContext();

           
            decimal decTicketPrice;

         
            Boolean bolWeekend = false;
            Boolean bolMatinee = false; 
            Boolean bolFriday = false;
            Boolean bolTuesday = false; 
            Boolean bolBefore5 = false; 

            MoviePrice movieprice = db.MoviePrices.FirstOrDefault(x => x.MoviePriceID == 1);

            Decimal decMoviePriceMat = movieprice.decMatineePrice;
            Decimal decMoviePriceWeek = movieprice.decWeekPrice;
            Decimal decMoviePriceWeeknd = movieprice.decWeekendPrice;
            Decimal decMoviePriceTues = movieprice.decTuesdayPrice;


            String strday = ShowDate.DayOfWeek.ToString();
            Int32 inthour = ShowDate.Hour;

            if (strday == "Friday")
            {
                bolFriday = true;
            }

            if ((strday == "Saturday") || (strday == "Sunday"))
            {
                bolWeekend = true;
            }

            if (strday == "Tuesday")
            {
                bolTuesday = true;
            }

            if (inthour < 12)
            {
                bolMatinee = true;
            }
            else if (inthour < 17)
            {
                bolBefore5 = true;
            }

     
            if ((bolTuesday) && (bolBefore5))  
            {
                decTicketPrice = decMoviePriceTues;
            }
            else if ((bolWeekend) || (bolFriday && !(bolMatinee)))
            {
                decTicketPrice = decMoviePriceWeeknd;
            }
            else if (bolMatinee) 
            {
                decTicketPrice = decMoviePriceMat;
            }
            else 
            {
                decTicketPrice = decMoviePriceWeek;
            }


            return decTicketPrice;

        }
    }
}
