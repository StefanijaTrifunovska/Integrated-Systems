using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Web.Models
{
    public enum OrderStatus { Pending, Complete, Cancelled }

    public class Order
    {

        public Int32 OrderID { get; set; }

        public Int32 ConfirmationCode { get; set; }

        public OrderStatus Status { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal Subtotal
        {
            get
            {
                return Tickets.Sum(t => t.TicketPrice);
            }
        }


        [Display(Name = "Order Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OrderDate { get; set; }

        // Tickets
        public virtual List<Ticket> Tickets { get; set; }
        // Credit Card
        public virtual CreditCard CreditCard { get; set; }

        //AppUser
        public virtual AppUser AppUser { get; set; }

        public Order()
        {
            if (Tickets == null)
            {
                Tickets = new List<Ticket>();
            }
        }
    }
}
