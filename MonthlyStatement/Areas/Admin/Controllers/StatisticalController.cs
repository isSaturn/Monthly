using MonthlyStatement.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    public class StatisticalController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Admin/Statistical
        public ActionResult Index(DateTime? date)
        {
            ViewBag.Date = date == null ? DateTime.Now : date;
            return View(db.Faculties.ToList());
        }

        public ActionResult Index_Statistical(DateTime? date, int id)
        {

            ViewBag.Date = date == null ? DateTime.Now : date;
            var data = db.Faculties.Where(y => y.faculty_id == id).ToList();
            return View(data);
        }

        public void ExportToExcelFaculty(DateTime date)
        {
            var faculty = db.Faculties.ToList();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Thống kê");


            ws.Name = "Thống kê báo cáo";
            ws.Cells.Style.Font.Size = 11;
            ws.Cells.Style.Font.Name = "Calibri";

            string[] arrColumnHeader = { "ID", "Tên khoa", "Nhân viên", "Giảng viên", "Bộ môn", "Đã báo cáo", "Chưa báo cáo", "Báo cáo trễ" };

            var countColHeader = arrColumnHeader.Count();

            ws.Cells[1, 1].Value = "Thống kê báo cáo" +" "+ "tháng" +" "+ string.Format("{0:MM/yyyy}", date);
            ws.Cells[1, 1, 1, countColHeader].Merge = true;
            ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
            ws.Cells[1, 1, 1, countColHeader].Style.Font.Size = 20;
            ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells["A3"].Value = "STT";
            ws.Cells["B3"].Value = "Tên Khoa";
            ws.Cells["C3"].Value = "Nhân viên";
            ws.Cells["D3"].Value = "Giảng viên";
            ws.Cells["E3"].Value = "Bộ môn";
            ws.Cells["F3"].Value = "Đã báo cáo";
            ws.Cells["G3"].Value = "Chưa báo cáo";
            ws.Cells["H3"].Value = "Báo cáo trễ";

            int rowStart = 4;
            foreach (var item in faculty)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.faculty_id;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.faculty_name;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.Profiles.Count(x => x.AspNetUser?.AspNetRoles?.FirstOrDefault().Name == "Nhân viên");
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.Profiles.Count(x => x.AspNetUser?.AspNetRoles?.FirstOrDefault().Name == "Giảng viên");
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.Profiles.Count(x => x.AspNetUser?.AspNetRoles?.FirstOrDefault().Name == "Bộ môn");

                ws.Cells[string.Format("F{0}", rowStart)].Value = item.Profiles.Count(x => x.AspNetUser?.PersonalReports?.Count(y => y.status == "Đã báo cáo" && y.ReportPeriod.start_date <= date && y.ReportPeriod.end_date > date) > 0)
                                                      + item.Profiles.Count(x => x.AspNetUser?.DepartmentReports?.Count(y => y.status == "Đã báo cáo" && y.ReportPeriod.start_date <= date && y.ReportPeriod.end_date > date) > 0)
                                                      + item.Profiles.Count(x => x.AspNetUser?.StaffReports?.Count(y => y.status == "Đã báo cáo" && y.ReportPeriod.start_date <= date && y.ReportPeriod.end_date > date) > 0);
                ws.Cells[string.Format("G{0}", rowStart)].Value = item.Profiles.Count(x => x.AspNetUser?.PersonalReports == null);
                ws.Cells[string.Format("H{0}", rowStart)].Value = item.Profiles.Count(x => x.AspNetUser?.PersonalReports?.Count(y => y.status == "Trễ báo cáo" && y.ReportPeriod.start_date <= date && y.ReportPeriod.end_date > date) > 0)
                                                      + item.Profiles.Count(x => x.AspNetUser?.DepartmentReports?.Count(y => y.status == "Trễ báo cáo" && y.ReportPeriod.start_date <= date && y.ReportPeriod.end_date > date) > 0)
                                                      + item.Profiles.Count(x => x.AspNetUser?.StaffReports?.Count(y => y.status == "Trễ báo cáo" && y.ReportPeriod.start_date <= date && y.ReportPeriod.end_date > date) > 0);
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