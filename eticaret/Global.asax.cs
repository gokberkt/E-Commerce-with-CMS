using eticaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace eticaret
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        void Session_Start()
        {
            eTicaretDBEntities db = new eTicaretDBEntities();

            HttpCookie cookie = Request.Cookies["UserCookie"];
            if (cookie!=null)
            {
                string username = cookie.Values["Username"];
                int userID = Convert.ToInt32(cookie.Values["UserId"]);
                Customers cs = db.Customers.FirstOrDefault(x => x.Username == username && x.ID == userID);
                if (cs != null)
                {
                    CustomerData.Info = cs;
                }
            }

            HttpCookie adminCookie = Request.Cookies["AdminCookie"];
            if (adminCookie != null)
            {
                string email = adminCookie.Values["Email"];
                int userID = Convert.ToInt32(adminCookie.Values["UserId"]);
                Admins admin = db.Admins.FirstOrDefault(x => x.Email == email && x.ID == userID);
                if (admin != null)
                {
                    CustomerData.AdminInfo = admin;
                }
            }

        }
    }
}
