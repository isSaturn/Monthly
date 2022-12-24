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
            return View(db.ReportYears.ToList());
        }
    }
}