using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace  OtoRobotWeb2.Models.Database
{
    public class CompanyLoginData
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
    }
}
