using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace  OtoRobotWeb2.Models.Database
{
    public class Companys
    {
        [Key]
        public int Id { get; set; }
        public string Kodu { get; set; }
        public string Unvani { get; set; }
        public string Url { get; set; }
        public string Yazilim { get; set; }
    }
}
