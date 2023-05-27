using MonthlyStatement.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Personal.Controllers
{
    [Authorize(Roles = "Giảng viên")]

    public class StaticController : Controller
    {

        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Personal/Static
        public ActionResult Index()
        {
            var report_period = db.ReportPeriods.Where(r => r.ReportYear.year == DateTime.Now.Year).ToList();
            return View(report_period);
        }

        public void ExportToExcelPersonal()
        {
            string emails = User.Identity.Name;
            var report_period = db.ReportPeriods.Where(r => r.ReportYear.year == DateTime.Now.Year).ToList();
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Thống kê");


            ws.Name = "Thống kê báo cáo giảng viên";
            ws.Cells.Style.Font.Size = 11;
            ws.Cells.Style.Font.Name = "Calibri";

            string[] arrColumnHeader = { "", "Hoàn thành", "Chưa hoàn thành"};

            var countColHeader = arrColumnHeader.Count();

            ws.Cells[1, 1].Value = "Thống kê báo cáo" + " " + "năm" + " " + DateTime.Now.Year;
            ws.Cells[1, 1, 1, countColHeader].Merge = true;
            ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
            ws.Cells[1, 1, 1, countColHeader].Style.Font.Size = 14;
            ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[2, 1].Value = "Tài khoản: " + emails;
            ws.Cells[2, 1, 2, countColHeader].Merge = true;
            ws.Cells[2, 1, 2, countColHeader].Style.Font.Italic = true;
            ws.Cells[3, 1].Value = "Vai trò: Giảng viên";
            ws.Cells[3, 1, 3, countColHeader].Merge = true;
            ws.Cells[3, 1, 3, countColHeader].Style.Font.Italic = true;

            ws.Cells["A4"].Value = "";
            ws.Cells["B4"].Value = "Hoàn thành";
            ws.Cells["C4"].Value = "Chưa hoàn thành";

            int rowStart = 5;
            foreach (var item in report_period)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.report_period_name;
                if(item.PersonalReports.Any(p => p.AspNetUser.Email == emails))
                {
                    ws.Cells[string.Format("B{0}", rowStart)].Value = "✔";
                    ws.Cells[string.Format("B{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                else 
                {
                    ws.Cells[string.Format("C{0}", rowStart)].Value = "✘";
                    ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
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