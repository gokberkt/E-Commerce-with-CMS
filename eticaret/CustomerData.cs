using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eticaret.Models;

namespace eticaret
{
    public class CustomerData
    {
       public static Customers Info
        {
            get
            {
                return HttpContext.Current.Session["CustomerData"] as Customers;
            }
            set
            {
                HttpContext.Current.Session["CustomerData"] = value;
            }
        }


        public static Admins AdminInfo
        {
            get
            {
                return HttpContext.Current.Session["AdminData"] as Admins;
            }
            set
            {
                HttpContext.Current.Session["AdminData"] = value;
            }
        }
    }
}