  

 
using System;
using System.Collections.Generic; 
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace  OtoRobotWeb2.Helpers
{
    public class HelperTools
    {
        static string rootPath = Directory.GetCurrentDirectory();
            public static DateTime getTRDateTime()
        {
            var info = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);

            return localTime.DateTime;
        }
        public static string paraMetinDuzenle(string val)
        {
            string bolum = "1", result = "";
             
            val = val.Replace(".", ",");
            var tamkismi = val.Substring(0, val.LastIndexOf(',') > 0 ? val.LastIndexOf(',') : val.Length).Replace(",", "");
            var ondalikkismi = val.Substring(val.LastIndexOf(',') > 0 ? val.LastIndexOf(',') + 1 : val.Length);
            ondalikkismi = ondalikkismi.Length < 1 ? "0" : ondalikkismi;
            for (int i = 0; i < ondalikkismi.Length; i++)
                bolum += "0";
            decimal tempondalik = decimal.Parse(ondalikkismi) / (decimal.Parse(bolum));
            decimal temptam = decimal.Parse(tamkismi);
            decimal toplam = temptam + tempondalik;
            result = toplam.ToString();
            return result;
        }

    }
}