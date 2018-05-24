using eticaret.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace eticaret.Areas.Admin.Controllers
{
    [ShopAuthorize]
    public class ManagementController : Controller
    {
        eTicaretDBEntities db = new eTicaretDBEntities();


        public ActionResult LeftSideBar()
        {
            return View();
        }


        // Tüm aktif ürünleri getir
        public ActionResult Index(int? pageNo)
        {
            int _pageNo = pageNo ?? 1;
            string arama = Request.QueryString["arama"];
            var productList = new Object();
            if (!string.IsNullOrWhiteSpace(arama))
            {
                productList = db.Products.Where(x => x.Name.Contains(arama) || x.Description.Contains(arama) || x.ExtraDescription.Contains(arama)).OrderBy(x => x.Name).ToPagedList<Products>(_pageNo, 10);

            }
            else
            {
                productList = db.Products.OrderBy(x => x.Name).ToPagedList<Products>(_pageNo, 10);
            }
            return View(productList);
        }

        //Ürünü pasife al
        [HttpGet]
        public ActionResult RemoveProduct(int? id)
        {
            try
            {
                Products pr = db.Products.FirstOrDefault(x => x.ID == id);

                if (pr != null)
                {
                    pr.Status = false;
                    db.SaveChanges();
                    Log.ProductRemoveLog(pr.ID);
                }
            }
            catch (Exception)
            {
                return Redirect("/Admin/management/index?Status=DeleteFailed");
            }

            return Redirect("/Admin/management/index?Status=DeleteSuccess");
        }

        //Ürünü güncelle -get
        [HttpGet]
        public ActionResult EditProduct(int? id)
        {
            Products pr = db.Products.FirstOrDefault(x => x.ID == id);
            if (pr == null)
            {
                return HttpNotFound();
            }
            return View(pr);
        }

        //Ürünü güncelle -post
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditProduct(int? id, Products p)
        {
            Products pr = db.Products.FirstOrDefault(x => x.ID == id);
            if (pr == null)
            {
                return HttpNotFound();
            }
            else
            {
                pr.Name = p.Name;
                pr.Description = p.Description;
                pr.ExtraDescription = p.ExtraDescription;
                pr.UnitsInStock = p.UnitsInStock;
                pr.Price = p.Price;
                pr.Status = p.Status;


                #region Image Upload
                if (Request.Files.Count > 0)
                {
                    #region Old Images - Delete
                    if (System.IO.File.Exists(Server.MapPath(pr.Photo)))
                    {
                        System.IO.File.Delete(Server.MapPath(pr.Photo));
                    }
                    if (System.IO.File.Exists(Server.MapPath(pr.Photo.Replace("uploads/ProductMasterImages/OS", "uploads/ProductMasterImages/L"))))
                    {
                        System.IO.File.Delete(Server.MapPath(pr.Photo.Replace("uploads/ProductMasterImages/OS", "uploads/ProductMasterImages/L")));
                    }
                    if (System.IO.File.Exists(Server.MapPath(pr.Photo.Replace("uploads/ProductMasterImages/OS", "uploads/ProductMasterImages/S"))))
                    {
                        System.IO.File.Delete(Server.MapPath(pr.Photo.Replace("uploads/ProductMasterImages/OS", "uploads/ProductMasterImages/S")));
                    }
                    #endregion

                    HttpPostedFileBase file = Request.Files[0];

                    WebImage img = new WebImage(file.InputStream);
                    FileInfo fotoinfo = new FileInfo(file.FileName);

                    Random rnd = new Random();
                    int random = rnd.Next(1000, 9999);
                    string filename = file.FileName.Remove(file.FileName.Length - fotoinfo.Extension.Length, fotoinfo.Extension.Length);
                    string imgname = DateTime.UtcNow.Month.ToString() + DateTime.UtcNow.Day.ToString() + "_" + pr.ID + "_" + filename + "_" + random;
                    string newfoto = imgname + fotoinfo.Extension;
                    img.Save("~/uploads/ProductMasterImages/OS/" + newfoto);
                    Helpers.ImageResize("uploads/ProductMasterImages/OS", "uploads/ProductMasterImages/L", newfoto, 1024, 768);
                    Helpers.ImageResize("uploads/ProductMasterImages/OS", "uploads/ProductMasterImages/S", newfoto, 200, 155);

                    pr.Photo = "/uploads/ProductMasterImages/OS/" + newfoto;
                }
                #endregion


                db.SaveChanges();
                Log.ProductUpdateLog(pr.ID);
            }
            return View(pr);
        }

        //Ürün resimlerini bas
        public ActionResult ProductImages(int? id)
        {
            Products pr = db.Products.FirstOrDefault(x => x.ID == id);
            if (pr == null)
            {
                return HttpNotFound();
            }

            return View(pr.ProductImages.ToList());
        }

        //Ürün resmi yükle
        [HttpPost]
        public ActionResult ImagesUpload()
        {
            try
            {
                string ProductID = Request.QueryString["ID"];
                if (String.IsNullOrWhiteSpace(ProductID)) return Redirect("/Admin/Admin");

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    ProductImages photo = new ProductImages();
                    photo.ProductID = int.Parse(ProductID);

                    WebImage img = new WebImage(file.InputStream);
                    FileInfo fotoinfo = new FileInfo(file.FileName);

                    Random rnd = new Random();
                    int random = rnd.Next(1000, 9999);
                    string filename = file.FileName.Remove(file.FileName.Length - fotoinfo.Extension.Length, fotoinfo.Extension.Length);
                    string imgname = DateTime.UtcNow.Month.ToString() + DateTime.UtcNow.Day.ToString() + "_" + photo.ProductID + "_" + filename + "_" + random;
                    string newfoto = imgname + fotoinfo.Extension;
                    img.Save("~/uploads/Products/OS/" + newfoto);
                    Helpers.ImageResize("uploads/Products/OS", "uploads/Products/L", newfoto, 1024, 768);
                    Helpers.ImageResize("uploads/Products/OS", "uploads/Products/S", newfoto, 200, 155);

                    photo.ImagePath = "/uploads/Products/OS/" + newfoto;
                    db.ProductImages.Add(photo);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("-1", JsonRequestBehavior.AllowGet);
            }

            return Json("1", JsonRequestBehavior.AllowGet);
        }


        //Ürün resmi sil
        [HttpPost]
        public ActionResult DeleteProductImage(int? id)
        {
            try
            {
                ProductImages img = db.ProductImages.Find(id);

                if (System.IO.File.Exists(Server.MapPath(img.ImagePath)))
                {
                    System.IO.File.Delete(Server.MapPath(img.ImagePath));
                }
                if (System.IO.File.Exists(Server.MapPath(img.ImagePath.Replace("uploads/Products/OS", "uploads/Products/L"))))
                {
                    System.IO.File.Delete(Server.MapPath(img.ImagePath.Replace("uploads/Products/OS", "uploads/Products/L")));
                }
                if (System.IO.File.Exists(Server.MapPath(img.ImagePath.Replace("uploads/Products/OS", "uploads/Products/S"))))
                {
                    System.IO.File.Delete(Server.MapPath(img.ImagePath.Replace("uploads/Products/OS", "uploads/Products/S")));
                }
                db.ProductImages.Remove(img);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("-1", JsonRequestBehavior.AllowGet);
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        //Tüm Pasif ürünleri getir ve güncelle
        public ActionResult SoftDeletedProducts(int? pageNo)
        {
            int _pageNo = pageNo ?? 1;
            var productList = db.Products.Where(x => x.Status == false).OrderBy(x => x.Name).ToPagedList<Products>(_pageNo, 10);

            return View(productList);
        }


        //Satışları listele
        public ActionResult SoldProducts(int? pageNo)
        {
            int _pageNo = pageNo ?? 1;

            var soldProductList = db.SoldProducts.OrderBy(x => x.ID).ToPagedList<SoldProducts>(_pageNo, 10);

            return View(soldProductList);
        }


        //Admin hesaplarını göster
        [HttpGet]
        public ActionResult Accounts()
        {
            List<Admins> adminList = db.Admins.ToList();

            return View(adminList);
        }


        //Admin hesaplarını güncelle -get
        public ActionResult EditAdmin(int? id)
        {
            Admins admin = db.Admins.FirstOrDefault(x => x.ID == id);

            return View(admin);
        }

        //Admin hesaplarını güncelle -post
        [HttpPost]
        public ActionResult EditAdmin(int? id, Admins a)
        {
            Admins admin = db.Admins.Where(x => x.ID == id).SingleOrDefault();
            if (admin == null)
            {
                return HttpNotFound();
            }
            else
            {
                admin.Name = a.Name;
                admin.Lastname = a.Lastname;
                admin.Email = a.Email;
                admin.Status = a.Status;

                db.SaveChanges();
                Log.AdminUpdateLog(admin.ID);
            }
            return View(admin);
        }


        //Logları getir
        public ActionResult GetLogs(int? pageNo)
        {
            int _pageNo = pageNo ?? 1;
            var logs = db.Logs.OrderBy(x => x.ID).ToPagedList<Logs>(_pageNo, 10);

            return View(logs);
        }

        //Ürün ekle
        public ActionResult AddProduct()
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddProduct(Products p)
        {
            if (String.IsNullOrWhiteSpace(p.Name) || p.UnitsInStock == null || String.IsNullOrWhiteSpace(p.Description) || String.IsNullOrWhiteSpace(p.ExtraDescription))
            {
                ViewBag.Error = "Ürün eklenirken hata oluştu, bilgileri kontrol ediniz.";
                return View();
            }

            p.CreatedDate = DateTime.Now;
            p.NumberOfSales = 0;
            p.seoUrl = Helpers.GenerateSeoUrl(p.Name);
            p.Photo = "";
            db.Products.Add(p);
            db.SaveChanges();

            #region Image Upload
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                WebImage img = new WebImage(file.InputStream);
                FileInfo fotoinfo = new FileInfo(file.FileName);

                Random rnd = new Random();
                int random = rnd.Next(1000, 9999);
                string filename = file.FileName.Remove(file.FileName.Length - fotoinfo.Extension.Length, fotoinfo.Extension.Length);
                string imgname = DateTime.UtcNow.Month.ToString() + DateTime.UtcNow.Day.ToString() + "_" + p.ID + "_" + filename + "_" + random;
                string newfoto = imgname + fotoinfo.Extension;
                img.Save("~/uploads/ProductMasterImages/OS/" + newfoto);
                Helpers.ImageResize("uploads/ProductMasterImages/OS", "uploads/ProductMasterImages/L", newfoto, 1024, 768);
                Helpers.ImageResize("uploads/ProductMasterImages/OS", "uploads/ProductMasterImages/S", newfoto, 200, 155);

                p.Photo = "/uploads/ProductMasterImages/OS/" + newfoto;
                db.SaveChanges();

            }
            #endregion

            Log.ProductUpdateLog(p.ID);

            TempData["SuccessAddProduct"] = "Ürün başarıyla eklendi.";
            return Redirect("/Admin/Management/editProduct/" + p.ID);
        }

        //Kategori Ekle
        public ActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddCategory(Categories cat)
        {
            if (string.IsNullOrWhiteSpace(cat.Name) || string.IsNullOrWhiteSpace(cat.Description))
            {
                ViewBag.Error = "Kategori eklenirken hata oluştu, bilgileri kontrol ediniz.";
                return View();
            }
            if (cat.MasterCatID == -1)
            {
                cat.MasterCatID = null;
            }
            cat.seoUrl = Helpers.GenerateSeoUrl(cat.Name);
            db.Categories.Add(cat);
            db.SaveChanges();

            TempData["SuccessAddCategory"] = "Kategori başarıyla eklendi";
            return Redirect("/Admin/Management/editcategory/" + cat.ID);
        }

        //Kategorileri getir
        public ActionResult GetCategories()
        {
            List<Categories> categoryList = db.Categories.ToList();
            return View(categoryList);
        }

        //Kategori güncelle
        public ActionResult EditCategory(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult EditCategory(Categories cat)
        {
            Categories category = db.Categories.FirstOrDefault(x => x.ID == cat.ID);
            if (category == null)
            {
                return HttpNotFound();
            }
            if (string.IsNullOrWhiteSpace(cat.Name) || string.IsNullOrWhiteSpace(cat.Description))
            {
                ViewBag.Error = "Kategori eklenirken hata oluştu, bilgileri kontrol ediniz.";
                return View();
            }
            category.Name = cat.Name;
            category.MasterCatID = cat.MasterCatID;
            category.Description = cat.Description;
            if (category.Status == false && cat.Status == true)
            {
                if (category.MasterCatID == null)
                {
                    foreach (Categories subcat in category.Categories1.ToList())
                    {
                        subcat.Status = true;
                        foreach (Products pd in subcat.Products)
                        {
                            pd.Status = true;
                        }
                    }
                }
                else
                {
                    foreach (Products pd in category.Products)
                    {
                        pd.Status = true;
                    }
                }
                
            }
            category.Status = cat.Status;
            db.SaveChanges();

            return View(category);
        }

        //Kategori Sil
        [HttpGet]
        public ActionResult RemoveCategory(int? id)
        {
            try
            {
                Categories cat = db.Categories.FirstOrDefault(x => x.ID == id);

                if (cat != null)
                {
                    if (cat.MasterCatID != null)
                    {
                        List<Categories> subCats = db.Categories.Where(x => x.MasterCatID == id).ToList();
                        foreach (Categories subcat in subCats)
                        {
                            List<Products> products = db.Products.Where(x => x.CategoryID == subcat.ID).ToList();
                            foreach (Products pd in products)
                            {
                                pd.Status = false;
                            }
                            subcat.Status = false;
                        }
                    }
                    else
                    {
                        cat.Status = false;
                    }


                    db.SaveChanges();

                }
            }
            catch (Exception)
            {
                return Redirect("/Admin/management/getcategories?Status=DeleteFailed");
            }

            return Redirect("/Admin/management/getcategories?Status=DeleteSuccess");
        }
    }
}