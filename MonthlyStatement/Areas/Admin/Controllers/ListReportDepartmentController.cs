using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Quản trị viên,Ban phòng khoa,Thư ký")]

    public class ListReportDepartmentController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();

        // GET: Admin/ListReportDepartment
        public ActionResult Index()
        {
            return View(db.DepartmentReports.ToList());
        }
        public ActionResult Detail(int id)
        {
            var per = db.DepartmentReports.Find(id);
            var form = db.FormDepartmentReports.FirstOrDefault(f => f.report_period_id == per.report_period_id);
            ViewBag.accID = per.account_id;
            return View(form);
        }
    }
}