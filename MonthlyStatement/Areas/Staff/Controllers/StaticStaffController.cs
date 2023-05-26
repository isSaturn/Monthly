using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Staff.Controllers
{
    [Authorize(Roles = "Nhân viên")]
    public class StaticStaffController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();
        // GET: Staff/StaticStaff
        public ActionResult Index()
        {
            var report_period = db.ReportPeriods.Where(r => r.ReportYear.year == DateTime.Now.Year).ToList();

            return View(report_period);
        }
    }
}