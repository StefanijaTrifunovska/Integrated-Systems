
namespace WebApplication1.Web.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Design;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    internal sealed class Configuration : DbMigrationsConfiguration<DAL.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebApplication1.Web.DAL.AppDbContext context)
        {
            
            MoviePriceData AddMoviePrices = new MoviePriceData();
            AddMoviePrices.SeedMoviePrice(context);

            MovieData AddMovies = new MovieData();
            AddMovies.SeedMovies(context);

            ShowingData AddShowings = new ShowingData();
            AddShowings.SeedShowings(context);

            SeedIdentity Seed = new SeedIdentity();
            Seed.AddAdmin(context);

            UserData AddUsers = new UserData();
            AddUsers.SeedUsers(context);
        }
    }
}

