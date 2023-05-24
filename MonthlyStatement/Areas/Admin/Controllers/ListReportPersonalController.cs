using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Quản trị viên,Ban phòng khoa,Bộ môn,Thư ký")]

    public class ListReportPersonalController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();

        // GET: Admin/ListReportPersonal
        public ActionResult Index()
        {
            return View(db.PersonalReports.ToList());
        }
        public ActionResult Detail(int id)
        {
            var per = db.PersonalReports.Find(id);
            var form = db.FormPersonalReports.FirstOrDefault(f => f.report_period_id == per.report_period_id);
            ViewBag.accID = per.account_id;
            return View(form);
        }
    }
}