using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Web.Models
{
    public enum ApprovalStatus { NotApproved, Approved }

    public class MovieReview
    {
        public Int32 MovieReviewID { get; set; }

        
        [Display(Name = "Review Text")]
        [StringLength(100, ErrorMessage = "The review must be fewer than 100 characters.")]
        public String ReviewText { get; set; }

        
        [Display(Name = "Review Rating")]
        [Range(1, 5)]
        [DisplayFormat(DataFormatString = "{0:F1}")]
        
        public Decimal NumStars { get; set; }

        public ApprovalStatus ApprovalStatus { get; set; }

        
        [Display(Name = "Net Vote")]
        [DisplayFormat(DataFormatString = "{0:F0}")]
        public Int32 Votes { get; set; }

        
        public virtual Movie Movie { get; set; }

        public virtual AppUser AppUser { get; set; }

        public virtual List<AppUser> UsersThatUpVoted { get; set; }

        public virtual List<AppUser> UsersThatDownVoted { get; set; }
    }
}
