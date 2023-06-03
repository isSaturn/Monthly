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
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            var check_Faculty = db.Profiles.FirstOrDefault(x => x.account_id == accID);
            Session["faculty"] = check_Faculty.faculty_id;
            var data = db.Faculties.Where(y => y.faculty_id == check_Faculty.faculty_id).ToList();
            return View(data);
        }

        public void ExportToExcelDepartment(DateTime date)
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            var check_Faculty = db.Profiles.FirstOrDefault(x => x.account_id == accID);
            Session["faculty"] = check_Faculty.faculty_id;
            var data = db.Faculties.Where(y => y.faculty_id == check_Faculty.faculty_id).ToList();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Thống kê");



            ws.Name = "Thống kê báo cáo bộ môn";
            ws.Cells.Style.Font.Size = 11;
            ws.Cells.Style.Font.Name = "Calibri";

            string[] arrColumnHeader = { "STT", "Tên bộ môn", "Nhân viên", "Giảng viên", "Đã báo cáo", "Chưa báo cáo", "Báo cáo trễ" };

            var countColHeader = arrColumnHeader.Count();

            ws.Cells[1, 1].Value = "Thống kê báo cáo" + " " + "tháng" + " " + string.Format("{0:MM/yyyy}", date);
            ws.Cells[1, 1, 1, countColHeader].Merge = true;
            ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
            ws.Cells[1, 1, 1, countColHeader].Style.Font.Size = 20;
            ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells["A3"].Value = "STT";
            ws.Cells["B3"].Value = "Tên";
            ws.Cells["C3"].Value = "Nhân viên";
            ws.Cells["D3"].Value = "Giảng viên";
            ws.Cells["E3"].Value = "Đã báo cáo";
            ws.Cells["F3"].Value = "Chưa báo cáo";
            ws.Cells["G3"].Value = "Báo cáo trễ";

            int i = 1;
            int rowStart = 4;
            foreach (var item in data)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = i++;
                ws.Cells[string.Format("B{0}", rowStart)].Value = "Bộ môn khoa " + item.faculty_name;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.Profiles.Count(x => x.AspNetUser?.AspNetRoles?.FirstOrDefault().Name == "Nhân viên");
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.Profiles.Count(x => x.AspNetUser?.AspNetRoles?.FirstOrDefault().Name == "Giảng viên");

                ws.Cells[string.Format("E{0}", rowStart)].Value = item.Profiles.Count(x => x.AspNetUser?.PersonalReports?.Count(y => y.status == "Đã báo cáo" && y.ReportPeriod.start_date <= date && y.ReportPeriod.end_date > date) > 0)
                                                      + item.Profiles.Count(x => x.AspNetUser?.StaffReports?.Count(y => y.status == "Đã báo cáo" && y.ReportPeriod.start_date <= date && y.ReportPeriod.end_date > date) > 0);
                ws.Cells[string.Format("F{0}", rowStart)].Value = item.Profiles.Count(x => x.AspNetUser?.PersonalReports == null);
                ws.Cells[string.Format("G{0}", rowStart)].Value = item.Profiles.Count(x => x.AspNetUser?.PersonalReports?.Count(y => y.status == "Trễ báo cáo" && y.ReportPeriod.start_date <= date && y.ReportPeriod.end_date > date) > 0)
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