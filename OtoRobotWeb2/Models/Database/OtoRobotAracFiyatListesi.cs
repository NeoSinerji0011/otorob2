namespace OtoRobotWeb2.Models.Database
{
    public class OtoRobotAracFiyatListesi
    {
        public int Id { get; set; }
        public string MarkaKodu { get; set; }
        public string OrjinalNo { get; set; }
        public string ParcaAdi { get; set; }
        public Nullable<decimal> ListeFiyati { get; set; }
        public Nullable<decimal> KdvDahil { get; set; }
        public string ParaBirimi { get; set; }
        public Nullable<DateTime> ListeTarihi { get; set; }
    }
}
