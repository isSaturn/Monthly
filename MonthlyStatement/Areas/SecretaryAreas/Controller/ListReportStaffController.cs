using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.SecretaryAreas.Controllers
{
    [Authorize(Roles = "Thư ký")]

    public class ListReportStaffController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();

        // GET: ListReportPersonal
        public ActionResult Index()
        {
            return View(db.StaffReports.ToList());
        }
        public ActionResult Detail(int id)
        {
            var per = db.StaffReports.Find(id);
            var form = db.FormStaffReports.FirstOrDefault(f => f.report_period_id == per.report_period_id);
            ViewBag.accID = per.account_id;
            return View(form);
        }
    }
}