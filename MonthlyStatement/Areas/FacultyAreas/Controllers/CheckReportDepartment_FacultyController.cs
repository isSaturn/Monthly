using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.FacultyAreas.Controllers
{
    public class CheckReportDepartment_FacultyController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();

        private ApplicationAccountManager _userManager;
        private ApplicationSignInManager _signInManager;


        public CheckReportDepartment_FacultyController()
        {
        }
        public CheckReportDepartment_FacultyController(ApplicationAccountManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public CheckReportDepartment_FacultyController(ApplicationAccountManager userManager)
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

        // GET: FacultyAreas/CheckReportDepartment_Faculty
        public ActionResult Index()
        {
            return View(db.DepartmentReports.ToList());
        }
        public ActionResult Detail(int id)
        {
            var per = db.DepartmentReports.Find(id);
            var form = db.FormDepartmentReports.FirstOrDefault(f => f.report_period_id == per.report_period_id);
            ViewBag.accID = per.account_id;
            return View(form);
        }

        public async Task<ActionResult> PostCommentFaculty(string CommentText, int id)
        {
            string emails = User.Identity.Name;
            string userId = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;

            string account_id = db.DepartmentReports.FirstOrDefault(t => t.department_report_id == id).account_id;
            string mail = db.AspNetUsers.FirstOrDefault(m => m.Id == account_id).Email;

            Comment c = new Comment();
            c.comment_content = CommentText;
            c.comment_date = DateTime.Now;
            c.account_id = userId;
            c.department_report_id = id;
            db.Comments.Add(c);
            db.SaveChanges();


            await UserManager.SendEmailAsync(account_id,
                           "Thông báo bình luận từ " + emails,
                           "Nội dung: " + c.comment_content);

            return RedirectToAction("Detail", "CheckReportDepartment_Faculty", new { id = c.department_report_id });
        }

        public async Task<ActionResult> SendMailFacultySuccessAsync(int id)
        {
            string email_tk = User.Identity.Name;
            string userId = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(email_tk.ToLower().Trim())).Id;

            string account_id = db.DepartmentReports.FirstOrDefault(t => t.department_report_id == id).account_id;
            string mail = db.AspNetUsers.FirstOrDefault(m => m.Id == account_id).Email;

            var current_time = DateTime.Now;
            var check_year = db.ReportYears.FirstOrDefault(y => y.year == current_time.Year);
            var check_month = db.ReportPeriods.FirstOrDefault(m => m.start_date.Value.Month == current_time.Month);

            var list_send = db.AspNetUsers.Where(s => s.Id != null).ToList();
            var id_faculty = db.Profiles.FirstOrDefault(m => m.account_id == userId).faculty_id;
            var Role = list_send.Where(y => y.AspNetRoles.FirstOrDefault(r => r.Name == "Bộ môn") != null && y.Profiles.FirstOrDefault(r => r.faculty_id == id_faculty) != null).ToArray();

            var name_faculty = db.Faculties.FirstOrDefault(i => i.faculty_id == id_faculty).faculty_name;

            if (check_year != null)
            {
                if (check_month != null)
                {
                    Notification notification = new Notification();
                    notification.notification_date = DateTime.Now;
                    notification.notification_content =
                       "Thông báo duyệt thành công " + check_month.report_period_name + " của Bộ môn. " + " khoa " + name_faculty +
                        "Khoa " + name_faculty  + " thông báo " + check_month.report_period_name + " của bộ môn " + " thuộc khoa " + name_faculty + " được chấp nhận ";
                    notification.status = "Duyệt thành công";
                    notification.account_id = userId;
                    db.Notifications.Add(notification);
                    db.SaveChanges();

                    var per = db.DepartmentReports.Find(id);
                    per.status_faculty = "Đã duyệt";
                    db.Entry(per).State = EntityState.Modified;
                    db.SaveChanges();

                    for (int i = 0; i < Role.Length; i++)
                    {
                        await UserManager.SendEmailAsync(Role[i].Id,
                       "Thông báo duyệt thành công " + check_month.report_period_name + " của Bộ môn " + " khoa " + name_faculty,
                        "Khoa " + name_faculty + " thông báo " + check_month.report_period_name + " của bộ môn " + " thuộc khoa " + name_faculty + " được chấp nhận ");
                    }
                }
            }
            return RedirectToAction("Index", "CheckReportDepartment_Faculty");
        }


        public async Task<ActionResult> SendMailFacultyErrorAsync(int id)
        {
            string email_tk = User.Identity.Name;
            string userId = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(email_tk.ToLower().Trim())).Id;

            string account_id = db.DepartmentReports.FirstOrDefault(t => t.department_report_id == id).account_id;
            string mail = db.AspNetUsers.FirstOrDefault(m => m.Id == account_id).Email;

            var current_time = DateTime.Now;
            var check_year = db.ReportYears.FirstOrDefault(y => y.year == current_time.Year);
            var check_month = db.ReportPeriods.FirstOrDefault(m => m.start_date.Value.Month == current_time.Month);

            var list_send = db.AspNetUsers.Where(s => s.Id != null).ToList();
            var id_faculty = db.Profiles.FirstOrDefault(m => m.account_id == userId).faculty_id;
            var Role = list_send.Where(y => y.AspNetRoles.FirstOrDefault(r => r.Name == "Bộ môn") != null && y.Profiles.FirstOrDefault(r => r.faculty_id == id_faculty) != null).ToArray();

            var name_faculty = db.Faculties.FirstOrDefault(i => i.faculty_id == id_faculty).faculty_name;

            if (check_year != null)
            {
                if (check_month != null)
                {
                    Notification notification = new Notification();
                    notification.notification_date = DateTime.Now;
                    notification.notification_content =
                       "Thông báo duyệt không thành công " + check_month.report_period_name + " của Bộ môn. " + " khoa " + name_faculty +
                        "Khoa " + name_faculty + " Thông báo " + check_month.report_period_name + " của bộ môn " + " thuộc khoa " + name_faculty + " không được chấp nhận" + ". Vui lòng bổ sung.";
                    notification.status = "Duyệt không thành công";
                    notification.account_id = userId;
                    db.Notifications.Add(notification);
                    db.SaveChanges();

                    var per = db.DepartmentReports.Find(id);
                    per.status_faculty = "Chưa duyệt";
                    db.Entry(per).State = EntityState.Modified;
                    db.SaveChanges();

                    for (int i = 0; i < Role.Length; i++)
                    {
                        await UserManager.SendEmailAsync(Role[i].Id,
                       "Thông báo duyệt không thành công " + check_month.report_period_name + " của Bộ môn " + " khoa " + name_faculty,
                        "Khoa " + name_faculty + " thông báo " + check_month.report_period_name + " của bộ môn " + " thuộc khoa " + name_faculty + " không được chấp nhận" + ". Vui lòng bổ sung.");
                    }
                }
            }
            return RedirectToAction("Index", "CheckReportDepartment_Faculty");
        }
    }
}