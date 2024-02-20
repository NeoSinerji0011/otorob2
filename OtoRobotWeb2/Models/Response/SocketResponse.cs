using OtoRobotWeb2.Models.Inputs;
using System;
using System.Collections.Generic;

namespace OtoRobotWeb2.Models.Response
{

    public class SocketResponse
    {
        public int ProcessType { get; set; }
        public int UserId { get; set; }
        public int KullaniciKodu { get; set; }
        public string SorguId { get; set; }
        public string GrupID { get; set; }
        public string SorguTuru { get; set; }
        public string OEMNumarasi { get; set; }
        public bool StokVarmi { get; set; }
        public bool isFinished { get; set; } = false;
        public bool isCancel { get; set; } = false;

        public List<int> SorgulanacakFirmalar { get; set; }
        public List<string> OemNumaralari { get; set; }
        public List<ResponseLoginItem> Logins = new List<ResponseLoginItem>();
        public List<ResponseItem> OfferResult = new List<ResponseItem>();
        public List<ExcelOEM> ExcelOEM = new List<ExcelOEM>();

        public RequestInput RequestInput { get; set; }

        public void setProcessType(enum_Process enum_Process)
        {
            this.ProcessType = ((int)enum_Process);
        }
    }
    public class ResponseItem
    {
        public string SirketAd { get; set; }
        public int SirketKodu { get; set; }
        public string Stok { get; set; }
        public string OemNo { get; set; }
        public string StokNo { get; set; }
        public string UrunAd { get; set; }
        public string Marka { get; set; }
        public string ListeFiyat { get; set; }
        public string KdvSiz { get; set; }
        public string KdvLi { get; set; }
        public string ImageBase64 { get; set; }
        public bool ImageCheck { get; set; }
        public int YanitVarmi { get; set; }
        public string SorgulananOem { get; set; }
        public List<string> TumFiyat { get; set; }

    }
    public class ResponseItemExcel
    {
        public string SorgulananOem { get; set; }
        public string SirketAd { get; set; }
        public string Stok { get; set; }
        public string OemNo { get; set; }
        public string StokNo { get; set; }
        public string UrunAd { get; set; }
        public string Marka { get; set; }
        public string KdvSiz { get; set; }
        public string KdvLi { get; set; }
        public string ListeFiyat { get; set; }

    }
    public class ResponseItemExcel2
    {
        public string SorgulananOem { get; set; }
        public string SirketAd { get; set; }
        public string Sonuc { get; set; }


    }
    public class ResponseLoginItem
    {
        public int SirketKodu { get; set; }
        public string SirketAdi { get; set; }
        public string SiteUrl { get; set; }
        public string Telefon { get; set; }

        public void setProcessType(SirketKodlari sirket)
        {
            this.SirketKodu = ((int)sirket);
        }
        public bool isOpen { get; set; } = false;

    }
    public class ExcelOEM
    {
        public string OemKodu { get; set; }
    }
}