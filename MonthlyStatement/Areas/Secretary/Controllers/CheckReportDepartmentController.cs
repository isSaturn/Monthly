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
        // GET: Admin/Comments
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
    }
}