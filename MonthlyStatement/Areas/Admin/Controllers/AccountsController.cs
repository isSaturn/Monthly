using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Quản trị viên,Ban phòng khoa")]

    public class AccountsController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        private ApplicationAccountManager _accountManager;

        public AccountsController()
        {
        }

        public AccountsController(ApplicationAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public ApplicationAccountManager AccountManager
        {
            get
            {
                return _accountManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationAccountManager>();
            }
            private set
            {
                _accountManager = value;
            }
        }

        // GET: Admin/Accounts
        public ActionResult Index()
        {
            return View(db.AspNetUsers.ToList());
        }

        // GET: Admin/Accounts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            // Get user role
            var query_role = db.AspNetUsers.Find(id).AspNetRoles.FirstOrDefault();
            if (query_role != null)
            {
                // Set selected role
                ViewBag.role_id = new SelectList(db.AspNetRoles, "id", "name", query_role.Id);
            }
            else
            {
                // Populate new role select list
                ViewBag.role_id = new SelectList(db.AspNetRoles, "id", "name");
            }

            return View(db.AspNetUsers.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(AspNetUser aspNetUser, string role_id)
        {
            // Declare variables
            var oldUser = AccountManager.FindById(aspNetUser.Id);
            var oldRole = AccountManager.GetRoles(oldUser.Id).FirstOrDefault();
            var role = db.AspNetRoles.Find(role_id);
            var result = new IdentityResult();

            // Prevent user from editing the last admin role
            int adminCount = db.AspNetUsers.Where(u => u.AspNetRoles.FirstOrDefault().Name == "Admin").Count();
            if (adminCount <= 1 && oldRole == "Admin" && role.Name != "Admin")
            {
                return Json(new { result.Errors }, JsonRequestBehavior.AllowGet);
            }

            if (oldRole == null)
            {
                // Add user to role
                result = AccountManager.AddToRole(aspNetUser.Id, role.Name);
            }
            else
            {
                // Update user role
                AccountManager.RemoveFromRole(aspNetUser.Id, oldRole);
                result = AccountManager.AddToRole(aspNetUser.Id, role.Name);
            }

            return RedirectToAction("Index", "Accounts");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
