using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication1.Web.Models
{
    public class AppUser : IdentityUser
    {
        
        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public String FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public String LastName { get; set; }

        [Required(ErrorMessage = "Birthday is required")]
        [Display(Name = "Birthday")]
        public DateTime Birthday { get; set; }

        
        [Required(ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public String City { get; set; }

        // Orders
        public virtual List<Order> Orders { get; set; }
        // MovieReviews
        public virtual List<MovieReview> MovieReviews { get; set; }
        // CreditCards
        public virtual List<CreditCard> CreditCards { get; set; }

        public AppUser()
        {
            if (Orders == null)
            {
                Orders = new List<Order>();
            }

            if (MovieReviews == null)
            {
                MovieReviews = new List<MovieReview>();
            }

            if (CreditCards == null)
            {
                CreditCards = new List<CreditCard>();
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}
