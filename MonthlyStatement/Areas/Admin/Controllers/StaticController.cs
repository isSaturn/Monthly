using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    public class StaticController : Controller
    {
        // GET: Admin/Static
        public ActionResult Index()
        {
            return View();
        }
    }
}