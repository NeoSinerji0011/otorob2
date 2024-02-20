using System.ComponentModel.DataAnnotations;

namespace  OtoRobotWeb2.Models.Database
{
    public class TVMKullanicilar
    {
        [Key]
        public int KullaniciKodu { get; set; }
        public int TVMKodu { get; set; }
        public string Adi{ get; set; }
        public string Soyadi { get; set; }
        public string Email { get; set; }
        public string Sifre { get; set; }
        public byte Durum { get; set; }
        public string? SessionToken { get; set; }
        
    }
}
