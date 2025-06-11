using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OtoRobotWeb2.Helpers;
using OtoRobotWeb2.Models.Inputs;
using OtoRobotWeb2.Models.Response;
using OtoRobotWeb2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Security.Claims;
using SharpCompress.Common;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using MessagePack;
using OtoRobotWeb2.Models.Database;
using OtoRobotWeb2.Models;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using MathNet.Numerics;
using Microsoft.Data.SqlClient;
using System.Data;

namespace OtoRobotWeb2.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private OfferService _offerService;
        private readonly AppSettings _appSettings;  //asdad 
        int userid = 0; //adasd

        private DataContext _context;    
        //önemli olan yerler 
        public static bool localdsocketprocescalistir = false;//local calısmak icin true, canlıya alırken false yapılmalı!!!!sorguyu socketprcess projesine gonderme.
        public static bool localdconncalistir = false;//local calısmak icin true, canlıya alırken false yapılmalı!!!! sorguyu conn projesine gonderme
        public static List<ResponseItem> yanitVarMi1List = new List<ResponseItem>(); // Sonuc bulunamayan listesi
        public static List<ResponseItem> yanitVarMi2List = new List<ResponseItem>(); // Sonuc bulunamayan listesi
        public RequestController(OfferService offerService)
        {
            _offerService = offerService;
        }
        #region login process
        [HttpGet("sendlogin")]
        public IActionResult SendLogin()
        {
            return Ok(true);
        }
        [HttpGet("checklogin")]
        public IActionResult CheckLogin()
        {
            SocketResponse socketResponse = new SocketResponse();
            socketResponse.setProcessType(enum_Process.LoginResult);
            socketResponse.UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("sid")).Value);
            SocketResponse res;
            if (localdsocketprocescalistir)
                res = new SocketProcess(socketResponse.UserId).SendRequestOffer(JsonConvert.SerializeObject(socketResponse));
            else
                res = new HttpRequestProcess().HttpGetCheckLogin(socketResponse, Tools.TVMDetays.Where(x => x.Kodu == socketResponse.UserId).FirstOrDefault().WebAdresi).Result;
            try
            {
                if (res != null)
                    return Ok(new { Success = true, Logins = res.Logins, Data = res });
                else
                    return Ok(new { Success = true, Data = res });
            }
            catch (System.Exception)
            {
                return Ok(new { Success = true, Data = res });

            }
        }



        [HttpGet("relogin")]
        public IActionResult ReLogin(int sirketkodu)
        {
            SocketResponse socketResponse = new SocketResponse();
            socketResponse.setProcessType(enum_Process.ReLogin);
            socketResponse.UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("sid")).Value);
            //socketResponse.UserId = sirketkodu;
            SocketResponse res;
            if (localdsocketprocescalistir)
                res = new SocketProcess(socketResponse.UserId).SendRequestOffer(JsonConvert.SerializeObject(socketResponse));
            else
                res = new HttpRequestProcess().HttpGetCheckLogin(socketResponse, Tools.TVMDetays.Where(x => x.Kodu == socketResponse.UserId).FirstOrDefault().WebAdresi).Result;
            return Ok(true);
        }









        #endregion
        #region parça process
        //checkProses Deneme
        [HttpGet("checkoffer")]
        [AllowAnonymous]
        public IActionResult CheckOffer([FromQuery] RequestInput key)
        {
            SocketResponse socketResponse = new SocketResponse();
            socketResponse.setProcessType(enum_Process.OfferResult);
            socketResponse.UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("sid")).Value);
            socketResponse.KullaniciKodu = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("primarysid")).Value);
            socketResponse.SorguId = key.SorguId;
            SocketResponse res;

            if (localdsocketprocescalistir)
                res = new SocketProcess(socketResponse.UserId).SendRequestOffer(JsonConvert.SerializeObject(socketResponse));
            else
                res = new HttpRequestProcess().HttpGetCheckOffer(socketResponse, Tools.TVMDetays.Where(x => x.Kodu == socketResponse.UserId).FirstOrDefault().WebAdresi).Result;

            if (res != null)
                return Ok(new { Success = true, OfferResult = res.OfferResult, Data = res });
            else
                return Ok(new { Success = true, Data = res });

        }

        [HttpGet("checkmultioffer")]
        [AllowAnonymous]
        public IActionResult CheckMultiOffer([FromQuery] RequestInput key)
        {
            yanitVarMi1List = new List<ResponseItem>();
            yanitVarMi2List = new List<ResponseItem>();
            SocketResponse socketResponse = new SocketResponse();
            socketResponse.setProcessType(enum_Process.MultiOfferResult);
            socketResponse.UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("sid")).Value);
            socketResponse.KullaniciKodu = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("primarysid")).Value);
            socketResponse.SorguId = key.SorguId;
            socketResponse.GrupID = key.GrupID;
            socketResponse.SorgulanacakFirmalar = key.SorgulanacakFirmalar;
            ;

            SocketResponse res;

            if (localdsocketprocescalistir)
                res = new SocketProcess(socketResponse.UserId).SendRequestOffer(JsonConvert.SerializeObject(socketResponse));
            else
                res = new HttpRequestProcess().HttpGetCheckOffer(socketResponse, Tools.TVMDetays.Where(x => x.Kodu == socketResponse.UserId).FirstOrDefault().WebAdresi).Result;

            List<ResponseItemExcel> list = new List<ResponseItemExcel>();
            List<ResponseItem> liste2 = new List<ResponseItem>();


            if (res != null)
            {


                if (res.OfferResult != null)

                {
                    liste2 = res.OfferResult.Where(x => x.YanitVarmi == 0).ToList();
                    liste2 = liste2.Where(x => x.Stok.ToLower().Contains("var")).ToList();
                }


                if (res.isFinished)
                {
                    string reportname = Guid.NewGuid().ToString();
                    var currentpath = AppContext.BaseDirectory;
                    ResponseItemExcel responseItemExcel = null;



                    if (res.OfferResult != null)
                    {
                        var yanitVarMi0 = res.OfferResult.Where(x => x.YanitVarmi == 0).ToList();
                        var yanitVarMi1 = yanitVarMi0.Where(x => x.Stok.ToLower().Contains("yok")).ToList();
                        yanitVarMi0 = yanitVarMi0.Where(x => x.Stok.ToLower().Contains("var")).ToList();
                        yanitVarMi1.AddRange(res.OfferResult.Where(x => x.YanitVarmi == 1).ToList());
                        var yanitVarMi2 = res.OfferResult.Where(x => x.YanitVarmi == 2).ToList();
                        if (yanitVarMi0.Count > 0)
                        {
                            foreach (var item in yanitVarMi0)
                            {
                                responseItemExcel = new ResponseItemExcel();
                                responseItemExcel.Stok = item.Stok.Contains("Var") ? "Var" : "Yok";
                                if (item.Stok.ToLower() == "yok") continue;
                                responseItemExcel.Marka = item.Marka;
                                responseItemExcel.SorgulananOem = item.SorgulananOem;
                                responseItemExcel.SirketAd = item.SirketAd;
                                responseItemExcel.KdvLi = item.KdvLi;
                                responseItemExcel.KdvSiz = item.KdvSiz;
                                responseItemExcel.ListeFiyat = item.ListeFiyat;
                                responseItemExcel.OemNo = item.OemNo;
                                responseItemExcel.StokNo = item.StokNo;
                                responseItemExcel.UrunAd = item.UrunAd;

                                list.Add(responseItemExcel);

                            }


                        }
                        if (yanitVarMi1.Count > 0)
                            yanitVarMi1List = yanitVarMi1;
                        if (yanitVarMi2.Count > 0)
                            yanitVarMi2List = yanitVarMi2;
                    }


                    var exportbytes = Tools.ExporttoExcelOEM(list, reportname);
                    var temp = JsonConvert.SerializeObject(exportbytes);

                    var path = AppContext.BaseDirectory + "\\File\\Download\\" + reportname + ".json";

                    System.IO.File.WriteAllText(path, temp);


                    //var exportbytes = Tools.ExporttoExcelOEM(list, reportname);



                    return Ok(new { Success = true, OfferResult = res.OfferResult, Data = res, Link = reportname, Count = list.Count });

                }
                else
                    return Ok(new { Success = true, OfferResult = res.OfferResult, Data = res, Count = liste2.Count });
            }

            else
                return Ok(new { Success = true, Data = res, Count = 0 });

        }
        [HttpGet("dmo")]
        [AllowAnonymous]
        public IActionResult DownloadMultiOffer([FromQuery] string filename, string orjinalname)
        {
            var path = AppContext.BaseDirectory + "\\File\\Download\\" + filename + ".json";
            string Tarih = DateTime.Now.ToString("yyyy/MM/dd");
            string reportname = "Otorobot_topluSorgu_" + orjinalname + "_" + Tarih + ".xlsx";
            var qwe = System.IO.File.ReadAllText(path);
            var exportbytes = JsonConvert.DeserializeObject<byte[]>(qwe);
            System.IO.File.Delete(path);
            return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
        }

        [HttpGet("sendoffer")]
        [AllowAnonymous]
        public IActionResult SendOffer([FromQuery] RequestInput key)
        {


            if (key.SorgulanacakFirmalar == null || key.OemNumarasi == null)
                return BadRequest();

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("sid")).Value);
            SocketResponse socketResponse = new SocketResponse();
            socketResponse.OEMNumarasi = key.OemNumarasi;
            socketResponse.StokVarmi = key.StokVarmi;
            socketResponse.SorguId = key.SorguId;
            socketResponse.UserId = userId;
            socketResponse.SorgulanacakFirmalar = key.SorgulanacakFirmalar;
            socketResponse.KullaniciKodu = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("primarysid")).Value);
            socketResponse.setProcessType(enum_Process.Offer);

            if (localdsocketprocescalistir)
                new Thread(new ThreadStart(() =>
                {
                    new SocketProcess(socketResponse.UserId).SendRequest(JsonConvert.SerializeObject(socketResponse));
                })).Start();

            else
                new Thread(new ThreadStart(() =>
                {
                    new HttpRequestProcess().HttpGetSendOffer(socketResponse, Tools.TVMDetays.Where(x => x.Kodu == userId).FirstOrDefault().WebAdresi).Wait();
                })).Start();


            return Ok(true);
        }
        [HttpPost("sendorder")]
        [AllowAnonymous]
        public IActionResult SendOrder(RequestInput key)
        {
            var identity = (ClaimsIdentity)User.Identity;
            if (!Convert.ToBoolean(identity.FindFirst("sepetYetki").Value))
                return BadRequest();

            SocketResponse socketResponse = new SocketResponse();
            socketResponse.OEMNumarasi = key.OemNumarasi;
            socketResponse.UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("sid")).Value);
            socketResponse.SorgulanacakFirmalar = key.SorgulanacakFirmalar;
            socketResponse.setProcessType(enum_Process.Order);
            socketResponse.KullaniciKodu = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("primarysid")).Value);
            Task<SocketResponse> temp = null;
            SocketResponse res = null;
            if (localdsocketprocescalistir)
                res = new SocketProcess(socketResponse.UserId).SendRequestOffer(JsonConvert.SerializeObject(socketResponse));
            else
            {
                temp = new HttpRequestProcess().HttpGetSendOrder(socketResponse, Tools.TVMDetays.Where(x => x.Kodu == socketResponse.UserId).FirstOrDefault().WebAdresi);
                temp.Wait();
                res = temp.Result;
            }
            if (res != null)
            {
                res.KullaniciKodu = 0;
                res.UserId = 0;
                res.ProcessType = 0;
            }
            return Ok(res);
        }

        //[HttpPost("SaveData")]
        //public IActionResult SaveData([FromBody] TopluSorguKayit data)
        //{
        //    _context = new DataContext();

        //    try
        //    {
        //        // Gelen veri için bir liste oluştur
        //        List<TopluSorguKayit> list = new List<TopluSorguKayit>
        //{
        //    data // Gelen veriyi listeye ekle
        //};

        //        // Listeyi döngüyle işle
        //        foreach (var item in list)
        //        {
        //            var today = DateTime.Today; // Bugünün tarihi

        //            // Mevcut kaydı kontrol et
        //            var tempsorgu = _context.TopluSorguKayit
        //                .FirstOrDefault(x => x.SorgulananOemNo == item.SorgulananOemNo
        //                                     && x.SirketAd == item.SirketAd);

        //            if (tempsorgu != null)
        //            {
        //                // Tarih farkını hesapla
        //                var dateDifference = Math.Abs((tempsorgu.Tarih.Date - today).TotalDays);

        //                if (dateDifference >= 30)
        //                {
        //                    // Eğer 30 günden fazla fark varsa, mevcut kaydı güncelle
        //                    tempsorgu.Tarih = today;
        //                    tempsorgu.SorgulananOemNo = item.SorgulananOemNo;
        //                    tempsorgu.SirketAd = item.SirketAd;
        //                    tempsorgu.Stok = item.Stok;
        //                    tempsorgu.OemNo = item.OemNo;
        //                    tempsorgu.StokNo = item.StokNo;
        //                    tempsorgu.UrunAd = item.UrunAd;
        //                    tempsorgu.Marka = item.Marka;
        //                    tempsorgu.kdvLi = item.kdvLi;
        //                    tempsorgu.kdvSiz = item.kdvSiz;
        //                    tempsorgu.ListeFiyati = item.ListeFiyati;

        //                    // Güncellemeyi kaydet
        //                    _context.SaveChanges();
        //                }
        //                else
        //                {
        //                    // Eğer aynı tarih ile kayıt zaten varsa, hata döndür
        //                    return BadRequest(new { isFinished = false, message = " " });
        //                }
        //            }
        //            else
        //            {
        //                // Eğer kayıt yoksa, yeni kayıt ekle
        //                _context.TopluSorguKayit.Add(new TopluSorguKayit
        //                {
        //                    Tarih = today,
        //                    SorgulananOemNo = item.SorgulananOemNo,
        //                    SirketAd = item.SirketAd,
        //                    Stok = item.Stok,
        //                    OemNo = item.OemNo,
        //                    StokNo = item.StokNo,
        //                    UrunAd = item.UrunAd,
        //                    Marka = item.Marka,
        //                    kdvLi = item.kdvLi,
        //                    kdvSiz = item.kdvSiz,
        //                    ListeFiyati = item.ListeFiyati
        //                });

        //                // Yeni kaydı ekle
        //                _context.SaveChanges();
        //            }
        //        }

        //        return Ok(new { isFinished = true, message = " " });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { isFinished = false, message = "Bir hata oluştu", error = ex.Message });
        //    }
        //}

        #endregion




        //[HttpPost("SaveDataList")]
        //public async Task<IActionResult> SaveDataList([FromBody] List<TopluSorguKayit> dataList)
        //{
        //    try
        //    {
        //        if (dataList == null || !dataList.Any())
        //            return BadRequest("Gönderilen veri listesi boş");

        //        using (var context = new DataContext())
        //        {
        //            context.TopluSorguKayit.AddRange(dataList);
        //            await context.SaveChangesAsync();
        //        }

        //        return Ok(new { isFinished = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { isFinished = false, message = ex.Message });
        //    }
        //}


        [HttpPost("SaveDataList")]
        public async Task<IActionResult> SaveDataList([FromBody] List<TopluSorguKayit> data)
        {
            if (data == null || !data.Any())
            {
                return BadRequest(new { isFinished = false, message = "Veri eksik veya geçersiz." });
            }

            // Gelen listeyi DataTable'e dönüştür
            var dataTable = ConvertToDataTable(data);

            try
            {
                using (var _context = new DataContext())
                {
                    var connection = _context.Database.GetDbConnection(); // DataContext üzerinden bağlantıyı al
                    await connection.OpenAsync();

                    using (SqlCommand command = connection.CreateCommand() as SqlCommand)
                    {
                        command.CommandText = "SaveDataList";
                        command.CommandType = CommandType.StoredProcedure;

                        // TVP'yi parametre olarak ekle
                        var parameter = command.Parameters.AddWithValue("@DataList", dataTable);
                        parameter.SqlDbType = SqlDbType.Structured;
                        parameter.TypeName = "dbo.TopluSorguKayitType";  

                        // Prosedürü çalıştır
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok(new { isFinished = true, message = "Kayıt işlemi başarılı." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isFinished = false, message = "Bir hata oluştu.", error = ex.Message });
            }
        }


        private DataTable ConvertToDataTable(List<TopluSorguKayit> data)
        {
            DataTable table = new DataTable();

            // Sütunlar
            table.Columns.Add("SorgulananOemNo", typeof(string));
            table.Columns.Add("Tarih", typeof(DateTime));
            table.Columns.Add("SirketAd", typeof(string));
            table.Columns.Add("Stok", typeof(string));
            table.Columns.Add("OemNo", typeof(string));
            table.Columns.Add("StokNo", typeof(string));
            table.Columns.Add("UrunAd", typeof(string));
            table.Columns.Add("Marka", typeof(string));
            table.Columns.Add("Kdvli", typeof(string));
            table.Columns.Add("Kdvsiz", typeof(string));
            table.Columns.Add("ListeFiyati", typeof(string));
            // table.Columns.Add("Kampanya", typeof(string));

            // Satırları ekle
            foreach (var item in data)
            {
                // Eğer tüm sütunlar aynı anda null ise satır ekleme
                if (string.IsNullOrEmpty(item.Marka) &&
                    string.IsNullOrEmpty(item.kdvLi) &&
                    string.IsNullOrEmpty(item.kdvSiz) &&
                    string.IsNullOrEmpty(item.StokNo) &&
                    string.IsNullOrEmpty(item.UrunAd) &&
                    string.IsNullOrEmpty(item.Stok) &&
                    string.IsNullOrEmpty(item.ListeFiyati))
                {
                    continue; // Satır ekleme, sonraki öğeye geç
                }

                // Tarih güncelle
                item.Tarih = DateTime.Today;

                // Satırı tabloya ekle
                table.Rows.Add(
                    item.SorgulananOemNo,
                    item.Tarih,
                    item.SirketAd,
                    item.Stok,
                    item.OemNo,
                    item.StokNo,
                    item.UrunAd,
                    item.Marka,
                    item.kdvLi,
                    item.kdvSiz,
                    item.ListeFiyati
                // item.Kampanya
                );
            }

            return table;
        }





        #region test
        // SocketScreen
        [HttpGet("sendscreen")]
        [AllowAnonymous]
        public IActionResult SendScreen([FromQuery] RequestInput key)
        {
            //new SocketScreen().SendRequest("text");
            return Ok(true);
        }
        [HttpGet("checkscreen")]
        [AllowAnonymous]
        public IActionResult CheckScreen([FromQuery] RequestInput key)
        {
            //var res = new SocketScreen().SendRequestOffer("image");

            return Ok(new { Success = true, Data = 1 });
        }
        #endregion
    }
}
