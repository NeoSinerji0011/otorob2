using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace  OtoRobotWeb2.Models.Database
{
    public class TUMDetay
    {

        [Key]
        public int Kodu { get; set; }
        public string Unvani { get; set; }
        public byte Durum { get; set; }
        public string Telefon { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebAdresi { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
    }
}
