using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Staff.Controllers
{
    [Authorize(Roles = "Nhân viên")]
    public class StaffReportController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();
        // GET: Staff/Report
        public ActionResult Staff()
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            var per = db.StaffReports.Where(p => p.account_id == accID).ToList();
            return View(per);
        }
        public ActionResult StaffDetail(int id)
        {
            int per = db.StaffReports.Find(id).report_period_id;
            var form = db.FormStaffReports.FirstOrDefault(f => f.report_period_id == per);
            return View(form);
        }
    }
}