using eticaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eticaret.Controllers
{
    public class CheckOutController : Controller
    {
        eTicaretDBEntities db = new eTicaretDBEntities();

        [HttpGet]
        [Route("hesap-onay")]
        public ActionResult Shipping()
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

        [HttpPost]
        [Route("hesap-onay")]
        public ActionResult Shipping(Customers c)
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

            return RedirectToAction("OrderSummary", "CheckOut");
        }

        [HttpGet]
        [Route("siparis-ozet")]
        public ActionResult OrderSummary()
        {
            HttpCookie UserCookie = Request.Cookies["UserCookie"];
            if (UserCookie == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Bags bag = db.Bags.Where(x => x.CustomerID == CustomerData.Info.ID && x.Status == true).FirstOrDefault();
                if (bag == null)
                {
                    ViewBag.Error = "Üzgünüz sepetinizde hiç ürün yok :(";
                    return View();
                }
                List<BagProducts> bagProducts = bag.BagProducts.ToList();
                if (bagProducts == null)
                {
                    ViewBag.Error = "Üzgünüz sepetinizde hiç ürün yok :(";
                    return View();
                }

                return View(bagProducts);
            }
        }



        [HttpGet]
        [Route("odeme")]
        public ActionResult Payment()
        {

            return View();
        }

        [HttpGet]
        [Route("siparis-onay")]
        public ActionResult OrderComplete()
        {
            try
            {
                HttpCookie UserCookie = Request.Cookies["UserCookie"];
                if (UserCookie == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Bags bag = db.Bags.Where(x => x.CustomerID == CustomerData.Info.ID && x.Status == true && x.IsSold == false).FirstOrDefault();
                    if (bag == null)
                    {
                        ViewBag.Error = "Üzgünüz sepetinizde hiç ürün yok :(";
                    }
                    bag.IsSold = true;
                    bag.Status = false;
                    bag.SellDate = DateTime.Now;
                    db.Bags.Attach(bag);
                    db.Entry(bag).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    List<BagProducts> bagProducts = bag.BagProducts.ToList();

                    

                    foreach (var item in bagProducts)
                    {
                        SoldProducts sp = new SoldProducts();
                        sp.Amount = item.Amount;
                        sp.CustomerID = item.Bags.CustomerID;
                        sp.ProductID = item.ProductID;
                        sp.BagID = item.BagID;
                        db.SoldProducts.Add(sp);
                        db.SaveChanges();
                        
                    }

                    ViewBag.Date = bag.SellDate.ToString();
                    return View(bagProducts);
                }
            }
            catch (Exception)
            {

                ViewBag.Error = "Satın alırken bir hata oluştu..";
            }

            return View();

        }
    }
}