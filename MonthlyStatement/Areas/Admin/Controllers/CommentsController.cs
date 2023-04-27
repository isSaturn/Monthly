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
using Microsoft.AspNet.Identity.Owin;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    public class CommentsController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        private ApplicationAccountManager _userManager;
        private ApplicationSignInManager _signInManager;


        public CommentsController()
        {
        }
        public CommentsController(ApplicationAccountManager userManager, ApplicationSignInManager signInManager)
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
        public CommentsController(ApplicationAccountManager userManager)
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

        [HttpPost]
        // GET: Admin/Comments
        public async Task<ActionResult> PostCommentPersonal(string CommentText, int id)
        {
            string emails = User.Identity.Name;
            string userId = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;

            string account_id = db.PersonalReports.FirstOrDefault(t => t.personal_report_id == id).account_id;
            string mail = db.AspNetUsers.FirstOrDefault(m => m.Id == account_id).Email;

            Comment c = new Comment();
            c.comment_content = CommentText;
            c.comment_date = DateTime.Now;
            c.account_id = userId;
            c.personal_report_id = id;
           /* db.Comments.Add(c);
            db.SaveChanges();*/


            await UserManager.SendEmailAsync(mail,
                           "Thông báo bình luận từ " + userId,
                           "Nội dung: " + c.comment_content);

            return RedirectToAction("Index", "ListReportPersonal");
        }


        [HttpPost]
        // GET: Admin/Comments
        public async Task<ActionResult> PostCommentDepartment(string CommentText, int id)
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


            await UserManager.SendEmailAsync(mail,
                           "Thông báo bình luận từ " + userId,
                           "Nội dung: " + c.comment_content);

            return RedirectToAction("Index", "ListReportDepartment");
        }
    }
}