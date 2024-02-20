
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

        public List<ExcelOEM> Aktar(string path)
        {
            ISheet sheet = null;
            FileStream excelFile = null;

            try
            {
                excelFile = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch (IOException ioe)
            {

            }
            if (path.Contains(".xlsx"))
            {
                XSSFWorkbook wb1 = new XSSFWorkbook(excelFile);
                string namesheet = wb1.NumberOfSheets > 0 ? wb1.GetSheetName(0) : "Sayfa1";
                sheet = wb1.GetSheet(namesheet);
            }
            else
            {
                HSSFWorkbook wb = new HSSFWorkbook(excelFile);
                string namesheet = wb.NumberOfSheets > 0 ? wb.GetSheetName(0) : "Sayfa1";
                sheet = wb.GetSheet(namesheet);
            }
            List<ExcelOEM> list = new List<ExcelOEM>();
            ExcelOEM loginitem;


            for (int indx = 1; indx <= sheet.LastRowNum; indx++)
            {
                loginitem = new ExcelOEM();
                IRow row = sheet.GetRow(indx);
                List<ICell> cels = row.Cells;


                foreach (ICell cell in cels)
                {
                    if (cell.ColumnIndex == 0)
                    {
                        if (string.IsNullOrEmpty(cell.ToString()))
                        {
                            break;
                        }

                        loginitem.OemKodu = cell.ToString();
                    }



                    //örnek ekleme kodu
                    // pol.GenelBilgiler.PoliceNumarasi = cell.ToString();
                }
                if (!loginitem.OemKodu.IsNullOrEmpty())
                {
                    list.Add(loginitem);
                }
                    
            }

            return list;

        }
    }
}

