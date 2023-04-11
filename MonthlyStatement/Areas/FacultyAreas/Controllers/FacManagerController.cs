using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.FacultyAreas.Controllers
{
    [Authorize(Roles = "Ban phòng khoa")]

    public class FacManagerController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();

        // GET: FacultyAreas/FacManager
        public ActionResult Indexx()
        {
            string manguoidung = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault().Id;
            int makhoa = (int)db.Profiles.Where(p => p.account_id == manguoidung).FirstOrDefault().faculty_id;
            var data = db.Faculties.Where(y => y.faculty_id == makhoa).ToList();
            return View(data);
        }
        public ActionResult FacMember(int id)
        {
            var reportPeriods = db.Faculties.Find(id);
            return View(reportPeriods);
        }
        public ActionResult Index()
        {
            string manguoidung = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault().Id;
            int makhoa = (int)db.Profiles.Where(p => p.account_id == manguoidung).FirstOrDefault().faculty_id;
            var lstpersonal = db.PersonalReports.Where(p => p.AspNetUser.AspNetRoles.Where(a => a.Id.Equals("5")).Count() > 0).ToList();
            return View(lstpersonal);
        }
    }
}