using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace J111.Models
{
    public class urunData
    {
        public int UrunId { get; set; }
        public int KategoriId { get; set; }
        public string UrunAdi { get; set; }
        public string UrunAciklama { get; set; }
        public int UrunFiyat { get; set; }
        public int UrunStock { get; set; }
        public string KategoriAdi { get; set; }
        public string[] ozellikAd { get; set; }
        public string[] ozellikAciklama { get; set; }
    }
}