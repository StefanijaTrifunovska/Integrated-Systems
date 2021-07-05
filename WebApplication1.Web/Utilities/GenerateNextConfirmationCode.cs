using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Web.DAL;

namespace WebApplication1.Web.Utilities
{
    public class GenerateNextConfirmationCode
    {
        public static Int32 GetNextConfirmationCode()

        {

            AppDbContext db = new AppDbContext();


            Int32 intMaxConfirmationCode; 

            Int32 intNextConfirmationCode;



            if (db.Orders.Count() == 0) 

            {

                intMaxConfirmationCode = 10000;

            }

            else

            {

                intMaxConfirmationCode = db.Orders.Max(c => c.ConfirmationCode); 

            }


            intNextConfirmationCode = intMaxConfirmationCode + 1;



            return intNextConfirmationCode;

        }
    }
}
