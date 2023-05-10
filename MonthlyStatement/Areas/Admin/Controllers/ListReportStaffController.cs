using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Quản trị viên")]

    public class ListReportStaffController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();

        // GET: Admin/ListReportPersonal
        public ActionResult Index()
        {
            return View(db.PersonalReports.ToList());
        }
        public ActionResult Detail(int id)
        {
            var form = db.PersonalReports.Find(id);
            return View(form);
        }
    }
}