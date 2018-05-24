using eticaret.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace eticaret
{
    public class Helpers
    {
        eTicaretDBEntities _db = new eTicaretDBEntities();
        public static string PasswordToMD5(string password)
        {
            
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] dizi = Encoding.UTF8.GetBytes(password);
            dizi = md5.ComputeHash(dizi);
            StringBuilder sb = new StringBuilder();
            foreach (byte ba in dizi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }
            return sb.ToString();
        }

        public static Products GetProduct(int? ID)
        {
            Products p = null;
            if (ID != null)
            {
                Helpers h = new Helpers();
                p = h._db.Products.FirstOrDefault(x => x.ID == ID);
            }
            return p;
        }

        public static void ImageResize(string sourcePath, string path, string originalFilename, int canvasWidth, int canvasHeight)
        {
            Image image = Image.FromFile(HttpContext.Current.Server.MapPath("~/" + sourcePath + "/" + originalFilename));
            System.Drawing.Image thumbnail = new Bitmap(canvasWidth, canvasHeight);
            System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            int originalWidth = image.Width;
            int originalHeight = image.Height;
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            graphic.Clear(Color.White);
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);


            System.Drawing.Imaging.ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            thumbnail.Save(HttpContext.Current.Server.MapPath("~/" + path + "/" + originalFilename), info[1], encoderParameters);
            image.Dispose();
        }

        public static string GenerateSeoUrl(string name)
        {
            name = name.Replace(" ", "-");
            name = name.Replace(".", "-");
            name = name.Replace("ı", "i");
            name = name.Replace("İ", "I");

            name = String.Join("", name.Normalize(NormalizationForm.FormD)
                    .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));

            name = HttpUtility.UrlEncode(name);
            string lastStr = System.Text.RegularExpressions.Regex.Replace(name, @"\%[0-9A-Fa-f]{2}", "");

            return lastStr.Substring(0,lastStr.Length-1).ToLower();
        }
    }
}