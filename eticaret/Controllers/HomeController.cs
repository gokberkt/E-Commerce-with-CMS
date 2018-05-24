using eticaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eticaret.Controllers
{
    public class HomeController : Controller
    {
        eTicaretDBEntities db = new eTicaretDBEntities();
        // GET: Home
        public ActionResult Index()
        {
            List<Products> products = db.Products.ToList();
            return View(products);
        }

        public ActionResult Header()
        {

            List<Categories> cat = db.Categories.Where(x=>x.Status == true).ToList();
            return View(cat);

        }

        public ActionResult Carousel()
        {
            return View();
        }

        public ActionResult FeaturesBlock()
        {
            return View();
        }
    }
}