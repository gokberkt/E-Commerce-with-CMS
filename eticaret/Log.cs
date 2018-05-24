using eticaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eticaret
{
    public class Log
    {
        public static string Name
        {
            get
            {
                return CustomerData.AdminInfo.Name;
            }
            set
            {
                CustomerData.AdminInfo.Name = value;
            }
        }
        public static string Lastname
        {
            get
            {
                return CustomerData.AdminInfo.Lastname;
            }
            set
            {
                CustomerData.AdminInfo.Lastname = value;
            }
        }

        //Admin girişini logla
        public static void AdminLogin()
        {
            string msg = "";

            eTicaretDBEntities db = new eTicaretDBEntities();

            msg = string.Format("{0} {1} giriş yaptı!", Name, Lastname);

            Logs log = new Logs();
            log.AdminID = CustomerData.AdminInfo.ID;
            log.CreatedDate = DateTime.Now;
            log.Status = true;
            log.Message = msg;

            db.Logs.Add(log);
            db.SaveChanges();
        }

        //Admin çıkışlarını logla
        public static void AdminLogout()
        {
            string msg = "";

            eTicaretDBEntities db = new eTicaretDBEntities();

            msg = string.Format("{0} {1} çıkış yaptı!", Name, Lastname);

            Logs log = new Logs();
            log.AdminID = CustomerData.AdminInfo.ID;
            log.CreatedDate = DateTime.Now;
            log.Status = true;
            log.Message = msg;

            db.Logs.Add(log);
            db.SaveChanges();
        }

        //Admin güncellemelerini logla
        public static void AdminUpdateLog(int id)
        {
            string msg = "";

            eTicaretDBEntities db = new eTicaretDBEntities();

            msg = string.Format("{0} {1} '{2}' ID 'li admini güncelledi!", Name, Lastname, id);

            Logs log = new Logs();
            log.AdminID = CustomerData.AdminInfo.ID;
            log.CreatedDate = DateTime.Now;
            log.Status = true;
            log.Message = msg;

            db.Logs.Add(log);
            db.SaveChanges();
        }

        //Ürün güncellemesini logla
        public static void ProductUpdateLog(int id)
        {
            string msg = "";

            eTicaretDBEntities db = new eTicaretDBEntities();

            msg = string.Format("{0} {1} '{2}' ID 'li ürünü güncelledi!", Name, Lastname, id);

            Logs log = new Logs();
            log.AdminID = CustomerData.AdminInfo.ID;
            log.CreatedDate = DateTime.Now;
            log.Status = true;
            log.Message = msg;

            db.Logs.Add(log);
            db.SaveChanges();
        }

        //Ürün silinmesini logla
        public static void ProductRemoveLog(int id)
        {
            string msg = "";

            eTicaretDBEntities db = new eTicaretDBEntities();

            msg = string.Format("{0} {1} '{2}' ID 'li ürünü sildi!", Name, Lastname, id);

            Logs log = new Logs();
            log.AdminID = CustomerData.AdminInfo.ID;
            log.CreatedDate = DateTime.Now;
            log.Status = true;
            log.Message = msg;

            db.Logs.Add(log);
            db.SaveChanges();
        }

    }
}