using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Personal.Controllers
{
    [Authorize(Roles = "Nhân viên - Giảng viên")]

    public class ReportController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();
        // GET: Personal/Report
        public ActionResult Personal()
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            var per = db.PersonalReports.Where(p => p.account_id == accID).ToList();
            return View(per);
        }
        public ActionResult PersonalDetail(int id)
        {
            var form = db.PersonalReports.Find(id);
            return View(form);
        }
    }
}