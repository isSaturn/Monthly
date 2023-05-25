using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Secretary.Controllers
{
    public class CheckReportDepartmentController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();

        private ApplicationAccountManager _userManager;
        private ApplicationSignInManager _signInManager;


        public CheckReportDepartmentController()
        {
        }
        public CheckReportDepartmentController(ApplicationAccountManager userManager, ApplicationSignInManager signInManager)
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
        public CheckReportDepartmentController(ApplicationAccountManager userManager)
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
        // GET: Secretary/CheckReportDepartment
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
        [HttpPost]
        // GET: Secretary/Comments
        public async Task<ActionResult> PostCommentSec(string CommentText, int id)
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

            return RedirectToAction("Detail", "CheckReportDepartment", new { id = c.department_report_id });
        }

        public async Task<ActionResult> SendMailSecretarySuccessAsync(int id)
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
            var Role = list_send.Where(y => y.AspNetRoles.FirstOrDefault(r => r.Name == "Ban phòng khoa") != null && y.Profiles.FirstOrDefault(r => r.faculty_id == id_faculty) != null).ToArray();

            var name_faculty = db.Faculties.FirstOrDefault(i => i.faculty_id == id_faculty).faculty_name;

            if (check_year != null)
            {
                if (check_month != null)
                {
                    Notification notification = new Notification();
                    notification.notification_date = DateTime.Now;
                    notification.notification_content =
                       "Thông báo duyệt thành công " + check_month.report_period_name + " của Bộ môn. " + " khoa " + name_faculty +
                       email_tk + " của Thư ký thông báo " + check_month.report_period_name + " của " + mail + " bộ môn được chấp nhận";
                    notification.status = "Duyệt thành công";
                    db.Notifications.Add(notification);
                    db.SaveChanges();


                    for (int i = 0; i < Role.Length; i++)
                    {
                        await UserManager.SendEmailAsync(Role[i].Id,
                       "Thông báo duyệt thành công " + check_month.report_period_name + " của Bộ môn " + " khoa " + name_faculty,
                       email_tk + " của Thư ký thông báo " + check_month.report_period_name + " của " + mail + " bộ môn được chấp nhận");
                    }

                    return Json(new { status = true, message = "Gửi thành công!" }, JsonRequestBehavior.AllowGet);

                }
            }
            return View("Index");
        }


        public async Task<ActionResult> SendMailSecretaryErrorAsync(int id)
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
            var Role = list_send.Where(y => y.AspNetRoles.FirstOrDefault(r => r.Name == "Ban phòng khoa") != null && y.Profiles.FirstOrDefault(r => r.faculty_id == id_faculty) != null).ToArray();

            var name_faculty = db.Faculties.FirstOrDefault(i => i.faculty_id == id_faculty).faculty_name;

            if (check_year != null)
            {
                if (check_month != null)
                {
                    Notification notification = new Notification();
                    notification.notification_date = DateTime.Now;
                    notification.notification_content =
                       "Thông báo duyệt không thành công " + check_month.report_period_name + " của Bộ môn. " + " khoa " + name_faculty +
                       email_tk + " của Thư ký thông báo " + check_month.report_period_name + " của " + mail + " bộ môn không được chấp nhận";
                    notification.status = "Duyệt không thành công";
                    db.Notifications.Add(notification);
                    db.SaveChanges();


                    for (int i = 0; i < Role.Length; i++)
                    {
                        await UserManager.SendEmailAsync(Role[i].Id,
                       "Thông báo duyệt không thành công " + check_month.report_period_name + " của Bộ môn " + " khoa " + name_faculty,
                       email_tk + " của Thư ký thông báo " + check_month.report_period_name + " của " + mail + " bộ môn không được chấp nhận");
                    }

                    return Json(new { status = true, message = "Gửi thành công!" }, JsonRequestBehavior.AllowGet);

                }
            }
            return View("Index");
        }

    }
}