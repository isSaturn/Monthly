using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    public class YearReportController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Admin/YearReport
        public ActionResult Index()
        {
            var current_year = DateTime.Now.Year;
            var check = db.ReportYears.FirstOrDefault(y => y.year == current_year);
            if (check == null)
                //cho phep thêm mới năm báo cáo
                Session["ReportYear-Check"] = true;
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
                var check = db.ReportYears.FirstOrDefault(y => y.year == current_year);
                if (check == null)
                {

                    var report_year = new ReportYear();
                    report_year.year = current_year;
                    db.ReportYears.Add(report_year);
                    db.SaveChanges();


                    for (int i = 1; i <= 12; i++)
                    {
                        var report_period = new ReportPeriod();
                        report_period.report_year_id = report_year.report_year_id;
                        report_period.start_date = Convert.ToDateTime(current_year + "-" + i.ToString("D2")+ "-01");
                        report_period.deadline_date = Convert.ToDateTime(current_year + "-" + i.ToString("D2") + "-22");

                        report_period.end_date =
                            Convert.ToDateTime(current_year + "-" + i.ToString("D2") + "-" +
                            DateTime.DaysInMonth(current_year, i).ToString("D2"));
                        report_period.report_period_name = "Kỳ báo cáo tháng " + i + "-" + current_year;
                        if (DateTime.Now.Month != i)
                        {
                            report_period.status = "Không được báo cáo";
                        }
                        else
                        {
                            if (DateTime.Now.Day > 22)
                            {
                                report_period.status = "Báo cáo trễ";
                            }
                            else
                            {
                                report_period.status = "Đang được báo cáo";
                            }
                        }
                        db.ReportPeriods.Add(report_period);

                        db.SaveChanges();

                        FormDepartmentReport formDepartmentReport = new FormDepartmentReport();
                        formDepartmentReport.report_period_id = report_period.report_period_id;
                        db.FormDepartmentReports.Add(formDepartmentReport);

                        FormPersonalReport formPersonalReport = new FormPersonalReport();
                        formPersonalReport.report_period_id = report_period.report_period_id;
                        db.FormPersonalReports.Add(formPersonalReport);

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