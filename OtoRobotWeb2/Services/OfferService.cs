
using Microsoft.AspNetCore.Mvc.Rendering;
using OtoRobotWeb2.Helpers;
using OtoRobotWeb2.Models;
using OtoRobotWeb2.Models.Database;
using OtoRobotWeb2.Models.Inputs;
using OtoRobotWeb2.Models.Response;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OtoRobotWeb2.Services
{
    public class OfferService
    {
        private DataContext _context;
        private StorageService _storageService;

        public OfferService(Models.DataContext context)
        {
            _context = context;
        }
        public TVMKullanicilar Login(Users users)
        {
            string hashedPassword = Encryption.HashPassword(users.Password);
            var res = _context.TVMKullanicilar.Where(a => a.Email == users.Email && a.Sifre == hashedPassword && a.Durum==1).FirstOrDefault();
            if (res == null )
            {
                return null;
            } 
            return res;
        }
        public string SetSession(int kullaniciKodu)
        {
            string result = Guid.NewGuid().ToString();
            var res = _context.TVMKullanicilar.Where(a => a.KullaniciKodu == kullaniciKodu).FirstOrDefault();
            if (res != null)
            {
                res.SessionToken = result;
                _context.SaveChanges();
            }
            return result;
        }
        public void Logout(int tvmkodu)
        {
            var res = _context.TVMKullanicilar.Where(a => a.TVMKodu == tvmkodu).FirstOrDefault();
            if (res != null)
            { 
                res.SessionToken = "";
                _context.SaveChanges();
            } 
        }
        public TVMKullanicilar Logout(string username)
        {
            var res = _context.TVMKullanicilar.Where(a => a.Email == username).FirstOrDefault();
            if (res != null)
            {
                res.SessionToken = "";
                _context.SaveChanges();
                return res;
            }
            return null;
        }
        public bool CheckSession(string token)
        {

            var res = _context.TVMKullanicilar.Where(a => a.SessionToken == token).FirstOrDefault();
            if (res != null)
            {
                return false;
            }
            return true;
        }
        public List<SelectListItem> GetUserList()
        {
            var res = _context.TVMKullanicilar.Select(x => new SelectListItem { Text = x.Adi + " " + x.Soyadi, Value = x.TVMKodu.ToString(), }).ToList();
            if (res == null)
            {
                return null;
            }
            return res;
        }
        public TVMKullanicilar GetTVMKullanicilar(int kullanicikodu)
        {
            var res = _context.TVMKullanicilar.Where(x => x.KullaniciKodu == kullanicikodu).FirstOrDefault();
            if (res == null)
            {
                return null;
            }
            return res;
        }
        public List<SelectListItem> GetFirmaList()
        {
            //var res = _context.TVMDetay.Select(x => new SelectListItem { Text = x.Unvani, Value = x.Kodu.ToString()}).ToList();
            var res = _context.TVMDetay.Where(x => x.BagliTVMKodu == null).Select(x => new SelectListItem { Text = x.Unvani, Value = x.Kodu.ToString() }).ToList();
            if (res == null)
            {
                return null;
            }
            return res;
        }
        public TVMDetay GetFirma(int kod)
        { 
            var res = _context.TVMDetay.Where(x => x.Kodu == kod).FirstOrDefault();
            if (res == null)
            {
                return null;
            }
            return res;
        }
        public List<SelectListItem> GetUserListAdmin(int tvmkodu)
        {

            var res = (from k in _context.TVMKullanicilar join d in _context.TVMDetay on k.TVMKodu equals d.Kodu where d.BagliTVMKodu == tvmkodu select new SelectListItem { Text = k.Adi + " " + k.Soyadi, Value = k.KullaniciKodu.ToString() }).ToList();
            //var asdqwe = _context.TVMDetay.Where(x => x.Kodu == tvmkodu).FirstOrDefault();
            var tempres = (from k in _context.TVMKullanicilar join d in _context.TVMDetay on k.TVMKodu equals d.Kodu where d.Kodu == tvmkodu && !d.BagliTVMKodu.HasValue select new SelectListItem { Text = k.Adi + " " + k.Soyadi, Value = k.KullaniciKodu.ToString() }).FirstOrDefault();
            if (tempres != null)
            {
                res.Insert(0, tempres);
            }

            if (res == null)
            {
                return null;
            }
            return res;
        }

        public bool isAdmin(int tvmkodu)
        {
            var tempres = (from k in _context.TVMKullanicilar join d in _context.TVMDetay on k.TVMKodu equals d.Kodu where d.Kodu == tvmkodu && !d.BagliTVMKodu.HasValue select new SelectListItem { Text = k.Adi + " " + k.Soyadi, Value = k.KullaniciKodu.ToString() }).FirstOrDefault();
            if (tempres != null)
            {
                return true;
            }
            return false;
        }
        public List<int> SubUsers(int tvmkodu)
        {
            var tempres = (from k in _context.TVMKullanicilar join d in _context.TVMDetay on k.TVMKodu equals d.Kodu where d.BagliTVMKodu.Value == tvmkodu select k.TVMKodu).ToList();
            if (tempres != null)
            {
                return tempres;
            }
            return new List<int>();
        }
        public int GetTVMKoduByKullaniciKodu(int kullanicikodu)
        {
            var tempres = _context.TVMKullanicilar.Where(x => x.KullaniciKodu == kullanicikodu).FirstOrDefault();
            if (tempres != null)
            {
                return tempres.TVMKodu;
            }
            return 0;
        }

        public List<ResponseItem> OrjinalParcaSorgu(string oem)
        {
            var res = (from f in _context.OtoRobotAracFiyatListesi
                       join m in _context.OtoRobotAracMarka on f.MarkaKodu equals m.Kodu
                       where f.OrjinalNo == oem
                       select new ResponseItem
                       {
                           Marka = m.MarkaAdi,
                           ListeFiyat = f.ListeFiyati.ToString().Replace(".", ","),
                           OemNo = f.OrjinalNo,
                           KdvLi = f.KdvDahil.ToString().Replace(".", ","),
                           SirketAd = "Orjinal",
                           UrunAd = f.ParcaAdi,
                           YanitVarmi = 0,
                           KdvSiz = "",
                           SirketKodu = 0,
                           Stok = "",
                           StokNo = ""
                       }).ToList();

            return res;
        }
        public List<TVMDetay>  DoldurTvmDetay()
        { 
            var res = _context.TVMDetay.ToList();
            if (res == null)
            {
                return null;
            }
            return res;
        }
    }
}
