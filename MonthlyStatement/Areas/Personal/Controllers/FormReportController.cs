using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Personal.Controllers
{
    [Authorize(Roles = "Nhân viên - Giảng viên")]
    public class FormReportController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Personal/Report
        public ActionResult Index()
        {
            {
                var current_time = DateTime.Now;
                var check = db.ReportPeriods.FirstOrDefault(d => d.start_date <= current_time && d.end_date >= current_time);
                return View(check.FormPersonalReports);
            }
        }
    }
}