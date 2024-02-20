using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtoRobotWeb2.Models.MongoDb
{
    public class logTime
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [Required]
        [BsonElement("FirmaKodu")]
        public int FirmaKodu { get; set; }
        [BsonElement("FirmaAdi")]
        public string FirmaAdi { get; set; }
        [BsonElement("KullaniciKodu")]
        public int KullaniciKodu { get; set; }
        [BsonElement("KullaniciAdi")]
        public string KullaniciAdi { get; set; }

        [Required]
        [BsonElement("SorguMetni")]
        public string SorguMetni { get; set; }
        [Required]
        [BsonElement("TotalSorguBaslangici")]
        public DateTime TotalSorguBaslangici { get; set; }
        [Required]
        [BsonElement("TotalSorguBitis")]
        public DateTime TotalSorguBitis { get; set; }
        [Required]
        [BsonElement("TotalSeconds")]
        public double TotalSeconds { get; set; }
        [BsonElement("logTimeSirketler")]
        public List<logTimeSirketler> logTimeSirketler { get; set; }
        [NotMapped]
        public string TotalSorguBaslangicDisplay { get; set; }
        [NotMapped]
        public string TotalSorguBitisDisplay { get; set; }

    }
    public class logTimeSirketler
    {
        public int TedarikciKodu { get; set; }
        public string TedarikciAdi { get; set; }
        public DateTime SorguBaslangici { get; set; }
        public DateTime SorguBitisi { get; set; }
    }
}
