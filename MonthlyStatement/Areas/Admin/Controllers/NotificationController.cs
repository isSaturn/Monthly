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


namespace MonthlyStatement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Quản trị viên")]

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

                        
                        /*using (MailMessage mailMessage = new MailMessage())
                        {
                            mailMessage.From = new MailAddress("reportmonthlyk25pm04@gmail.com", "Report Monthly");
                            mailMessage.Subject = "Thông báo " + check_month.report_period_name;
                            mailMessage.IsBodyHtml = true;
                            mailMessage.Body =
                            @"<body>
                         <div>
    <p style=""font-weight:normal;text-align:justify;margin-top:0;margin-bottom:0;line-height:1.8;"">
        <span style=""color: rgb(24, 26, 28); font-size: 20pt; font-family: Times New Roman , serif, EmojiFont; font-weight: bold; "">Các cán bộ công chức nhân viên vui lòng hoàn thành " + check_month.report_period_name + @" </span>
    </p>
    <br aria-hidden=""true"">

    <p style=""font-weight:normal;text-align:justify;margin-top:0;margin-bottom:0;line-height:1.8;"">
<span style="" font-size:13pt; font-family: Times New Roman , serif, EmojiFont "">Thời gian bắt đầu từ ngày: <b> 
" + check_month.start_date.Value.ToString("dd/MM/yyyy") + @"</b></span>
        <span style="" font-size:13pt; font-family: Times New Roman , serif, EmojiFont "">cho đến ngày: <b> 
" + check_month.end_date.Value.ToString("dd/MM/yyyy") + @"</b></span>
    </p>
</div>
</body>";
                            *//*for (int i = 0; i < list_send.Length; i++)
                            {
                                mailMessage.To.Add((list_send[i].Email));
                            }*//*

                            mailMessage.To.Add("khuong.197pm09466@vanlanguni.vn");

                            Notification notification = new Notification();
                            notification.notification_date = DateTime.Now;
                            notification.notification_content =
                                "Các cán bộ công chức nhân viên vui lòng hoàn thành " + check_month.report_period_name + ". Thời gian bắt đầu từ ngày: " + check_month.start_date.Value.ToString("dd/MM/yyyy") + " cho đến ngày: " + check_month.end_date.Value.ToString("dd/MM/yyyy");
                            notification.status = "Đã thông báo";
                            db.Notifications.Add(notification);
                            db.SaveChanges();


                            using (SmtpClient smtp = new SmtpClient())
                            {
                                smtp.Host = "smtp.gmail.com";
                                smtp.EnableSsl = true;
                                NetworkCredential cred = new NetworkCredential("reportmonthlyk25pm04@gmail.com", "qumpsuequgwjjeuc");
                                smtp.UseDefaultCredentials = true;
                                smtp.Credentials = cred;
                                smtp.Port = 587;

                                smtp.Send(mailMessage);
                            }
                        }*/
                    }
                    

                }
            }
            return Content("Success");
        }

    }
}

