using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data;
using J111.Models;
using System.IO;

namespace J111.Controllers
{
    public class HomeController : Controller
    {
        DatabaseConnection db = new DatabaseConnection();

        public ActionResult adresGiris()
        {
            return View();
        }
        [HttpPost]
        public ActionResult adresGiris(adresData aD)
        {
            string sonuc = db.AdresEkle(aD);
            ViewBag.sonuc = sonuc;
            return View();
        }
            
        public ActionResult GetImage(int id)
        {
            var imageData = db.resimAl(id.ToString());

            return File(imageData, "image/jpg");
        }

        public ActionResult urunEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult urunEkle(HttpPostedFileBase file,string Urunid)
        {
            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/uimages"), pic);
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                    db.ResimEkle(Urunid, array);
                }

            }
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult about()
        {
            return View();
        }
        public ActionResult Urunler()
        {
            DataTable dt = db.GetDataTable("SELECT * FROM URUN", null);
            urunData[] uD = new urunData[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                uD[i] = new urunData();
                uD[i].UrunId = Convert.ToInt32(dt.Rows[i].ItemArray[0]);
                uD[i].UrunAdi = dt.Rows[i].ItemArray[1].ToString();
                uD[i].UrunFiyat = Convert.ToInt32(dt.Rows[i].ItemArray[2]);
                uD[i].UrunAciklama = dt.Rows[i].ItemArray[3].ToString();
            }
            return View(uD);
        }
        public ActionResult Urun(string UrunID)
        {
            if (UrunID != null)
            {
                urunData uD = new urunData();
                DataTable dt = db.GetDataTable("SELECT * FROM table(URUNPAC.urunpagedonder(:p0))", new string[] {UrunID });
                uD.UrunId = Convert.ToInt32(UrunID);
                uD.UrunAdi = dt.Rows[0].ItemArray[1].ToString();
                uD.UrunFiyat = Convert.ToInt32(dt.Rows[0].ItemArray[2]);
                uD.UrunStock = Convert.ToInt32(dt.Rows[0].ItemArray[3]);
                uD.UrunAciklama = dt.Rows[0].ItemArray[4].ToString();
                uD.KategoriId = Convert.ToInt32(dt.Rows[0].ItemArray[5]);
                uD.KategoriAdi = dt.Rows[0].ItemArray[6].ToString();
                uD.ozellikAd = dt.Rows[0].ItemArray[7].ToString().Split(',');
                uD.ozellikAciklama = dt.Rows[0].ItemArray[8].ToString().Split(',');

                return View(uD);
            }
            else
                return RedirectToAction("index", "Home");
        }
        public ActionResult Sepet(string UrunID)
        {
            int toplamfiyat = 0;
            if(UrunID!=null)
            if (Session["Sepet"] == null){
                Session["Sepet"] = UrunID.ToString();
            }
            else{
                bool eklenecek = true;
                string[] idler = Session["Sepet"].ToString().Split(',');
                for (int i = 0; i < idler.Length; i++)
                {
                    if (idler[i] == UrunID.ToString())
                        eklenecek = false;
                }
                if (eklenecek)
                    Session["Sepet"] = Session["Sepet"].ToString() + "," + UrunID.ToString();
            }
            string[] SUrunIds = Session["Sepet"].ToString().Split(',');
            int[] UrunIds = new int[SUrunIds.Length];
            for (int i = 0; i < SUrunIds.Length; i++)
            {
                UrunIds[i] = Convert.ToInt32(SUrunIds[i]);
            }
            urunData[] uD = new urunData[UrunIds.Length];
            for (int i = 0; i < UrunIds.Length; i++)
            {
                uD[i] = new urunData();
                DataTable dt = db.GetDataTable("SELECT * FROM URUN WHERE URUNID = :p0", new string[] { UrunIds[i].ToString() });
                uD[i].UrunId = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
                uD[i].UrunAdi = dt.Rows[0].ItemArray[1].ToString();
                uD[i].UrunFiyat = Convert.ToInt32(dt.Rows[0].ItemArray[2]);
                uD[i].UrunStock = Convert.ToInt32(dt.Rows[0].ItemArray[4]);
                uD[i].UrunAciklama = dt.Rows[0].ItemArray[3].ToString();
                uD[i].KategoriId = Convert.ToInt32(dt.Rows[0].ItemArray[5]);
                toplamfiyat += uD[i].UrunFiyat;
            }
                ViewBag.toplamfiyat = toplamfiyat;
                return View(uD);
        }
        public ActionResult Giris()
        {
            if(TempData["msg"]!=null)
                ViewBag.msg = TempData["msg"].ToString();
            return View();
        }
        [HttpPost]
        public ActionResult Giris(kullaniciGiris k)
        {
            DataTable dt = db.GetDataTable("SELECT * FROM KULLANICILAR WHERE KULLANICIEMAIL=:p0 AND KULLANICISIFRE=:p1", new string[]{ k.email, k.sifre });
            if (dt.Rows.Count==1)
            {
                Session.Add("KullaniciAdi", dt.Rows[0].ItemArray[2]);
                Session.Add("KullaniciSoyad", dt.Rows[0].ItemArray[3]);
                Session.Add("KullaniciEmail", dt.Rows[0].ItemArray[1]);             
                return RedirectToAction("index","Home");
            }
            else
            {
                ViewBag.HataliGiris = "Kullanici adi veya şifre Yanlış";
                return View();
            }
        }
        [HttpPost]
        public ActionResult GirisKayit(kullaniciGiris k)
        {
            string msg = db.UDTuserregister(k);
            TempData["msg"] = msg;
            return RedirectToAction("Giris","Home");
        }
    }
}