using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Personal.Controllers
{
    public class ReportController : Controller
    {
        // GET: Personal/Report
        public ActionResult Personal()
        {
            return View();
        }
        public ActionResult PersonalDetail()
        {
            return View();
        }
    }
}