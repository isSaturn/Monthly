using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    public class StaticController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Admin/Static
        public ActionResult Index()
        {
            return View(db.Faculties.ToList());
        }
    }
}