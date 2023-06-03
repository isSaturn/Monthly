using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.FacultyAreas.Controllers
{
    [Authorize(Roles = "Ban phòng khoa,Thư ký")]

    public class FacManagerController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();
        private ApplicationAccountManager _accountManager;

        public FacManagerController()
        {
        }

        public FacManagerController(ApplicationAccountManager accountManager)
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
        // GET: FacultyAreas/FacManager
        public ActionResult Index()
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            var check_Faculty = db.Profiles.FirstOrDefault(x => x.account_id == accID);
            Session["faculty"] = check_Faculty.faculty_id;
            var data = db.Faculties.Where(y => y.faculty_id == check_Faculty.faculty_id).ToList();
            return View(data);
        }

        [HttpGet]
        public ActionResult FacMember(int id)
        {
            Session["add-user-faculty"] = db.Profiles.Where(p => p.faculty_id == null).ToList();
            var listFacMember = db.Faculties.Find(id);
            return View(listFacMember);
        }

        [HttpPost]
        public ActionResult AddUser(int id, int khoa, string email)
        {
            var user = db.Profiles.Find(id);
            user.faculty_id = khoa;
            user.email = email;

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return Content("Success");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faculty faculty = db.Faculties.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }
            return View(faculty);
        }

        // POST: Admin/Faculty/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "faculty_id,faculty_name")] Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                db.Entry(faculty).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(faculty);
        }
        [HttpGet]
        public ActionResult EditRole(string id,string returnUrl)
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
            ViewBag.returnUrl = returnUrl;
            return View(db.AspNetUsers.Find(id));
        }

        [HttpPost]
        public ActionResult EditRole(AspNetUser aspNetUser, string role_id, string returnUrl)
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

            return Redirect(returnUrl);
        }

        public ActionResult DeleteFaculty(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var prof = db.Profiles.FirstOrDefault(t => t.account_id.Equals(id));
                prof.faculty_id = null;
                db.Entry(prof).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }
    }
}