using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace  OtoRobotWeb2.Models.Database
{
    public class OtoLoginSigortaSirketKullanicilar
    {
        [Key]
        public int Id { get; set; }
        public int TVMKodu { get; set; }
        public int TUMKodu { get; set; }
        public int? AltTVMKodu { get; set; }
        public string SigortaSirketAdi { get; set; }
        public string KullaniciAdi { get; set; }
        public string AcenteKodu { get; set; }
        public string Sifre { get; set; }
        public string InputTextKullaniciId { get; set; }
        public string InputTextAcenteKoduId { get; set; }
        public string InputTextSifreId { get; set; }
        public string InputTextGirisId { get; set; }
        public string LoginUrl { get; set; }
        public string ProxyIpPort { get; set; }
        public string ProxyKullaniciAdi { get; set; }
        public string ProxySifre { get; set; }
        public int? GrupKodu { get; set; }
        public string SmsKodTelNo { get; set; }
        public string SmsKodSecretKey1 { get; set; }
        public string SmsKodSecretKey2 { get; set; }




    }
}
