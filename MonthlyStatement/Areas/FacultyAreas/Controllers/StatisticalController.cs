using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.FacultyAreas.Controllers
{
    public class StatisticalController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: FacultyAreas/Statistical
        public ActionResult Index(DateTime? date)
        {

            ViewBag.Date = date == null ? DateTime.Now : date;

            return View(db.Faculties.ToList());
        }
        
    }
}