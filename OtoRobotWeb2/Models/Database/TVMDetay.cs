using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace OtoRobotWeb2.Models.Database
{
    public class TVMDetay
    {
        [Key]
        public int Kodu { get; set; }
        public int? BagliTVMKodu{ get; set; }

        public Nullable<byte> SorgulamaTuru { get; set; }

        public string Unvani { get; set; }
        public string? WebAdresi { get; set; }
        public Nullable<bool> TopluSorguVarmi { get; set; }
        public Nullable<bool> SepeteEkleVarmi { get; set; }

        
    }
}
