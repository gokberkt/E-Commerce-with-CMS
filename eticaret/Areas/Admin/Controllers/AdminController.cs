using eticaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace eticaret.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {

        eTicaretDBEntities db = new eTicaretDBEntities();

        // GET: Admin/Admin


        public ActionResult Register()
        {
            //Logon ise anasayfaya yönlendir
            return View();
        }

        [HttpPost]
        public ActionResult Register(Admins u)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(u.Name) || string.IsNullOrWhiteSpace(u.Lastname) || string.IsNullOrWhiteSpace(u.Email) || string.IsNullOrWhiteSpace(u.Password))
                {
                    ViewBag.Error = "Lütfen gerekli alanları doldurunuz !";
                }
                else if (db.Admins.Any(x => x.Email == u.Email))
                {
                    ViewBag.Error = "E-Posta hesabı kullanımda !";
                }
                else
                {
                    u.CreatedDate = DateTime.Now;
                    u.Status = false;
                    u.Password = Helpers.PasswordToMD5(u.Password);
                    db.Admins.Add(u);
                    db.SaveChanges();
                    return Redirect("/Admin/Login");
                }

            }
            catch (Exception)
            {
                ViewBag.Error = "Hesap oluşturulurken hata oluştu..";
            }
            return View();
        }




        public ActionResult Login()
        {
            //Logon ise anasayfaya yönlendir
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admins u)
        {
            try
            {
                Admins admin = db.Admins.Where(x => x.Email == u.Email).SingleOrDefault();
                if (admin.Email == u.Email && admin.Password == Helpers.PasswordToMD5(u.Password) && admin.Status == true)
                {

                    CustomerData.AdminInfo = admin;
                    HttpCookie adminCookie = new HttpCookie("AdminCookie");
                    adminCookie.Expires.AddDays(14);
                    adminCookie.Values.Add("Email", admin.Email);
                    adminCookie.Values.Add("UserId", admin.ID.ToString());
                    Response.Cookies.Add(adminCookie);

                    Log.AdminLogin();

                    return Redirect("/Admin/Management/Index");

                }
                else
                {
                    ViewBag.Error = "E-Posta ve ya Şifre Yanlış !";
                }
            }
            catch (Exception)
            {

                ViewBag.Error = "Bir hata oluştu !";
            }
            return View();
        }

        public ActionResult Logout()
        {
            Log.AdminLogout();
            Session.Abandon();
            CustomerData.AdminInfo = null;

            HttpCookie oldCookie = Request.Cookies["AdminCookie"];
            oldCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(oldCookie);

            return RedirectToAction("Login","Admin");
        }
    }
}