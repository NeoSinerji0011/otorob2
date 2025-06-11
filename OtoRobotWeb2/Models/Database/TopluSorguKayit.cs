using System.ComponentModel.DataAnnotations.Schema;

namespace OtoRobotWeb2.Models.Database
{
    public class TopluSorguKayit
    {
        public int Id { get; set; }
        public string? SorgulananOemNo { get; set; }
        public DateTime Tarih { get; set; }
        public string SirketAd { get; set; }
        public string? Stok { get; set; }
        public string? OemNo { get; set; }
        public string? StokNo { get; set; }
        public string? UrunAd { get; set; }
        public string? Marka { get; set; }
        public string? kdvLi { get; set; }
        public string? kdvSiz { get; set; }
        public string? ListeFiyati { get; set; }
    }

[Table("Logs")]
    public class Logs
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RecordCount { get; set; }
        public string Description { get; set; }
    }
    [Table("AxaSorguSonuclari")]
    public class AxaSorguSonuclari
    {
        public int Id { get; set; }
        public long? DosyaNo { get; set; }
        public long? RaporNo { get; set; }
        public string SorgulananOemNo { get; set; }
        public string Tedarikci { get; set; }
        public string StokDurumu { get; set; }
        public string DonenOemNo { get; set; }
        public string TedarikciStokNo { get; set; }
        public string ParcaUrunAdi { get; set; }
        public string MarkaAdi { get; set; }
        public string KdvHaricFiyat { get; set; }
        public string KdvDahilFiyat { get; set; }
        public string ListeFiyat { get; set; }
        public string KampanyaliFiyat { get; set; }
        public string ParaBirimi { get; set; }
        public string SorguIslemTarihi { get; set; } // Eğer string ise (nvarchar(15)), DateTime olarak kullanmak istiyorsan dönüştürmen gerekebilir
        public string AxaTarih { get; set; }        // Aynı şekilde nvarchar(15)
    }

}
