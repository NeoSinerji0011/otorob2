using MongoDB.Bson.IO;
using Newtonsoft.Json;
using OtoRobotWeb2.Models.Response;
using System.Collections.Generic;
using System.Linq;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace  OtoRobotWeb2.Models.Inputs
{
    public class Enums
    {
    }
    public enum enum_Process
    {
        Login,
        LoginResult,
        Offer,
        OfferResult,
        OfferKasko,
        OfferKaskoResult,
        OFferCancel,
        Vehicle,
        Captcha,
        FinishOfferProcess,
        MultiOffer,
        MultiOfferResult,
        MultiOfferCancel,
        Order,
        ReLogin

    }
    public enum SirketKodlari
    {
        KAYAPAR,
        ERYAZ,
        KILINC,
        ERDAG,
        MARTAS,
        IKIMIZ,
        DEGA,
        OZCETE,
        OPAR,
        SENER,
        MERCANLAR,
        BASBUG,
        ART,
        MAKRO,
        KAMA,
        KAPLAN,
        HEMERA,
        ZIRVE
    }

    public class SirketListesi
    {
        static List<SirketItem> sirketList = new List<SirketItem> {
        new SirketItem{ SirketKodu=(int)SirketKodlari.KAYAPAR,SirketAdi="Kayapar" ,SirketAdiDisplay="KAYAPAR A.Ş",SiteUrl="https://b2b.kayapar.com/web/login",KullaniciAdi="erdinc",Sifre="qwer1234",OturumAcikMiScript="document.body.innerText.includes('Bakiye')",Telefon="444 9 009"},
        new SirketItem{ SirketKodu=(int)SirketKodlari.ERYAZ,SirketAdi="Eryaz",SirketAdiDisplay="ERYAZ SOFTWARE",SiteUrl="" ,KullaniciAdi="",Sifre="",Telefon=""},
        new SirketItem{ SirketKodu=(int)SirketKodlari.KILINC,SirketAdi ="Kilinc",SirketAdiDisplay="KILINÇ B2B" , SiteUrl="https://satis.kilincotoyedekparca.com/Default.aspx",KullaniciAdi="",Sifre="",OturumAcikMiScript="document.body.innerText.includes('Online Ödeme')",Telefon="0(212) 286 07 02"},
        new SirketItem{ SirketKodu=(int)SirketKodlari.ERDAG,SirketAdi ="Erdag",SirketAdiDisplay="ERDAĞ B2B" , SiteUrl="https://b2b.erdagotomotiv.com/Default.aspx",KullaniciAdi="",Sifre="",OturumAcikMiScript="document.body.innerText.includes('Temsilci Bilgileri')",Telefon="0(216) 471 03 99"},
        new SirketItem{ SirketKodu=(int)SirketKodlari.MARTAS,SirketAdi="Martas" ,SirketAdiDisplay="MARTAŞ A.Ş",SiteUrl="https://online.martas.com.tr/web/",KullaniciAdi="erdinc2",Sifre="1234567",OturumAcikMiScript="document.body.innerText.includes('Bakiye')",Telefon="0(216) 561 03 03 "},
         new SirketItem{ SirketKodu=(int)SirketKodlari.IKIMIZ,SirketAdi="İkimiz" ,SirketAdiDisplay="İKİMİZ LTD.ŞTİ.",SiteUrl="https://demo.ikimizoto.com.tr/login",KullaniciAdi="1200340201-02",Sifre="722CZf950",OturumAcikMiScript="document.body.innerText.includes('Siparişler')",Telefon="0(212) 671 48 85"},
         new SirketItem{ SirketKodu=(int)SirketKodlari.DEGA,SirketAdi="Dega" ,SirketAdiDisplay="DEGA",SiteUrl="",KullaniciAdi="",Sifre="",OturumAcikMiScript="",Telefon="444 4 006"},
          new SirketItem{ SirketKodu=(int)SirketKodlari.OZCETE, SirketAdi="Ozcete",SirketAdiDisplay="ÖZÇETE",SiteUrl="",KullaniciAdi="",Sifre="",OturumAcikMiScript="",Telefon="444 4 006"},
          new SirketItem{ SirketKodu=(int)SirketKodlari.OPAR,SirketAdi="Opar" ,SirketAdiDisplay="OPAR",SiteUrl="https://b2b.opar.com/",Telefon="0(216) 277 75 00"},
            new SirketItem{ SirketKodu=(int)SirketKodlari.SENER,SirketAdi="Sener" ,SirketAdiDisplay="ŞENER",SiteUrl="https://m.seneroto.com.tr/Stok" ,Telefon="0(212) 412 19 00"},
        };

        public static SirketItem GetSirketItem(int sirketKod)
        {
            var res = sirketList.Where(x => x.SirketKodu == sirketKod).FirstOrDefault();
            if (res != null)
            {
                return res;
            }
            return null;
        }
    }
    public class SirketItem
    {
        public int SirketKodu { get; set; }
        public string SirketAdi { get; set; }
        public string SirketAdiDisplay { get; set; }
        public string SiteUrl { get; set; }
        public string SirketDosyaAdi { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public string Telefon { get; set; }
        public string OturumAcikMiScript { get; set; }

    }
    public class PersonDetailItem
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
    public class ExcelPersonDetailItem
    {
        public string Firma { get; set; }
        public string SorguSayisi { get; set; }
    }
    public class AuthorizeList
    {
        public List<string> list()
        {
            var path = Environment.CurrentDirectory + "\\File\\Json\\jsonauth.txt";
            var temptext = File.ReadAllText(path);
            var res = JsonConvert.DeserializeObject<List<string>>(temptext);
            if (res != null)
                return res;
            else
                return new List<string>();
        }
    }

}
