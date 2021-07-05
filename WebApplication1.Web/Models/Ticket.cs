using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Web.Models
{
    public class Ticket
    {
        
        public Int32 TicketID { get; set; }
        public String Seat { get; set; }

        [Display(Name = "Ticket Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal TicketPrice { get; set; }

        [Display(Name = "Senior Citizen Discount")]
        public String SeniorCitizenDiscount { get; set; }

        [Display(Name = "Early Purchase Discount")]
        public String EarlyDiscount { get; set; }

        public virtual Order Order { get; set; }
        public virtual Showing Showing { get; set; }

    }
}
