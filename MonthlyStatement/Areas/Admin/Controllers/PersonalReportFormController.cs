using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    public class PersonalReportFormController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();
        // GET: Admin/PersonalReportForm
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Formreport()
        {
            return View();
        }

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