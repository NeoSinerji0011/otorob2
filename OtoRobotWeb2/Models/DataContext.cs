﻿using Microsoft.EntityFrameworkCore;
using OtoRobotWeb2.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  OtoRobotWeb2.Models
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //İSTANBUL BİLGİSAYAR DATABASE BAĞLANTI KODLARI 

            optionsBuilder.UseSqlServer(@"Server = tcp: 213.254.135.175; Initial Catalog = NSEdirneTest2; Persist Security Info = False; User ID = NeosinerjiDBManager@neoonline; Password = NeoSnrj2017); MultipleActiveResultSets = True;Encrypt = True; TrustServerCertificate = True; Connection Timeout = 400;");

            //EDİRNE SERVER DATABASE BAĞLANTI KODLARI 

            //optionsBuilder.UseSqlServer(@"Server = tcp: 159.146.11.107; Initial Catalog = NSEdirneTest2; Persist Security Info = False; User ID = NeosinerjiDBManager@neoonline; Password = SnrjNeo2024); MultipleActiveResultSets = True;Encrypt = True; TrustServerCertificate = True; Connection Timeout = 400;");

        }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { 
        }
        
        public DbSet<TVMKullanicilar> TVMKullanicilar { get; set; }
        public DbSet<TVMDetay> TVMDetay { get; set; }
        public DbSet<OtoRobotAracFiyatListesi> OtoRobotAracFiyatListesi { get; set; }
        public DbSet<OtoRobotAracMarka> OtoRobotAracMarka{ get; set; }




    }
}
