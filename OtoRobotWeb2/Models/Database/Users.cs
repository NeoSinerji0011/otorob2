using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace  OtoRobotWeb2.Models.Database
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public int Kodu { get; set; }
        public string Unvani { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
