using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Core.Configuration;
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
            var tempcontrol = false;
            //İSTANBUL BİLGİSAYAR DATABASE BAĞLANTI KODLARI 
            //string connectionString = (@"Server = tcp: neoonline.database.windows.net,1433; Initial Catalog = NeoOnline; Persist Security Info = False; User ID = NeosinerjiDBManager@neoonline; Password = NeoSnrj2017); MultipleActiveResultSets = True;          Encrypt = True; TrustServerCertificate = False; Connection Timeout = 400;");
            
            
             string connectionString = (@"Server = tcp:91.93.56.75; Initial Catalog = NSEdirneTest2; Persist Security Info = False; User ID = NeosinerjiDBManager@neoonline; Password = NeoSnrj2017); MultipleActiveResultSets = True;Encrypt = True; TrustServerCertificate = True; Connection Timeout = 20;");


            optionsBuilder.UseSqlServer(connectionString);
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                }
            }
            catch (Exception ex)
            {

                tempcontrol = true;
                
               
            }
            if (tempcontrol)
            {
                //EDİRNE SERVER DATABASE BAĞLANTI true 
                //string connectionString2 = (@"Server = tcp: neoonline.database.windows.net,1433; Initial Catalog = NeoOnline; Persist Security Info = False; User ID = NeosinerjiDBManager@neoonline; Password = NeoSnrj2017); MultipleActiveResultSets = True;          Encrypt = True; TrustServerCertificate = False; Connection Timeout = 400;"); 
                
                string connectionString2 = (@"Server = tcp:91.93.56.75; Initial Catalog = NSEdirneTest2; Persist Security Info = False; User ID = NeosinerjiDBManager@neoonline; Password = NeoSnrj2017); MultipleActiveResultSets = True;Encrypt = True; TrustServerCertificate = True; Connection Timeout = 25;");
                optionsBuilder.UseSqlServer(connectionString2);
                try
                {
                    using (var connection = new SqlConnection(connectionString2))
                    {
                        connection.Open();

                    }
                }
                catch (Exception ex)
                {

                    
                }
            }


             

        }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { 
        }
        
        public DbSet<TVMKullanicilar> TVMKullanicilar { get; set; }
        public DbSet<TVMDetay> TVMDetay { get; set; }
        public DbSet<OtoRobotAracFiyatListesi> OtoRobotAracFiyatListesi { get; set; }
        public DbSet<OtoRobotAracMarka> OtoRobotAracMarka{ get; set; }
         public DbSet<TopluSorguKayit> TopluSorguKayit { get; set; }    
         public DbSet<Logs> axalogs { get; set; }    
         public DbSet<AxaSorguSonuclari> axaSorguSonuclaris { get; set; }    
         
    }
}
