using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebApplication1.Web.Utilities
{
    public class EmailMessaging
    {
        public static void SendEmail(String toEmailAddress, String emailSubject, String emailBody)
        {
               
            var client = new SmtpClient("smtp.gmail.com", 587) { Credentials = new NetworkCredential("Homework1@gmail.com", "Homew0Rk!"), EnableSsl = true };
            MailAddress senderEmail = new MailAddress("Homework1@gmail.com", "Team 5");
            MailMessage mm = new MailMessage();
            mm.Subject = "Hw - " + emailSubject;
            mm.Sender = senderEmail;
            mm.From = senderEmail;
            mm.To.Add(new MailAddress(toEmailAddress));
            mm.Body = emailBody;
            client.Send(mm);
        }

    }
}
