using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using OtoRobotWeb2.Helpers;
using OtoRobotWeb2.Models.Database;
using OtoRobotWeb2.Models.Inputs;
using OtoRobotWeb2.Models.MongoDb;
using OtoRobotWeb2.Models.Response;
using OtoRobotWeb2.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OtoRobotWeb2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppSettings _appSettings;
        private OfferService _offerService;
        public static List<string> Oem = new List<string>();
        public HomeController(ILogger<HomeController> logger, IOptions<AppSettings> appSettings, OfferService offerService)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _offerService = offerService;
        }
        public IActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var email = identity.FindFirst(ClaimTypes.Name).Value;
            var unvan = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var id = Convert.ToInt32(identity.FindFirst(ClaimTypes.Sid).Value);

            ViewBag.Admin = "";
            var role2 = identity.FindFirst("SuperAdmin");
            var role3 = identity.FindFirst("AdminOnly");

            var SorgulamaTuru = Convert.ToInt32(identity.FindFirst("SorgulamaTuru").Value);
            ViewBag.SorgulamaTuru = SorgulamaTuru;

            bool TopluSorgu = Convert.ToBoolean(identity.FindFirst("TopluSorgu").Value);
            ViewBag.TopluSorgu = TopluSorgu;

            //sepetYetki start sepetYetki
            bool sepetYetki = Convert.ToBoolean(identity.FindFirst("sepetYetki").Value);
            ViewBag.sepetYetki = sepetYetki;
            //sepetYetki end

            if (role2 != null)
                ViewBag.Admin = role2.Value;
            else if (role3 != null)
                ViewBag.Admin = role3.Value;
            ViewBag.Id = id;
            ViewBag.Email = email;
            ViewBag.Unvani = unvan;
            return View();
        }
        public IActionResult IndexExcel()
        {
            var identity = (ClaimsIdentity)User.Identity;
            //sayfaya giriş yetkisi var mı yok mu kontrolu
            var TopluSorgu = Convert.ToBoolean(identity.FindFirst("TopluSorgu").Value);
            if (!TopluSorgu) return RedirectToAction(actionName: "Index");

            var tvmkodu = Convert.ToInt32(identity.FindFirst(ClaimTypes.Sid).Value);
            var SorgulamaTuru = Convert.ToInt32(identity.FindFirst("SorgulamaTuru").Value);
            ViewBag.SorgulamaTuru = SorgulamaTuru;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexExcel(IFormFile formFile, string GrupID, List<int> firmalar)
        {
            // Kullanıcının sayfaya giriş yetkisini kontrol et
            var identity = (ClaimsIdentity)User.Identity;
            var TopluSorgu = Convert.ToBoolean(identity.FindFirst("TopluSorgu").Value);
            if (!TopluSorgu) return NotFound();

            string oemCount = "0";
            if (formFile != null)
            {
                try
                {
                    // wwwroot/File dizinine dosya kaydetme işlemi
                    var currentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "File");
                    if (!Directory.Exists(currentPath))
                    {
                        Directory.CreateDirectory(currentPath); // Dizin yoksa oluştur
                    }

                    // Benzersiz dosya ismi oluştur
                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(formFile.FileName)}";
                    var filePath = Path.Combine(currentPath, uniqueFileName);

                    // Dosyayı kaydet
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    // Excel dosyasını okuyup içeriğini al
                    List<ExcelOEM> list;
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var excelReader = new ExcelReader();
                        list = excelReader.Aktar(stream);
                    }

                    // Dosya okunduktan sonra güvenli bir şekilde sil
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Dosya silinirken hata oluştu: " + ex.Message);
                    }

                    oemCount = list.Count.ToString();

                    // Eğer liste 1-50 arasında ise işleme devam et
                    if (list.Count > 0 && list.Count <= 50)
                    {
                        var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("sid"))?.Value);
                        var kullaniciKodu = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("primarysid"))?.Value);

                        var socketResponse = new SocketResponse
                        {
                            OemNumaralari = list.Select(x => x.OemKodu).ToList(),
                            GrupID = GrupID,
                            UserId = userId,
                            KullaniciKodu = kullaniciKodu,
                            SorgulanacakFirmalar = firmalar,
                            StokVarmi = true,
                            SorguId = Guid.NewGuid().ToString()
                        };

                        socketResponse.setProcessType(enum_Process.MultiOffer);

                        // IIS uyumlu Task kullanımı
                        Task.Run(() =>
                        {
                            if (RequestController.localdsocketprocescalistir)
                            {
                                new SocketProcess(socketResponse.UserId).SendRequestOffer(JsonConvert.SerializeObject(socketResponse));
                            }
                            else
                            {
                                var tvmDetay = Tools.TVMDetays.FirstOrDefault(x => x.Kodu == userId);
                                if (tvmDetay != null)
                                {
                                    new HttpRequestProcess().HttpGetSendOffer(socketResponse, tvmDetay.WebAdresi).Wait();
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata oluştu: " + ex.Message);
                    return BadRequest("Dosya işlenirken bir hata oluştu.");
                }
            }

            return Ok(oemCount);
        }


        //[HttpPost]
        //public IActionResult IndexExcel(IFormFile formFile, string GrupID, List<int> firmalar)
        //{
        //    //sayfaya giriş yetkisi var mı yok mu kontrolu
        //    var identity = (ClaimsIdentity)User.Identity;
        //    var TopluSorgu = Convert.ToBoolean(identity.FindFirst("TopluSorgu").Value);
        //    if (!TopluSorgu) return NotFound();

        //    string oemCount = "0";
        //    if (formFile != null)
        //    {
        //        var currentpath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        //        var filePath = System.IO.Path.Combine(currentpath, "File", formFile.FileName);

        //        using (var stream = System.IO.File.Create(filePath))
        //        {
        //            var qwe = formFile.CopyToAsync(stream);
        //            qwe.Wait();
        //        }
        //        ExcelReader excelReader = new ExcelReader();

        //        var list = excelReader.Aktar(filePath);
        //        System.IO.File.Delete(filePath);
        //        oemCount = list.Count.ToString();
        //        if (list.Count > 0 && list.Count <= 50)
        //        {

        //            //var guidID = Guid.NewGuid().ToString();
        //            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("sid")).Value);
        //            var kullanicikodu = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("primarysid")).Value);

        //            SocketResponse socketResponse = new SocketResponse();
        //            socketResponse.OemNumaralari = list.Select(x => x.OemKodu).ToList();
        //            List<string> list1 = list.Select(x => x.OemKodu).ToList();
        //            Oem = list1;
        //            socketResponse.GrupID = GrupID;
        //            socketResponse.UserId = userId;
        //            socketResponse.KullaniciKodu = kullanicikodu;
        //            socketResponse.SorgulanacakFirmalar = firmalar;
        //            socketResponse.StokVarmi = true;
        //            socketResponse.SorguId = Guid.NewGuid().ToString();
        //            socketResponse.setProcessType(enum_Process.MultiOffer);


        //            if (RequestController.localdsocketprocescalistir)
        //                new Thread(new ThreadStart(() =>
        //                {
        //                    new SocketProcess(socketResponse.UserId).SendRequestOffer(JsonConvert.SerializeObject(socketResponse));

        //                })).Start();
        //            else
        //                new Thread(new ThreadStart(() =>
        //                {
        //                    new HttpRequestProcess().HttpGetSendOffer(socketResponse, Tools.TVMDetays.Where(x => x.Kodu == userId).FirstOrDefault().WebAdresi).Wait();
        //                })).Start();
        //            //foreach (var item in list)
        //            //{
        //            //    SocketResponse socketResponse = new SocketResponse();
        //            //    socketResponse.OEMNumarasi = item.OemKodu;
        //            //    socketResponse.GrupID = GrupID;
        //            //    socketResponse.UserId = userId;
        //            //    socketResponse.KullaniciKodu = kullanicikodu;
        //            //    socketResponse.SorgulanacakFirmalar = firmalar;
        //            //    socketResponse.SorguId = Guid.NewGuid().ToString();
        //            //    socketResponse.setProcessType(enum_Process.MultiOffer);

        //            //    new HttpRequestProcess().HttpGetSendOffer(socketResponse, Tools.TVMDetays.Where(x => x.Kodu == userId).FirstOrDefault().WebAdresi).Wait();
        //            //}
        //        }


        //    }
        //    return Ok(oemCount);
        //}

        [Authorize(Policy = "AdminOnly")]
        public IActionResult LogIndex()
        {
            var identity = (ClaimsIdentity)User.Identity;

            var role2 = identity.FindFirst("SuperAdmin");
            if (role2 != null)
                if (role2.Value == "SuperAdmin")
                    return RedirectToAction("AdminLogIndex");
            return View(GetLogIndex());
        }
        //[Authorize(Roles = "100,205")] 
        [Authorize(Policy = "SuperAdmin")]
        public IActionResult AdminLogIndex()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var role2 = identity.FindFirst("Admin");
            if (role2 != null)
                if (role2.Value != "SuperAdmin")
                    return RedirectToAction("LogIndex");
            var id = Convert.ToInt32(identity.FindFirst(ClaimTypes.Sid).Value);
            ViewBag.Id = id;
            return View(GetLogIndex());
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public IActionResult LogIndex(int tvmkodu, int kullanicikodu, string sorgu, DateTime date_baslangic, DateTime date_bitis, DateTime date_ozet)
        {
            return View(PostLogIndex(tvmkodu, kullanicikodu, sorgu, date_baslangic, date_bitis, date_ozet));
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpPost]
        // İlk aksiyon, sorgu parametreleri ile
        public IActionResult AdminLogIndex(int tvmkodu, int kullanicikodu, string sorgu, DateTime date_baslangic, DateTime date_bitis, DateTime date_ozet)
        {
            return View(PostLogIndex(tvmkodu, kullanicikodu, sorgu, date_baslangic, date_bitis, date_ozet));
        }
     
        void SetRadioFlags(OtoLogResponse log, string sorgu)
        {
            log.rd_detayli = sorgu == "sorguturu1";
            log.rd_ozet = sorgu == "sorguturu2";
            log.rd_multiozet = sorgu == "sorguturu3";
            log.rd_multiozetWeb = sorgu == "sorguturu4";
        }

        OtoLogResponse GetLogIndex()
        {

            var identity = (ClaimsIdentity)User.Identity;

            var id = Convert.ToInt32(identity.FindFirst(ClaimTypes.Sid).Value);
            ViewBag.Id = id;
            List<SelectListItem> firmalist = new List<SelectListItem>();

            firmalist = _offerService.GetUserListAdmin(id);
            List<logTime> logTime = new List<logTime>();

            //var res = new logTimeCollection(id.ToString()).GetAllLogTime();
            //logTime.AddRange(res);

            //var tempsubusers = _offerService.SubUsers(id);
            //foreach (var item in tempsubusers)
            //{
            //    res = new logTimeCollection(item.ToString()).GetAllLogTime();
            //    logTime.AddRange(res);
            //}
            //var groups = logTime.GroupBy(n => n.KullaniciAdi)
            //             .Select(n => new PersonDetailItem
            //             {
            //                 Name = n.Key,
            //                 Count = n.Count()
            //             })
            //             .OrderBy(n => n.Count).ToList();
        

            OtoLogResponse logResponse = new OtoLogResponse { TVMKullanicilars = firmalist };
        
            logResponse.Tvmdetay = _offerService.GetFirmaList();
            logResponse.TvmKodu = id;
            var temptvm = logResponse.Tvmdetay.Where(x => x.Value == id.ToString()).FirstOrDefault();
            logResponse.KullaniciKodu = id;
            temptvm.Selected = true;
            logResponse.Unvani = temptvm.Text;
            logResponse.rd_ozet = true;
            logResponse.Date_Baslangic = DateTime.Now.AddDays(-7);
            logResponse.Date_Bitis = DateTime.Now;
            logResponse.Date_Ozet = DateTime.Now;

            return logResponse;
        }

        OtoLogResponse PostLogIndex(int tvmkodu, int kullanicikodu, string sorgu, DateTime date_baslangic, DateTime date_bitis, DateTime date_ozet)
        {
            Console.WriteLine("Sorgu türü geldi: " + sorgu);
            var identity = (ClaimsIdentity)User.Identity;

           
            var id = Convert.ToInt32(identity.FindFirst(ClaimTypes.Sid).Value);
            ViewBag.Id = id;
            List<SelectListItem> firmalist = new List<SelectListItem>();

            firmalist = _offerService.GetUserListAdmin(tvmkodu);
            firmalist.Insert(0, new SelectListItem { Text = "Kullanıcı Seçiniz", Value = "" });


            OtoLogResponse logResponse = new OtoLogResponse { TVMKullanicilars = firmalist };
            SetRadioFlags(logResponse, sorgu);
            //logResponse.rd_multiozet = sorgu == "sorguturu4";
            logResponse.Tvmdetay = _offerService.GetFirmaList();
            logResponse.TvmKodu = tvmkodu;
            var temptvm = logResponse.Tvmdetay.Where(x => x.Value == tvmkodu.ToString()).FirstOrDefault();
            temptvm.Selected = true;
            var diffdate = (date_bitis - date_baslangic).TotalDays;
            logResponse.Unvani = temptvm.Text;
            logResponse.KullaniciKodu = tvmkodu;
            logResponse.Date_Baslangic = DateTime.Now.AddDays(-7);
            logResponse.Date_Bitis = DateTime.Now;
            logResponse.Date_Ozet = DateTime.Now;
            if (sorgu == "sorguturu1")
            {
                logResponse.rd_detayli = true;
                if (diffdate > 30)
                {
                    logResponse.HataMesaji = "En fazla 30 günlük listeleme yapılmaktadır. ";
                }
                else if (date_baslangic.Date > date_bitis.Date)
                {
                    logResponse.HataMesaji = "Başlangıç tarihi bitiş tarihinden sonra olamaz.";
                }
                else
                {
                    var temptvmkodu = _offerService.GetTVMKoduByKullaniciKodu(kullanicikodu);
                    var res = new logTimeCollection(temptvmkodu.ToString()).GetAllLogTime();
                    res = res.Where(x => x.KullaniciAdi != "multilog").ToList();
                    res = res.Where(x => x.TotalSorguBaslangici.Date >= date_baslangic.Date && x.TotalSorguBaslangici.Date <= date_bitis.Date).ToList();
                    res = res.OrderByDescending(x => x.TotalSorguBaslangici).ToList();
                    logResponse.LogTimes = res;
                    logResponse.Date_Baslangic = date_baslangic;
                    logResponse.Date_Bitis = date_bitis;
                    logResponse.Date_Ozet = DateTime.Now;
                }
                var tempuser = firmalist.Where(x => x.Value == kullanicikodu.ToString()).FirstOrDefault();
                if (tempuser != null)
                {
                    tempuser.Selected = true;
                    logResponse.Unvani = tempuser.Text;
                    var temptvmkullanici = _offerService.GetTVMKullanicilar(kullanicikodu);
                    logResponse.KullaniciKodu = temptvmkullanici.TVMKodu;

                }
            }
            else if (sorgu == "sorguturu2")
            {
                var res = new logTimeCollection(tvmkodu.ToString()).GetAllLogTime();
                res = res.Where(x => x.KullaniciAdi != "multilog").ToList();
                logResponse.rd_ozet = true;

                List<logTime> logTime = new List<logTime>();
                res = res.Where(x => x.TotalSorguBaslangici.Date == date_ozet.Date).ToList();
                logTime.AddRange(res);

                var tempsubusers = _offerService.SubUsers(tvmkodu);
                foreach (var item in tempsubusers)
                {
                    res = new logTimeCollection(item.ToString()).GetAllLogTime();
                    res = res.Where(x => x.TotalSorguBaslangici.Date == date_ozet.Date).ToList();
                    logTime.AddRange(res);
                }

                var groups = logTime.GroupBy(n => n.KullaniciAdi)
                             .Select(n => new PersonDetailItem
                             {
                                 Name = n.Key,
                                 Count = n.Count()
                             })
                             .OrderBy(n => n.Count).ToList();

                logResponse.PersonDetails = groups;
                logResponse.Total = logTime.Count;
                logResponse.Unvani = temptvm.Text;
                logResponse.KullaniciKodu = tvmkodu;
                logResponse.Date_Ozet = date_ozet;


            }
            else if (sorgu == "sorguturu3")
            {
                logResponse.rd_multiozet = true;
                if (diffdate > 30)
                {
                    logResponse.HataMesaji = "En fazla 30 günlük listeleme yapılmaktadır. ";
                }
                else if (date_baslangic.Date > date_bitis.Date)
                {
                    logResponse.HataMesaji = "Başlangıç tarihi bitiş tarihinden sonra olamaz.";
                }
                else
                {

                    var res = new logTimeCollection(tvmkodu.ToString()).GetAllLogTime().Where(x => x.TotalSorguBaslangici.Date >= date_baslangic.Date && x.TotalSorguBaslangici.Date <= date_bitis.Date).ToList(); ;

                    res = res.Where(x => x.KullaniciAdi == "multilog").ToList();
                    //res = res.Where(x => x.TotalSorguBaslangici.Date >= date_baslangic.Date && x.TotalSorguBaslangici.Date <= date_bitis.Date).ToList();
                    res = res.OrderByDescending(x => x.TotalSorguBaslangici).ToList();
                    logResponse.LogTimes = res;
                    logResponse.Date_Baslangic = date_baslangic;
                    logResponse.Date_Bitis = date_bitis;
                    logResponse.Date_Ozet = DateTime.Now;
                }
                var tempuser = firmalist.Where(x => x.Value == kullanicikodu.ToString()).FirstOrDefault();
                if (tempuser != null)
                {
                    tempuser.Selected = true;
                    logResponse.Unvani = tempuser.Text;
                    var temptvmkullanici = _offerService.GetTVMKullanicilar(kullanicikodu);
                    logResponse.KullaniciKodu = temptvmkullanici.TVMKodu;

                }
            }
            else if (sorgu == "sorguturu4")
            {
                logResponse.rd_multiozetWeb = true;

                if (diffdate > 30)
                {
                    logResponse.HataMesaji = "En fazla 30 günlük listeleme yapılmaktadır.";
                }
                else if (date_baslangic.Date > date_bitis.Date)
                {
                    logResponse.HataMesaji = "Başlangıç tarihi bitiş tarihinden sonra olamaz.";
                }
                else
                {
                    // OfferService içindeki metodu çağırıyoruz
                    var logs = _offerService.GetWebDataLogsWithJoin(date_baslangic, date_bitis);

                    logResponse.WebDataLogs = logs;
                    logResponse.Date_Baslangic = date_baslangic;
                    logResponse.Date_Bitis = date_bitis;
                    logResponse.Date_Ozet = DateTime.Now;
                }
            }


            if (logResponse.LogTimes != null)
            {
                foreach (var item in logResponse.LogTimes)
                {
                    item.TotalSorguBaslangici = item.TotalSorguBaslangici.AddHours(3);
                    item.TotalSorguBitis = item.TotalSorguBitis.AddHours(3);
                    item.TotalSorguBaslangicDisplay = item.TotalSorguBaslangici.ToString("dd.MM.yyyy HH:mm:ss");
                    item.TotalSorguBitisDisplay = item.TotalSorguBitis.ToString("dd.MM.yyyy HH:mm:ss");
                }
            }
            return logResponse;
        }
        [Authorize(Policy = "SuperAdmin")]
        [HttpPost]
        public IActionResult UserList(int id = 0)
        {
            var firmalist = _offerService.GetUserListAdmin(id);

            return Json(firmalist);
        }
        [Authorize(Policy = "SuperAdmin")]
        [HttpPost]
        public IActionResult GenelOzet(DateTime date_baslangic, DateTime date_bitis)
        {
            var tvmlist = _offerService.GetFirmaList();
            List<ExcelPersonDetailItem> lst = new List<ExcelPersonDetailItem>();
            foreach (var itemTvm in tvmlist)
            {
                int tvmkodu = Convert.ToInt32(itemTvm.Value);
                var res = new logTimeCollection(tvmkodu.ToString()).GetAllLogTime();

                List<logTime> logTime = new List<logTime>();
                res = res.Where(x => x.TotalSorguBaslangici.Date >= date_baslangic.Date && x.TotalSorguBaslangici.Date <= date_bitis.Date).ToList();

                logTime.AddRange(res);

                var tempsubusers = _offerService.SubUsers(tvmkodu);
                foreach (var item in tempsubusers)
                {
                    res = new logTimeCollection(item.ToString()).GetAllLogTime();
                    res = res.Where(x => x.TotalSorguBaslangici.Date >= date_baslangic.Date && date_bitis.Date <= x.TotalSorguBaslangici.Date).ToList();
                    logTime.AddRange(res);
                }

                if (logTime.Count > 0)
                    lst.Add(new ExcelPersonDetailItem { Firma = itemTvm.Text, SorguSayisi = logTime.Count().ToString() });
            }
            if (lst.Count > 0)
            {
                string reportname = $"Log_{Guid.NewGuid():N}.xlsx";
                var exportbytes = Tools.ExporttoExcel<ExcelPersonDetailItem>(lst, reportname, date_baslangic, date_bitis);
                return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
            }
            else
            {
                return Ok("Sonuç Bulunamadı");
            }
        }
        public IActionResult LogDetails(DateTime startDate, DateTime endDate)
        {
            var logs = _offerService.GetWebDataLogsWithJoin(startDate, endDate);
            return View(logs); // logs verisini view'a gönder
        }

        [HttpGet]
        public async Task<IActionResult> DownloadOtorobotSorguFile()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "File/Sablon/OtoRobot_topluSorgu.xlsx");

            if (!System.IO.File.Exists(path))
            {
                
                return NotFound("Dosya bulunamadı: " + path);
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(path); // "await" ekledik

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OtoRobot_topluSorgu.xlsx");
        }

    }

}
