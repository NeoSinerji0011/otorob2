
using System.Collections.Generic;
using System;
using System.IO;
using OtoRobotWeb2.Models.Database;
using System.Security.Claims;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OtoRobotWeb2.Models.Response;
using Microsoft.IdentityModel.Tokens;

namespace OtoRobotWeb2.Helpers
{
    public class ExcelReader
    {

        public List<ExcelOEM> Aktar(Stream fileStream)
        {
            ISheet sheet = null;
            List<ExcelOEM> list = new List<ExcelOEM>();

            try
            {
                using (var workbook = new XSSFWorkbook(fileStream))
                {
                    string sheetName = workbook.NumberOfSheets > 0 ? workbook.GetSheetName(0) : "Sayfa1";
                    sheet = workbook.GetSheet(sheetName);

                    for (int indx = 1; indx <= sheet.LastRowNum; indx++)
                    {
                        var row = sheet.GetRow(indx);
                        if (row == null) continue;

                        var loginItem = new ExcelOEM { OemKodu = row.GetCell(0)?.ToString() ?? "" };

                        if (!string.IsNullOrEmpty(loginItem.OemKodu))
                        {
                            list.Add(loginItem);
                        }
                    }
                }
            }
            catch (IOException ioe)
            {
                Console.WriteLine("Excel dosyası okunurken hata oluştu: " + ioe.Message);
            }

            return list;
        }



        //public List<ExcelOEM> Aktar(string path)
        //{
        //    if (!System.IO.File.Exists(path))
        //    {
        //        Console.WriteLine("Dosya bulunamadı: " + path);
        //        return new List<ExcelOEM>();
        //    }

        //    ISheet sheet = null;

        //    try
        //    {
        //        // 📌 Dosyayı RAM'e alıp okuma (FileStream yerine ReadAllBytes kullanıyoruz)
        //        byte[] fileBytes = System.IO.File.ReadAllBytes(path);
        //        using (MemoryStream stream = new MemoryStream(fileBytes))
        //        {
        //            if (path.EndsWith(".xlsx"))
        //            {
        //                using (XSSFWorkbook wb1 = new XSSFWorkbook(stream))
        //                {
        //                    sheet = wb1.GetSheetAt(0);
        //                }
        //            }
        //            else
        //            {
        //                using (HSSFWorkbook wb = new HSSFWorkbook(stream))
        //                {
        //                    sheet = wb.GetSheetAt(0);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Dosya okuma hatası: " + ex.Message);
        //        return new List<ExcelOEM>();
        //    }

        //    if (sheet == null)
        //    {
        //        Console.WriteLine("Excel dosyası boş veya okunamadı.");
        //        return new List<ExcelOEM>();
        //    }

        //    List<ExcelOEM> list = new List<ExcelOEM>();

        //    for (int indx = 1; indx <= sheet.LastRowNum; indx++)
        //    {
        //        IRow row = sheet.GetRow(indx);
        //        if (row == null || row.Cells.Count == 0) continue;

        //        ExcelOEM loginitem = new ExcelOEM();

        //        foreach (ICell cell in row.Cells)
        //        {
        //            if (cell.ColumnIndex == 0)
        //            {
        //                string cellValue = cell.ToString().Trim();
        //                if (string.IsNullOrEmpty(cellValue)) break;

        //                loginitem.OemKodu = cellValue;
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(loginitem.OemKodu))
        //        {
        //            list.Add(loginitem);
        //        }
        //    }

        //    return list;
        //}

    }
}

