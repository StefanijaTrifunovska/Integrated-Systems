using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Web.Models
{
    public enum Time { Morning, Afternoon, Evening }
    public class MoviePrice
    {
        public Int32 MoviePriceID { get; set; }
        public Decimal decMatineePrice { get; set; }
        public Decimal decTuesdayPrice { get; set; }
        public Decimal decWeekendPrice { get; set; }
        public Decimal decWeekPrice { get; set; }
    }
}
