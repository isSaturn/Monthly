using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Personal.Controllers
{
        // GET: Personal/NotificationPersonal
    public class NotificationPersonalController : Controller
    {
        private ApplicationAccountManager _userManager;
        private ApplicationSignInManager _signInManager;
        private CP25Team04Entities db = new CP25Team04Entities();


        public NotificationPersonalController()
        {
        }
        public NotificationPersonalController(ApplicationAccountManager userManager, ApplicationSignInManager signInManager)
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
        public NotificationPersonalController(ApplicationAccountManager userManager)
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
        // GET: Personal/NotificationPersonal
        public ActionResult Index()
        {
            var list_notification = db.Notifications.ToList();
            return View(list_notification);
        }

        public async Task<ActionResult> SendMailPersonalAsync()
        {
            string email_gv = User.Identity.Name;
            string userId = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(email_gv.ToLower().Trim())).Id;

            var current_time = DateTime.Now;
            var check_year = db.ReportYears.FirstOrDefault(y => y.year == current_time.Year);
            var check_month = db.ReportPeriods.FirstOrDefault(m => m.start_date.Value.Month == current_time.Month);

            var list_send = db.AspNetUsers.Where(s => s.Id != null).ToList();
            var id_faculty = db.Profiles.FirstOrDefault(m => m.account_id == userId).faculty_id;
            var Role = list_send.Where(y => y.AspNetRoles.FirstOrDefault(r => r.Name == "Bộ môn") != null && y.Profiles.FirstOrDefault(r => r.faculty_id == id_faculty) != null).ToArray();

            if (check_year != null)
            {
                if (check_month != null)
                {
                    Notification notification = new Notification();
                    notification.notification_date = DateTime.Now;
                    notification.notification_content =
                        "Thông báo đã hoàn thành " + check_month.report_period_name + " (Giảng viên): " +
                        "Giảng viên đã hoàn thành " + check_month.report_period_name + "." + " Có vấn đề thì vui lòng bình luận dưới phần báo cáo " + email_gv;
                    notification.status = "Đã thông báo";
                    notification.account_id = userId;
                    db.Notifications.Add(notification);
                    db.SaveChanges();


                    for (int i = 0; i < Role.Length ; i++)
                    {
                        await UserManager.SendEmailAsync(Role[i].Id,
                       "Thông báo đã hoàn thành " + check_month.report_period_name + " (Giảng viên)",
                       "Giảng viên đã hoàn thành " + check_month.report_period_name + "." + " Có vấn đề thì vui lòng bình luận dưới phần báo cáo " + email_gv);
                    }



                    return Json(new { status = true, message = "Gửi thành công!" }, JsonRequestBehavior.AllowGet);

                }


            }
            return Content("Success");
        }
    }
}