using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Personal.Controllers
{
    public class StaticController : Controller
    {

        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Personal/Static
        public ActionResult Index()
        {
            var report_period = db.ReportPeriods.Where(r => r.ReportYear.year == DateTime.Now.Year).ToList();

            return View(report_period);
        }
    }
}