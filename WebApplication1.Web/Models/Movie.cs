using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Web.DAL;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Web.Models
{
    public enum MPAARating { G, PG, R, Unrated, PG13, NC17, None };

    public class Movie
    {
        private AppDbContext db = new AppDbContext();

        public Int32 MovieID { get; set; }

        [Display(Name = "Movie Title")]
        public String Title { get; set; }

        public String Overview { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        public Int64 Revenue { get; set; }

        public Int32 Runtime { get; set; }
        
        public String Tagline { get; set; }

        public String Actors { get; set; }

        [Display(Name = "MPAA Rating")]
        public MPAARating MPAARating { get; set; }


        [DisplayFormat(DataFormatString = "{0:0.#}")]
        public Decimal RatingAverage
        {
            get
            {
                if (MovieReviews.Count() == 0)
                    return 0;
                else
                    return MovieReviews.Average(mr => mr.NumStars);
            }
        }

        public String GenresString
        {
            get
            {
                String ret = "";
                foreach (Genre g in Genres)
                {
                    ret += g.Name + ", ";
                }
                if (ret.Length < 2) return ret; 
                return ret.Substring(0, ret.Length - 2); 
            }
        }

          public virtual List<MovieReview> MovieReviews { get; set; }


        public virtual List<Showing> Showings { get; set; }


        public virtual List<Genre> Genres { get; set; }

        public Movie()
        {
            if (Genres == null)
            {
                Genres = new List<Genre>();
            }

            if (MovieReviews == null)
            {
                MovieReviews = new List<MovieReview>();
            }

            if (Showings == null)
            {
                Showings = new List<Showing>();
            }
        }

        public Movie(String MovieTitle)
        {
            if (Genres == null)
            {
                Genres = new List<Genre>();
            }

            if (MovieReviews == null)
            {
                MovieReviews = new List<MovieReview>();
            }

            if (Showings == null)
            {
                Showings = new List<Showing>();
            }

            Title = MovieTitle;
        }
    }
}
