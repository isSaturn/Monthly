using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.FacultyAreas.Controllers
{
    [Authorize(Roles = "Ban phòng khoa")]

    public class FacReportController : Controller
    {
        private ApplicationAccountManager _userManager;
        CP25Team04Entities db = new CP25Team04Entities();

        public FacReportController()
        {
        }
        public FacReportController(ApplicationAccountManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationAccountManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationAccountManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: FacultyAreas/FacReport
        public ActionResult Index()
        {
            var current_year = DateTime.Now.Year;
            int check = db.ReportYears.Where(y => y.year == current_year).Count();
            return View(db.ReportYears.OrderByDescending(y => y.year).ToList());
        }
        public ActionResult FacReportPerior(int id)
        {

            var reportPeriods = db.ReportPeriods.Where(r => r.report_year_id == id).ToList();
            return View(reportPeriods);

        }
        public ActionResult FacManagerReportPerAndDep(int? id)
        {
            var form = db.ReportPeriods.Find(id);
            if (form != null)
            {
                return View(form);
            }
            return RedirectToAction("Index", "FacReport");
        }
        public ActionResult FacManagerReportPer()
        {
            string manguoidung = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault().Id;
            int makhoa = (int)db.Profiles.Where(p => p.account_id == manguoidung).FirstOrDefault().faculty_id;
            var lstpersonal = db.PersonalReports.Where(p => p.AspNetUser.AspNetRoles.Where(a => a.Id == "5").Any() && p.AspNetUser.Profiles.Where(pro => pro.faculty_id == makhoa).Count() > 0).ToList();
            return View(lstpersonal);
        }

    }
}