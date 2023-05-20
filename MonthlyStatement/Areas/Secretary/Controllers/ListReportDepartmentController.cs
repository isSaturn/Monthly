using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Secretary.Controllers
{
    [Authorize(Roles = "Thư ký")]

    public class ListReportDepartmentController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();

        // GET:Secretaryy/ListReportDepartment
        public ActionResult Index()
        {
            return View(db.DepartmentReports.ToList());
        }
        public ActionResult Detail(int id)
        {
            var form = db.DepartmentReports.Find(id);
            return View(form);
        }
    }
}