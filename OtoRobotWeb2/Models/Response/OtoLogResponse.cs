using Microsoft.AspNetCore.Mvc.Rendering;
using OtoRobotWeb2.Models.Database;
using OtoRobotWeb2.Models.Inputs;
using OtoRobotWeb2.Models.MongoDb;

namespace OtoRobotWeb2.Models.Response
{
    public class OtoLogResponse
    {
        public List<logTime> LogTimes { get; set; }
        public List<SelectListItem> TVMKullanicilars { get; set; }
        public List<SelectListItem> Tvmdetay { get; set; }
        public List<PersonDetailItem> PersonDetails{ get; set; }
        public int TvmKodu { get; set; } 
        public int KullaniciKodu{ get; set; }
        public string Unvani{ get; set; }
        public int SorguSayisi { get; set; }
        public bool rd_ozet { get; set; } = false;
        public bool rd_detayli { get; set; } = false;
        public bool rd_multiozet { get; set; } = false;
        public string HataMesaji { get; set; } = null;
        public int Total { get; set; } = 0;
        public DateTime Date_Baslangic { get; set; }
        public DateTime Date_Bitis { get; set; }
        public DateTime Date_Ozet { get; set; }
    }
}
