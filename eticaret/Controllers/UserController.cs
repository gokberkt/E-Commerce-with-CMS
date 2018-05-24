using eticaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace eticaret.Controllers
{

    public class UserController : Controller
    {
        eTicaretDBEntities db = new eTicaretDBEntities();
        // GET: User
        [Route("kullanici-kayit")]
        public ActionResult Register()
        {
            return View();
        }

        [Route("kullanici-kayit")]
        [HttpPost]
        public ActionResult Register(Customers c)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(c.Username) || string.IsNullOrWhiteSpace(c.Email) || string.IsNullOrWhiteSpace(c.Password))
                {
                    ViewBag.Error = "Lütfen gerekli alanları doldurunuz !";
                }
                else if (db.Customers.Any(x => x.Username == c.Username))
                {
                    ViewBag.Error = "Kullanıcı adı kullanımda !";
                }
                else if (db.Customers.Any(x => x.Email == c.Email))
                {
                    ViewBag.Error = "E-Posta hesabı kullanımda !";
                }
                else
                {
                    c.CreatedDate = DateTime.Now;
                    c.Status = true;
                    c.Password = Helpers.PasswordToMD5(c.Password);
                    db.Customers.Add(c);
                    db.SaveChanges();
                    return Redirect("/kullanici-giris?RegisterStatus=Success");
                }

            }
            catch (Exception)
            {
                ViewBag.Error = "Hesap oluşturulurken hata oluştu..";
            }
            return View();
        }

        [Route("kullanici-giris")]
        public ActionResult Login()
        {
            return View();
        }

        [Route("kullanici-giris")]
        [HttpPost]
        public ActionResult Login(Customers c)
        {
            Customers user = db.Customers.Where(x => x.Email == c.Email).SingleOrDefault();
            try
            {
                if (user.Email == c.Email && user.Password == Helpers.PasswordToMD5(c.Password))
                {

                    CustomerData.Info = user;
                    HttpCookie cookie = new HttpCookie("UserCookie");
                    cookie.Expires.AddDays(14);
                    cookie.Values.Add("Username", user.Username);
                    cookie.Values.Add("UserId", user.ID.ToString());
                    Response.Cookies.Add(cookie);
                    return Redirect("/");

                }
            }
            catch (Exception)
            {

                ViewBag.Error = "E-Posta ve ya Şifre Yanlış !";
            }

            return View();

        }


        [Route("kullanici-cikis")]
        public ActionResult Logout()
        {
            Session.Abandon();
            CustomerData.Info = null;

            HttpCookie oldCookie = Request.Cookies["UserCookie"];
            oldCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(oldCookie);

            return RedirectToAction("Index", "Home");
        }


        [Route("hesabim")]
        public ActionResult MyAccount()
        {

            HttpCookie UserCookie = Request.Cookies["UserCookie"];
            if (UserCookie == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Customers user = CustomerData.Info;
                return View(user);
            }

        }



        [Route("hesabim")]
        [HttpPost]
        public ActionResult MyAccount(Customers c)
        {
            Customers user = db.Customers.Where(x => x.ID == CustomerData.Info.ID).SingleOrDefault();

            user.Address = c.Address;
            user.Email = c.Email;
            user.Fullname = c.Fullname;
            user.Phone = c.Phone;

            db.Customers.Attach(user);
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            CustomerData.Info = user;

            return RedirectToAction("MyAccount", "User");
        }
    }
}