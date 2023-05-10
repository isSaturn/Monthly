using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Quản trị viên")]

    public class YearReportController : Controller
    {

        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Admin/YearReport
        public ActionResult Index()
        {
            var current_year = DateTime.Now.Year;
            int check = db.ReportYears.Where(y => y.year == current_year).Count();

            if (check < 1)
            {
                //cho phep thêm mới năm báo cáo
                Session["ReportYear-Check"] = true;
            }

            else
                //khong cho phep them moi
                Session["ReportYear-Check"] = false;
            return View(db.ReportYears.OrderByDescending(y => y.year).ToList());
        }
        [HttpPost]
        public JsonResult Create()
        {
            try
            {
                var current_year = DateTime.Now.Year;
                int check = db.ReportYears.Where(y => y.year == current_year).Count();
                if (check < 1)
                {

                    var report_year = new ReportYear();
                    report_year.year = current_year;
                    db.ReportYears.Add(report_year);
                    db.SaveChanges();


                    for (int i = 1; i <= 12; i++)
                    {
                        var report_period = new ReportPeriod();
                        report_period.report_year_id = report_year.report_year_id;
                        report_period.start_date = Convert.ToDateTime(current_year + "-" + i.ToString("D2") + "-01");
                        report_period.deadline_date = Convert.ToDateTime(current_year + "-" + i.ToString("D2") + "-22");

                        report_period.end_date =
                            Convert.ToDateTime(current_year + "-" + i.ToString("D2") + "-" +
                            DateTime.DaysInMonth(current_year, i).ToString("D2"));
                        report_period.report_period_name = "Kỳ báo cáo tháng " + i + "-" + current_year;
                        db.ReportPeriods.Add(report_period);

                        db.SaveChanges();

                        FormDepartmentReport formDepartmentReport = new FormDepartmentReport();
                        formDepartmentReport.report_period_id = report_period.report_period_id;
                        db.FormDepartmentReports.Add(formDepartmentReport);

                        FormPersonalReport formPersonalReport = new FormPersonalReport();
                        formPersonalReport.report_period_id = report_period.report_period_id;
                        db.FormPersonalReports.Add(formPersonalReport);

                        FormStaffReport formStaffReport = new FormStaffReport();
                        formStaffReport.report_period_id = report_period.report_period_id;
                        db.FormStaffReports.Add(formStaffReport);

                        db.SaveChanges();

                    }
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Năm báo cáo đã tồn tại!", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json("Lỗi: " + e.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}