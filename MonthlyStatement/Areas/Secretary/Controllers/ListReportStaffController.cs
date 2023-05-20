using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Secretary.Controllers
{
    [Authorize(Roles = "Thư ký")]

    public class ListReportStaffController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();

        // GET: Secretaryy/ListReportPersonal
        public ActionResult Index()
        {
            return View(db.StaffReports.ToList());
        }
        public ActionResult Detail(int id)
        {
            var form = db.StaffReports.Find(id);
            return View(form);
        }
    }
}