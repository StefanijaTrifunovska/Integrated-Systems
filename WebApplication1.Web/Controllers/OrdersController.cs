using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication1.Web.DAL;
using WebApplication1.Web.Models;

namespace WebApplication1.Web.Controllers
{
    public class OrdersController : Controller
    {
        private AppDbContext db = new AppDbContext();

        
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Manager") || User.IsInRole("Employee"))
                return View(db.Orders.ToList());
            else
            {
                String UserID = User.Identity.GetUserId();
                List<Order> Orders = db.Orders.Where(o => o.AppUser.Id == UserID).ToList();
                return View(Orders);
                
            }
        }
         [Authorize]
        public ActionResult Details(int OrderID)
        {
            Order order = db.Orders.Find(OrderID);
            if (order == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Manager") || User.IsInRole("Employee"))
                return View(order);
            else
            {
                if (order.AppUser.Id == User.Identity.GetUserId())
                    return View(order);
                else
                    return View("Error", new string[] { "This is not your Order!" });
            }
        }

       
        [Authorize]
        public ActionResult Create(int TicketID)
        {
           
            return View();
        }

       
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,ConfirmationCode,Complete,Subtotal,Total,OrderDate")] Order order, Int32 TicketID)
        {
         
            Ticket ticket = db.Tickets.Find(TicketID);
            order.Tickets.Add(ticket);

         
            order.OrderDate = DateTime.Today;
            order.Status = OrderStatus.Pending;

            order.ConfirmationCode = Utilities.GenerateNextConfirmationCode.GetNextConfirmationCode();

            AppUser user = db.Users.Find(User.Identity.GetUserId());
            order.AppUser = user;

            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Details", "Orders", new { OrderID = order.OrderID });
            }
          
            return View(order);
        }

       
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Manager") || User.IsInRole("Employee"))
                return View(order);
            else
            {
                if (order.AppUser.Id == User.Identity.GetUserId())
                    return View(order);
                else
                    return View("Error", new string[] { "This is not your Order!" });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "OrderID")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (User.IsInRole("Manager") || User.IsInRole("Employee"))
                return View(order);
            else
            {
                if (order.AppUser.Id == User.Identity.GetUserId())
                    return View(order);
                else
                    return View("Error", new string[] { "This is not your Order!" });
            }
        }

        [Authorize]
        public ActionResult Cancel(int OrderID)
        {
            Order order = db.Orders.Find(OrderID);
            if (order == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Manager") || User.IsInRole("Employee"))
                return View(order);
            else
            {
                if (order.AppUser.Id == User.Identity.GetUserId())
                    return View(order);
                else
                    return View("Error", new string[] { "This is not your Order!" });
            }
        }

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CancelConfirmed(int OrderID)
        {
            AppUser user = db.Users.Find(User.Identity.GetUserId());
            Order order = db.Orders.Find(OrderID);

            Boolean EligbleForCancellation = true;
            foreach (Ticket t in order.Tickets)
            {
                if (DateTime.Now + new TimeSpan(1, 0, 0) > t.Showing.StartTime)
                {
                    EligbleForCancellation = false;
                }
            }

            if (EligbleForCancellation)
            {
                order.Status = OrderStatus.Cancelled;
                db.Entry(order).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                db.SaveChanges();

                Utilities.EmailMessaging.SendEmail(user.Email, "Team 5: Order Cancellation Confirmation",
                            "You've successfully cancelled your order with us.\n" +
                            "This email confirms your order with confirmation number " + order.ConfirmationCode + " with " + order.Tickets.Count() + " tickets for a total of $" + order.Total + " was cancelled.");

                if (User.IsInRole("Manager") || User.IsInRole("Employee"))
                    return RedirectToAction("Index", "Orders");
                else
                {
                    if (order.AppUser.Id == User.Identity.GetUserId())
                        return RedirectToAction("Index", "Orders");
                    else
                        return View("Error", new string[] { "This is not your Order!" });
                }
            }
            else
            {
                return View("Error", new string[] { "It is too late to cancel this order since it contains a showing that starts in the next hour." });
            }


        }

        public ActionResult Checkout(int? OrderID)
        {
            if (OrderID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(OrderID);
            if (order == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Manager") || User.IsInRole("Employee"))
            {
                ViewBag.AllCreditCards = GetCreditCards();
                return View("Error", new string[] { "Only customers can purchase tickets. Please login or create a customer profile." });
            }
            else if (order.Tickets.Count() == 0)
            {
                return View("Error", new string[] { "You must have at least one ticket in your order to checkout." });
            }
            else
            {
                if (order.AppUser.Id == User.Identity.GetUserId())
                {
                    ViewBag.AllCreditCards = GetCreditCards();
                    return View(order);
                }
                else
                {
                    return View("Error", new string[] { "This is not your Order!" });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout([Bind(Include = "OrderID,CreditCard")] Int32 CreditCardID, String CreditCardInput, Int32 OrderID)
        {
            String UserID = User.Identity.GetUserId();

            Order order = db.Orders.Find(OrderID);


            if (CreditCardID != 0)
            {

                
                order.CreditCard = db.CreditCards.Find(CreditCardID);

                if (ModelState.IsValid)
                {
                    db.Entry(order).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Confirm", "Orders", new { OrderID = order.OrderID });
                }
                else return View("Error", new string[] { "An error occurred with the credit card." });
            }

          
            CreditCard creditCard = new CreditCard(CreditCardInput);


            creditCard.CardNumber = creditCard.CardNumber;
            order.CreditCard = creditCard;

            if (((order.CreditCard.CardNumber != null) &&
                (order.CreditCard.CardType != CardType.Invalid) &&
                (order.CreditCard.CardNumber.Length > 0)) &&
                ((order.CreditCard.CardType == CardType.Amex && order.CreditCard.CardNumber.Length == 15) ||
                (order.CreditCard.CardType == CardType.Visa && order.CreditCard.CardNumber.Length == 16) ||
                (order.CreditCard.CardType == CardType.MasterCard && order.CreditCard.CardNumber.Length == 16) ||
                (order.CreditCard.CardType == CardType.Discover && order.CreditCard.CardNumber.Length == 16)))
            {
                db.CreditCards.Add(creditCard);

                if (ModelState.IsValid)
                {
                    db.Entry(order).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Confirm", "Orders", new { OrderID = order.OrderID });
                }
                else
                {
                    ViewBag.CardNumberError = "Invalid Credit Card";
                    ViewBag.AllCreditCards = GetCreditCards();
                    return View(order);
                }
            }
            else
            {
                ViewBag.CardTypeError = "Invalid Credit Card Number";
                ViewBag.AllCreditCards = GetCreditCards();
                return View(order);
            }
          
        }

        public ActionResult Confirm(int? OrderID)
        {
            if (OrderID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(OrderID);

            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm([Bind(Include = "OrderID")] Int32 OrderID)
        {
            Order order = db.Orders.Find(OrderID);
            order.Status = OrderStatus.Complete;
            AppUser user = db.Users.Find(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                db.Entry(order).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                db.SaveChanges();

                Utilities.EmailMessaging.SendEmail(user.Email, "Team 5:  Order Confirmation",
                        "Thanks for ordering with us \n" +
                        "This email confirms your order with confirmation number " + order.ConfirmationCode + " with " + order.Tickets.Count() + " tickets for a total of $" + order.Total + " was completed. ");

                return RedirectToAction("Thanks", "Orders", new { OrderID = order.OrderID });

            }
            return View(order);
        }

    
        public ActionResult Thanks(int? OrderID)
        {
            if (OrderID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(OrderID);

            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        public SelectList GetCreditCards()
        {
            String UserID = User.Identity.GetUserId();
            CreditCard custom = new CreditCard("Enter a New Card");
            List<CreditCard> CreditCards = new List<CreditCard> { custom };

            SelectList AllCreditCards = new SelectList(CreditCards.OrderBy(u => u.CardNumber), "CreditCardID", "CardNumberShort");
            CreditCards.AddRange(db.CreditCards.Where(u => u.AppUser.Id == UserID).ToList());

            return AllCreditCards;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
