using DocumentFormat.OpenXml.Drawing.Diagrams;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OtoRobotWeb2.Controllers;
using OtoRobotWeb2.Models;
using OtoRobotWeb2.Models.Database;
using OtoRobotWeb2.Models.Response;
using System.Linq;

namespace OtoRobotWeb2.Helpers
{
    public class Tools
    {
        public static List<TVMDetay> TVMDetays = new List<TVMDetay>();
        public static void TvmDoldur()
        {
            DataContext dataContext = new DataContext();
            TVMDetays = dataContext.TVMDetay.ToList();

        }
        public static byte[] ExporttoExcel<T>(List<T> table, string filename, DateTime baslangicTarih, DateTime bitisTarih)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage pack = new ExcelPackage();
            int baslangicSatiri = 3;
            ExcelWorksheet ws = pack.Workbook.Worksheets.Add(filename);
            ws.Cells["A" + baslangicSatiri].LoadFromCollection(table, true, TableStyles.Light1);

            ws.Cells["A1:C1"].Merge = true;
            ws.Cells["A1"].Value = "OtoRobot Log Sonuçları";
            ws.Cells["A1"].Style.Font.Size = 15;
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A2:C2"].Merge = true;
            ws.Cells["A2"].Value = baslangicTarih.ToShortDateString() + "-" + bitisTarih.ToShortDateString() + " Aralığında";
            ws.Cells["A2"].Style.Font.Bold = true;

            //ws.Cells["A" + baslangicSatiri].Value = "Firma";
            ws.Cells["B" + baslangicSatiri].Value = "Sorgu Sayısı";

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            return pack.GetAsByteArray();
        }
        public static byte[] ExporttoExcelOEM<T>(List<T> table, string filename )
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage pack = new ExcelPackage();
            int baslangicSatiri = 3;
            string Tarih = DateTime.Now.ToShortDateString();
            ExcelWorksheet ws = pack.Workbook.Worksheets.Add("Stokda Bulunanlar");
            ws.Cells["A" + baslangicSatiri].LoadFromCollection(table, true, TableStyles.Light1);

            ws.Cells["A1:F1"].Merge = true;

            ws.Cells["A1"].Value = "Otorobot OEM Toplu Sorgu Stokda Bulunan Kayıtlar Tablosu  - "+Tarih;
            ws.Cells["A1"].Style.Font.Size = 15;
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A2:C2"].Merge = true;
            ws.Cells["A2"].Style.Font.Bold = true; 


            //ws.Cells["G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            ws.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            ws.Cells["G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Cells["H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Cells["I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Başlık ekleme (para birimi sütunu için)
            int sonSutunIndex = ws.Dimension.End.Column;
           
            ws.Cells[baslangicSatiri - 1, sonSutunIndex + 1].Value = "Para Birimi";

            // Başlıkların biçimlendirilmesi (isteğe bağlı)
            ws.Cells[baslangicSatiri - 1, sonSutunIndex, baslangicSatiri - 1, sonSutunIndex + 1].Style.Font.Bold = true;
            ws.Cells[baslangicSatiri - 1, sonSutunIndex, baslangicSatiri - 1, sonSutunIndex + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Verileri işlemeye başla
            for (int row = baslangicSatiri; row <= ws.Dimension.End.Row; row++)
            {
                var cellValue = ws.Cells[row, sonSutunIndex].Text; // Son sütundaki hücre değeri
                if (!string.IsNullOrEmpty(cellValue))
                {
                    string currencySymbol = ""; // Para birimi için geçici değişken
                    int symbolIndex = -1; // İşaretin bulunma indeksi

                    // USD işareti kontrolü
                    if ((symbolIndex = cellValue.IndexOf("$")) >= 0)
                    {
                        currencySymbol = "USD";
                    }
                    if ((symbolIndex = cellValue.IndexOf("₺")) >= 0)
                    {
                        currencySymbol = "TL";
                    }
                    if ((symbolIndex = cellValue.IndexOf("€")) >= 0)
                    {
                        currencySymbol = "EUR";
                    }
                    if ((symbolIndex = cellValue.IndexOf("USD")) >= 0)
                    {
                        currencySymbol = "USD";
                    }
                    // EUR işareti kontrolü
                    else if ((symbolIndex = cellValue.IndexOf("EUR")) >= 0)
                    {
                        currencySymbol = "EUR";
                    }
                    // EU işareti kontrolü
                    else if ((symbolIndex = cellValue.IndexOf("EU")) >= 0)
                    {
                        currencySymbol = "EU";
                    }

                    // Eğer işaret bulunduysa
                    if (symbolIndex >= 0)
                    {
                        // İşaretten önceki kısmı hücrede bırak
                        ws.Cells[row, sonSutunIndex].Value = cellValue.Substring(0, symbolIndex).Trim();

                        // İşareti yeni sütuna yaz
                        ws.Cells[row, sonSutunIndex + 1].Value = currencySymbol;
                    }
                }
            }

            // Sütunları otomatik genişlet
            ws.Cells[ws.Dimension.Address].AutoFitColumns();




            // STOKDA BULUNAMAYANLAR START

            ExcelWorksheet ws2 = pack.Workbook.Worksheets.Add("Stokda Bulunamayanlar");
            List<ResponseItemExcel2> list2 = new List<ResponseItemExcel2>();
            ResponseItemExcel2 responseItemExcel2 = null;
            int i = 0;
           
            foreach (var item in RequestController.yanitVarMi1List)
            {
                responseItemExcel2 = new ResponseItemExcel2();
                responseItemExcel2.SorgulananOem = item.SorgulananOem;
                responseItemExcel2.SirketAd = item.SirketAd;
                responseItemExcel2.Sonuc = "Parça Bulunamadı.Stok yok veya bu OEM numarasına sahip bir ürün yok. ";
                list2.Add(responseItemExcel2);

            }
            foreach (var item in RequestController.yanitVarMi2List)
            {
                responseItemExcel2 = new ResponseItemExcel2();
                responseItemExcel2.SorgulananOem = "";
                responseItemExcel2.SirketAd = item.SirketAd;
                responseItemExcel2.Sonuc = "Oturum açılamadığı için sonuç gelmedi. Kullanıcı adı veya şifreniz hatalı. ";
                list2.Add(responseItemExcel2);

            }

            ws2.Cells["A3"].LoadFromCollection(list2, true, TableStyles.Light1);

            ws2.Cells["A1:F1"].Merge = true;

            ws2.Cells["A1"].Value = "Otorobot OEM Toplu Sorgu Stokda Bulunamayan Kayıtlar Tablosu - " + Tarih;
            ws2.Cells["A1"].Style.Font.Size = 15;
            ws2.Cells["A1"].Style.Font.Bold = true;
            ws2.Cells["A2:C2"].Merge = true;
            ws2.Cells["A2"].Style.Font.Bold = true;


            //ws.Cells["G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            ws2.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws2.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws2.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            ws2.Cells["G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws2.Cells["H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws2.Cells["I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            ws2.Cells[ws2.Dimension.Address].AutoFitColumns();

            // STOKDA BULUNAMAYANLAR END


            return pack.GetAsByteArray();

        }
      
    }
}
