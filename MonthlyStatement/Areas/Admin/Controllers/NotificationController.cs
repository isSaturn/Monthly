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
using MonthlyStatement.Models;


namespace MonthlyStatement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Quản trị viên")]

    public class NotificationController : Controller
    {


        private CP25Team04Entities db = new CP25Team04Entities();
        // GET: Admin/Notification
        public ActionResult Index()
        {
            var list_notification = db.Notifications.ToList();
            return View(list_notification);
        }

        public ActionResult SendMail()
        {
            var current_time = DateTime.Now;
            var check_year = db.ReportYears.FirstOrDefault(y => y.year == current_time.Year);
            var check_month = db.ReportPeriods.FirstOrDefault(m => m.start_date.Value.Month == current_time.Month);

            var list_send = db.AspNetUsers.Where(s => s.Email != null).ToArray();

            if (check_year != null) 
            {
                if(check_month != null)
                {
                    MailMessage message = new MailMessage();
                    // Tạo nội dung Email
                    message.IsBodyHtml = true;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.SubjectEncoding = System.Text.Encoding.UTF8;

                    message.From = new MailAddress("trankhuong2512001@gmail.com", "Report Monthly");
                    message.Subject = "Thông báo " + check_month.report_period_name;

                    message.Body =
                         @"<div>
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
</div>";

                    for (int i = 0; i < list_send.Length; i++)
                    {
                        message.To.Add((list_send[i].Email));
                    }

                    Notification notification = new Notification();
                    notification.notification_date = DateTime.Now;
                    notification.notification_content = "Thông báo " + check_month.report_period_name;
                    notification.status = "Đã thông báo";
                    db.Notifications.Add(notification);
                    db.SaveChanges();

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("trankhuong2512001@gmail.com", "idyoglvcvcwutpwu");

                    try
                    {
                        smtp.Send(message);
                    }
                    catch (Exception ex)
                    {
                        Session["thongbao-loi"] = ex.Message;
                    }
                }
            }
            return View("Index");
        }

    }
}

