using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Quản trị viên")]

    public class NotificationController : Controller
    {
        // GET: Admin/Notification
        public ActionResult Index()
        {
            return View();
        }
    }
}