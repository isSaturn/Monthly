using MonthlyStatement.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using System.Windows.Forms;

namespace MonthlyStatement.Areas.Department.Controllers
{
    public class StatisticalController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Department/Statistical
        public ActionResult Index(DateTime? date)
        {

            ViewBag.Date = date == null ? DateTime.Now : date;

            return View(db.Faculties.ToList());
        }

        public void ExportToExcel()
        {
            var faculty = db.Faculties.ToList();
            

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Thống kê");

            ws.Name = "Thống kê báo cáo";
            ws.Cells.Style.Font.Size = 11;
            ws.Cells.Style.Font.Name = "Calibri";

            string[] arrColumnHeader = {"ID","Tên khoa" };

            var countColHeader = arrColumnHeader.Count();

            ws.Cells[1, 1].Value = "Thống kê báo cáo";
            ws.Cells[1, 1, 1, countColHeader].Merge = true;
            ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
            ws.Cells[1, 1, 1, countColHeader].Style.Font.Size = 20;
            ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells["A2"].Value = "Thời gian";
            ws.Cells["B2"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);

            ws.Cells["A3"].Value = "ID";
            ws.Cells["B3"].Value = "Tên Khoa";
           

            int rowStart = 7;
            foreach (var item in faculty)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.faculty_id;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.faculty_name;
                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=Export.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }


        

    }
}