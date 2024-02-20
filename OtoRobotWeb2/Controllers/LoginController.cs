using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using OtoRobotWeb2.Helpers;
using OtoRobotWeb2.Models.Database;
using OtoRobotWeb2.Models.Inputs;
using OtoRobotWeb2.Models.Login;
using OtoRobotWeb2.Models.MongoDb;
using OtoRobotWeb2.Models.Response;
using OtoRobotWeb2.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace OtoRobotWeb2.Controllers
{
    public class LoginController : Controller
    {
        private OfferService _offerService;

        public LoginController(OfferService offerService)
        {
            _offerService = offerService;
        }
        public IActionResult Index()
        {

            return View();
        }


        [AllowAnonymous]
        public async Task<IActionResult> CheckLogin([FromQuery] LoginPage key)
        {
            var temp = _offerService.Login(new Models.Database.Users { Email = key.username, Password = key.password });
            bool alreadysession = false, login = false;
            if (temp != null)
            {
                login = true;
                if (!string.IsNullOrEmpty(temp.SessionToken))
                {
                    alreadysession = true;
                }
                Login(temp);
                //var token = _offerService.SetSession(temp.KullaniciKodu);
                //var firmaDetay = _offerService.GetFirma(temp.TVMKodu);
                //var claims = new List<Claim>
                //{
                //    new Claim(ClaimTypes.Name,temp.Adi+" "+temp.Soyadi),
                //    new Claim(ClaimTypes.NameIdentifier,temp.Email),
                //    new Claim(ClaimTypes.Sid,temp.TVMKodu.ToString()),
                //    new Claim(ClaimTypes.PrimarySid,temp.KullaniciKodu.ToString()),
                //};

                ////Sorgulama turu 0 ise Tedarikçi seçimi olmayacak, 1 ise Tedarikçi seçile bilir
                //string sorgulamaturu = "0";
                //if (firmaDetay.SorgulamaTuru.HasValue)
                //{
                //    sorgulamaturu = firmaDetay.SorgulamaTuru.Value.ToString();
                //}
                //claims.Add(new Claim("SorgulamaTuru", sorgulamaturu));
                //claims.Add(new Claim("Token", token));

                ////toplusorgu 0 ise Toplu sorgu yapamaz, 1 ise Toplu sorgu yapabilir
                //string toplusorgu = "false";
                //if (firmaDetay.TopluSorguVarmi.HasValue)
                //{
                //    toplusorgu = firmaDetay.TopluSorguVarmi.Value.ToString();
                //}
                ////toplusorgu = "true";
                //claims.Add(new Claim("TopluSorgu", toplusorgu));

                //if (new AuthorizeList().list().Contains(temp.TVMKodu.ToString()))
                //{
                //    claims.Add(new Claim("SuperAdmin", "SuperAdmin"));
                //}
                //if (_offerService.isAdmin(temp.TVMKodu))
                //{
                //    claims.Add(new Claim(ClaimTypes.Role, temp.TVMKodu.ToString()));
                //    claims.Add(new Claim("AdminOnly", "AdminOnly"));

                //}
                //var useridentity = new ClaimsIdentity(claims, "Login");
                //ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                //await HttpContext.SignInAsync(principal);
            }
            if (RequestController.localdsocketprocescalistir)
            {
                alreadysession = false;
            }
            return Ok(new { Login = login, SessionAlready = alreadysession });
        }
        async void Login(TVMKullanicilar temp)
        {
            if (temp != null)
            {
                string token = "";

                var firmaDetay = _offerService.GetFirma(temp.TVMKodu);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,temp.Adi+" "+temp.Soyadi),
                    new Claim(ClaimTypes.NameIdentifier,temp.Email),
                    new Claim(ClaimTypes.Sid,temp.TVMKodu.ToString()),
                    new Claim(ClaimTypes.PrimarySid,temp.KullaniciKodu.ToString()),
                };

                //Sorgulama turu 0 ise Tedarikçi seçimi olmayacak, 1 ise Tedarikçi seçile bilir
                string sorgulamaturu = "0";
                if (firmaDetay.SorgulamaTuru.HasValue)
                {
                    sorgulamaturu = firmaDetay.SorgulamaTuru.Value.ToString();
                }
                claims.Add(new Claim("SorgulamaTuru", sorgulamaturu));

                if (string.IsNullOrEmpty(temp.SessionToken))
                {
                    token = _offerService.SetSession(temp.KullaniciKodu);
                    claims.Add(new Claim("Token", token));
                }

                //toplusorgu 0 ise Toplu sorgu yapamaz, 1 ise Toplu sorgu yapabilir
                string toplusorgu = "false";
                if (firmaDetay.TopluSorguVarmi.HasValue)
                {
                    toplusorgu = firmaDetay.TopluSorguVarmi.Value.ToString();
                }
                //toplusorgu = "true";
                claims.Add(new Claim("TopluSorgu", toplusorgu));

                //Sepet Yetkisi Start 
               
                string sepetYetki = "false";
                if (firmaDetay.SepeteEkleVarmi.HasValue)
                {
                    sepetYetki = firmaDetay.SepeteEkleVarmi.Value.ToString();
                }
                claims.Add(new Claim("sepetYetki", sepetYetki));
                //Sepet Yetkisi End

                if (new AuthorizeList().list().Contains(temp.TVMKodu.ToString()))
                {
                    claims.Add(new Claim("SuperAdmin", "SuperAdmin"));
                }
                if (_offerService.isAdmin(temp.TVMKodu))
                {
                    claims.Add(new Claim(ClaimTypes.Role, temp.TVMKodu.ToString()));
                    claims.Add(new Claim("AdminOnly", "AdminOnly"));

                }
                var useridentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                await HttpContext.SignInAsync(principal);

            }
        }
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var tvmkodu = Convert.ToInt32(identity.FindFirst(ClaimTypes.Sid).Value);
            _offerService.Logout(tvmkodu);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(true);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CheckSession()
        {
            if (RequestController.localdsocketprocescalistir)
                return Ok(false);
            var identity = (ClaimsIdentity)User.Identity;
            var role2 = identity.FindFirst("Token");
            bool islogin = false;
            if (role2 != null)
                islogin = _offerService.CheckSession(role2.Value);
            else
                islogin = true;
            if (islogin)
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(islogin);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CloseAnotherSession(string username)
        {
            bool result = false;
            var res = _offerService.Logout(username);
            if (res != null)
            {
                Login(res);
                result = true;
            }
            return Ok(result);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Error()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var tvmkodu = Convert.ToInt32(identity.FindFirst(ClaimTypes.Sid).Value);
            _offerService.Logout(tvmkodu);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
