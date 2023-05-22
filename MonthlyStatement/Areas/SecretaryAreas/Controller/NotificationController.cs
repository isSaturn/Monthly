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


namespace MonthlyStatement.Areas.SecretaryAreas.Controllers
{
    [Authorize(Roles = "Thư ký")]

    public class NotificationController : Controller
    {

        private ApplicationAccountManager _userManager;
        private ApplicationSignInManager _signInManager;
        private CP25Team04Entities db = new CP25Team04Entities();

       
        public NotificationController()
        {
        }
        public NotificationController(ApplicationAccountManager userManager, ApplicationSignInManager signInManager)
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
        public NotificationController(ApplicationAccountManager userManager)
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
        // GET: Admin/Notification
        public ActionResult Index()
        {
            var list_notification = db.Notifications.ToList();
            return View(list_notification);
        }

        public async Task<ActionResult> SendMailAsync()
        {

            var current_time = DateTime.Now;
            var check_year = db.ReportYears.FirstOrDefault(y => y.year == current_time.Year);
            var check_month = db.ReportPeriods.FirstOrDefault(m => m.start_date.Value.Month == current_time.Month);

            var list_send = db.AspNetUsers.Where(s => s.Id != null).ToArray();


            if (db.Notifications.FirstOrDefault(n => n.notification_date.Value.Month == current_time.Month && n.notification_date.Value.Year == current_time.Year) == null)
            {
                if (check_year != null)
                {
                    if (check_month != null)
                    {
                        Notification notification = new Notification();
                        notification.notification_date = DateTime.Now;
                        notification.notification_content =
                            "Các cán bộ công chức nhân viên vui lòng hoàn thành " + check_month.report_period_name + ". Thời gian bắt đầu từ ngày: " + check_month.start_date.Value.ToString("dd/MM/yyyy") + " cho đến ngày: " + check_month.end_date.Value.ToString("dd/MM/yyyy");
                        notification.status = "Đã thông báo";
                        db.Notifications.Add(notification);
                        db.SaveChanges();


                        for (int i = 0; i < list_send.Length; i++)
                        {
                            await UserManager.SendEmailAsync(list_send[i].Id,
                           "Thông báo " + check_month.report_period_name,
                           "Các cán bộ công chức nhân viên vui lòng hoàn thành " + check_month.report_period_name + ". Thời gian bắt đầu từ ngày: " + check_month.start_date.Value.ToString("dd/MM/yyyy") + " cho đến ngày: " + check_month.end_date.Value.ToString("dd/MM/yyyy"));
                        }

                        

                        return Json(new { status = true, message = "Gửi thành công!" }, JsonRequestBehavior.AllowGet);
                        
                    }
                    

                }
            }
            return Content("Success");
        }

    }
}

