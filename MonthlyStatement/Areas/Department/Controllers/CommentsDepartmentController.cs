using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MonthlyStatement.Areas.Admin.Controllers;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Department.Controllers
{
    [Authorize(Roles = "Bộ môn")]
    public class CommentsDepartmentController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        private ApplicationAccountManager _userManager;
        private ApplicationSignInManager _signInManager;


        public CommentsDepartmentController()
        {
        }
        public CommentsDepartmentController(ApplicationAccountManager userManager, ApplicationSignInManager signInManager)
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
        public CommentsDepartmentController(ApplicationAccountManager userManager)
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
        // GET: Department/CommentsDepartment
        public ActionResult PostComment(string CommentText, int id)
        {
            string emails = User.Identity.Name;
            string userId = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;

            /*string account_id = db.DepartmentReports.FirstOrDefault(t => t.Department_report_id == id).account_id;
            string mail = db.AspNetUsers.FirstOrDefault(m => m.Id == account_id).Email;*/


            var list_send_comment = db.Comments.Where(p => p.department_report_id == id).ToArray();


            Comment c = new Comment();
            c.comment_content = CommentText;
            c.comment_date = DateTime.Now;
            c.account_id = userId;
            c.department_report_id = id;
            db.Comments.Add(c);
            db.SaveChanges();

            //for (int i = 0; i < list_send_comment.Length; i++)
            //{
            //    await UserManager.SendEmailAsync(list_send_comment[i].account_id,
            //                  "Thông báo bình luận từ " + userId,
            //                  "Nội dung: " + c.comment_content);

            //}
            return RedirectToAction("Department", "Report");
        }

    }
}