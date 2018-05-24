using eticaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eticaret.Controllers
{
    public class CartController : Controller
    {

        eTicaretDBEntities db = new eTicaretDBEntities();
        
        [HttpGet]
        public ActionResult BagList()
        {
            if (CustomerData.Info == null)
            {
                return Json("-2", JsonRequestBehavior.AllowGet);
            }
            try
            {
                Bags bag = db.Bags.Where(x => x.CustomerID == CustomerData.Info.ID && x.Status == true).FirstOrDefault();
                if (bag==null)
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                List<BagProducts> bagProducts = bag.BagProducts.ToList();
                if (bagProducts==null)
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                var list = bagProducts.Select(x => new {
                    ID = x.ID,
                    Image = Helpers.GetProduct(x.ProductID).Photo,
                    BagID = x.BagID,
                    Amount = x.Amount,
                    Price = Helpers.GetProduct(x.ProductID).Price, 
                    Name = Helpers.GetProduct(x.ProductID).Name
                }).ToList();
                if (list.Count==0)
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception )
            {
                return Json("-1", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult BagProductIncrease(int ID)
        {
            if (CustomerData.Info == null)
            {
                return Json("-2", JsonRequestBehavior.AllowGet);
            }
            try
            {
                BagProducts bp = db.BagProducts.FirstOrDefault(x => x.ID == ID);
                Products prd = Helpers.GetProduct(bp.ProductID);
                if (prd.UnitsInStock<bp.Amount+1)
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                bp.Amount += 1;
                db.SaveChanges();
                return Json("1",JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("-1",JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult BagProductDecrease(int ID)
        {
            if (CustomerData.Info == null)
            {
                return Json("-2", JsonRequestBehavior.AllowGet);
            }
            try
            {
                BagProducts bp = db.BagProducts.FirstOrDefault(x => x.ID == ID);
                if (bp.Amount>1)
                {
                    bp.Amount -= 1;
                    db.SaveChanges();
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("0",JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json("-1", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult BagProductDelete(int ID)
        {
            if (CustomerData.Info == null)
            {
                return Json("-2", JsonRequestBehavior.AllowGet);
            }
            try
            {
                BagProducts bp = db.BagProducts.FirstOrDefault(x => x.ID == ID);
                if (bp!=null)
                {
                    db.BagProducts.Remove(bp);
                    db.SaveChanges();
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json("-1", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult BagProductAdd(int productID,int amount)
        {
            if (CustomerData.Info == null)
            {
                return Json("-2", JsonRequestBehavior.AllowGet);
            }
            try
            {
                Products prd = Helpers.GetProduct(productID);
                if (prd.UnitsInStock < amount)
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                Bags bag = db.Bags.Where(x => x.CustomerID == CustomerData.Info.ID && x.Status == true).FirstOrDefault();
                if (bag==null)
                {
                    Bags newBag = new Bags();
                    newBag.CreatedDate = DateTime.Now;
                    newBag.CustomerID = CustomerData.Info.ID;
                    newBag.Status = true;
                    newBag.IsSold = false;
                    db.Bags.Add(newBag);
                    db.SaveChanges();

                    BagProducts newBP = new BagProducts();
                    newBP.Amount = amount;
                    newBP.BagID = newBag.ID;
                    newBP.ProductID = productID;
                    db.BagProducts.Add(newBP);
                    db.SaveChanges();
                    return Json("1",JsonRequestBehavior.AllowGet);
                }
                else
                {
                    BagProducts bp = db.BagProducts.FirstOrDefault(x => x.BagID==bag.ID && x.ProductID == productID);
                    if (bp==null)
                    {
                        BagProducts newBP = new BagProducts();
                        newBP.Amount = amount;
                        newBP.BagID = bag.ID;
                        newBP.ProductID = productID;
                        db.BagProducts.Add(newBP);
                        db.SaveChanges();
                    }
                    else
                    {
                        bp.Amount += 1;
                        db.SaveChanges();
                    }
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json("-1", JsonRequestBehavior.AllowGet);
            }
        }

    }
}