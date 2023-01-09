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

        // GET: Personal/FormReport
        public ActionResult Index()
        {
            {
                var current_year = DateTime.Now.Year;
                var check = db.ReportYears.Where(y => y.year == current_year);
                return View(check);
            }
        }
        public ActionResult FormReport()
        {
            return View();
        }
    }
}