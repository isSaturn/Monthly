using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    public class PersonalReportFormController : Controller
    {
        // GET: Admin/PersonalReportForm
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Formreport()
        {
            return View();
        }
    }
}