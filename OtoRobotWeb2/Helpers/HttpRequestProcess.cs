using Newtonsoft.Json;
using NuGet.Packaging;
using OtoRobotWeb2.Controllers;
using OtoRobotWeb2.Models.Response;
using System.Text;

namespace OtoRobotWeb2.Helpers
{
    public class HttpRequestProcess
    {


        string url_test = "https://localhost:7006/";// localde test ederken conndaki url
        public async Task<SocketResponse> HttpGetCheckLogin(SocketResponse socketResponse, string url)
        {

            using var client = new HttpClient();
            if (RequestController.localdconncalistir)
                url = url_test;
            var tempres = client.GetStringAsync(url + "checklogin?UserId=" + socketResponse.UserId);
            try
            {
                tempres.Wait();
                var content = tempres.Result;
                var res = JsonConvert.DeserializeObject<SocketResponse>(content);
                if (res != null)
                    return res;

            }
            catch (Exception)
            {

            }
            return null;
        }
        public async Task<SocketResponse> HttpGetSendOffer(SocketResponse socketResponse, string url)
        {
            using var client = new HttpClient();
            if (RequestController.localdconncalistir)
                url = url_test;
            //string StringSorgulanacakFirmalar = "", OemNumaralari = "";
            //socketResponse.SorgulanacakFirmalar.ForEach(s =>
            //{
            //    StringSorgulanacakFirmalar += "&SorgulanacakFirmalar=" + s.ToString();
            //});
            //if (socketResponse.OemNumaralari != null)
            //{

            //    socketResponse.OemNumaralari.ForEach(s =>
            //    {
            //        OemNumaralari += "&OemNumaralari=" + s.ToString();
            //    });
            //}
            //var tempres = client.GetStringAsync(url + "sendoffer?UserId=" + socketResponse.UserId + "&SorguId=" + socketResponse.SorguId + "&OemNumarasi=" + socketResponse.OEMNumarasi + "&StokVarmi=" + socketResponse.StokVarmi + "&ProcessType=" + socketResponse.ProcessType + "&KullaniciKodu=" + socketResponse.KullaniciKodu + "&GrupID=" + socketResponse.GrupID + StringSorgulanacakFirmalar + OemNumaralari);
            //var tempres = client.GetStringAsync(url + "sendoffer?UserId=" + socketResponse.UserId + "&SorguId=" + socketResponse.SorguId + "&OemNumarasi=" + socketResponse.OEMNumarasi + "&StokVarmi=" + socketResponse.StokVarmi + "&ProcessType=" + socketResponse.ProcessType + "&KullaniciKodu=" + socketResponse.KullaniciKodu + "&GrupID=" + socketResponse.GrupID + StringSorgulanacakFirmalar + OemNumaralari);
            //var values = new Dictionary<string, string>
            //  {
            //      { "UserId",       socketResponse.UserId.ToString() },
            //      { "SorguId",      socketResponse.SorguId  },
            //      { "OemNumarasi",  socketResponse.OEMNumarasi },
            //      { "StokVarmi",    socketResponse.StokVarmi.ToString() },
            //      { "ProcessType",  socketResponse.ProcessType.ToString() },
            //      { "KullaniciKodu",socketResponse.KullaniciKodu.ToString() },
            //      { "GrupID",       socketResponse.GrupID }
            //  };
            //values.AddRange(OemNumaralari);
            //values.AddRange(StringSorgulanacakFirmalar);
            //var content = new FormUrlEncodedContent(values);


            try
            {
                var postitem = new HttpPostItem();
                postitem.UserId = socketResponse.UserId;
                postitem.SorguId = socketResponse.SorguId;
                postitem.OEMNumarasi = socketResponse.OEMNumarasi;
                postitem.StokVarmi = socketResponse.StokVarmi;
                postitem.ProcessType = socketResponse.ProcessType;
                postitem.KullaniciKodu = socketResponse.KullaniciKodu;
                postitem.GrupID = socketResponse.GrupID;
                postitem.SorgulanacakFirmalar = socketResponse.SorgulanacakFirmalar;
                postitem.OemNumaralari = socketResponse.OemNumaralari;
                var json = JsonConvert.SerializeObject(postitem);

                //string postData = data;
                //byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url + "sendoffer", data);

                string result = response.Content.ReadAsStringAsync().Result;


                //tempres.Wait();
                //var content = tempres.Result;
                //var res = JsonConvert.DeserializeObject<SocketResponse>(content);
                var res = JsonConvert.DeserializeObject<SocketResponse>(result);
                if (res != null)
                    return res;

            }
            catch (Exception)
            {

            }
            return null;
        }
        public async Task<SocketResponse> HttpGetCheckOffer(SocketResponse socketResponse, string url)
        {
            using var client = new HttpClient();
            if (RequestController.localdconncalistir)
                url = url_test;
            var tempres = client.GetStringAsync(url + "checkoffer?UserId=" + socketResponse.UserId + "&SorguId=" + socketResponse.SorguId + "&KullaniciKodu=" + socketResponse.KullaniciKodu + "&GrupID=" + socketResponse.GrupID + "&ProcessType=" + socketResponse.ProcessType);
            try
            {
                tempres.Wait();
                var content = tempres.Result;
                var res = JsonConvert.DeserializeObject<SocketResponse>(content);
                if (res != null)
                    return res;

            }
            catch (Exception)
            {

            }
            return null;
        }
        public async Task<SocketResponse> HttpGetSendOrder(SocketResponse socketResponse, string url)
        {
            using var client = new HttpClient();
            if (RequestController.localdconncalistir)
                url = url_test;
            try
            {
                var postitem = new HttpPostItem();
                postitem.UserId = socketResponse.UserId;
                postitem.SorguId = socketResponse.SorguId;
                postitem.OEMNumarasi = socketResponse.OEMNumarasi;
                postitem.StokVarmi = socketResponse.StokVarmi;
                postitem.ProcessType = socketResponse.ProcessType;
                postitem.KullaniciKodu = socketResponse.KullaniciKodu;
                postitem.GrupID = socketResponse.GrupID;
                postitem.SorgulanacakFirmalar = socketResponse.SorgulanacakFirmalar;
                postitem.OemNumaralari = socketResponse.OemNumaralari;

                var json = JsonConvert.SerializeObject(postitem); 
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url + "sendorder", data);

                string result = response.Content.ReadAsStringAsync().Result;

                var res = JsonConvert.DeserializeObject<SocketResponse>(result);
                if (res != null)
                    return res;

            }
            catch (Exception)
            {

            }
            return null;
        }
    }
    class HttpPostItem
    {
        public int ProcessType { get; set; }
        public int UserId { get; set; }
        public int KullaniciKodu { get; set; }
        public string SorguId { get; set; }
        public string GrupID { get; set; }
        public string SorguTuru { get; set; }
        public string OEMNumarasi { get; set; }
        public bool StokVarmi { get; set; }

        public List<int> SorgulanacakFirmalar { get; set; }
        public List<string> OemNumaralari { get; set; }
    }
}
