using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.FacultyAreas.Controllers
{
    [Authorize(Roles = "Ban phòng khoa,Thư ký")]

    public class FacManagerController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();

        // GET: FacultyAreas/FacManager
        public ActionResult Index()
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            var check_Faculty = db.Profiles.FirstOrDefault(x => x.account_id == accID);
            Session["faculty"] = check_Faculty.faculty_id;
            var data = db.Faculties.Where(y => y.faculty_id == check_Faculty.faculty_id).ToList();
            return View(data);
        }
        public ActionResult FacMember(int id)
        {
            var reportPeriods = db.Faculties.Find(id);
            return View(reportPeriods);
        }
    }
}