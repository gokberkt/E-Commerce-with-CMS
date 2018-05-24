using eticaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eticaret.Controllers
{

    public class ProductController : Controller
    {
        eTicaretDBEntities db = new eTicaretDBEntities();

        // GET: Product
        [Route("urunler/{seoUrl}-{ID}")]
        public ActionResult Index(int? id)
        {
            string sort = Request.QueryString["sort"];


            ViewBag.Categories = db.Categories.ToList();
            Categories cats = db.Categories.FirstOrDefault(x => x.ID == id);
            ViewBag.Category = cats;

            List<Products> productList = new List<Products>();
            if (sort == "NameAZ" || sort == null)
            {
                productList = db.Products.Where(x => x.CategoryID == id && x.Status == true).OrderBy(x => x.Name).ToList();
            }
            else if (sort == "NameZA")
            {
                productList = db.Products.Where(x => x.CategoryID == id && x.Status == true).OrderByDescending(x => x.Name).ToList();
            }
            else if (sort == "PriceHL")
            {
                productList = db.Products.Where(x => x.CategoryID == id && x.Status == true).OrderByDescending(x => x.Price).ToList();
            }
            else if (sort == "PriceLH")
            {
                productList = db.Products.Where(x => x.CategoryID == id && x.Status == true).OrderBy(x => x.Price).ToList();
            }

            return View(productList);
        }

        public ActionResult Banner()
        {
            return View();
        }

        [Route("urunler/urun-detay/{seoUrl}-{ID}")]
        public ActionResult Detail(int? id)
        {
            Products pr = db.Products.FirstOrDefault(x => x.ID == id && x.Status == true);
            if (pr == null)
            {
                return View("/");
            }

            ViewBag.ProductName = pr.Name;
            ViewBag.Category = pr.Categories;

            return View(pr);
        }
    }
}