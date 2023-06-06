using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Department.Controllers
{
    [Authorize(Roles = "Bộ môn")]
    public class ListDepartmentMemberController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();
        // GET: Department/ListDepartmentMember
        public ActionResult Index()
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            var check_Faculty = db.Profiles.FirstOrDefault(x => x.account_id == accID);
            var data = db.Faculties.FirstOrDefault(y => y.faculty_id == check_Faculty.faculty_id);
            var check_Dep = db.DepartmentLists.Where(dep => dep.faculty_id == data.faculty_id);
            return View(check_Dep);
        }
        public ActionResult Create()
        {
            ViewBag.faculty_id = new SelectList(db.Faculties, "faculty_id", "faculty_name");
            return View();
        }
        [HttpPost]
        public ActionResult Create([Bind(Include = "department_id,department_name,faculty_id")] DepartmentList departmentList)
        {
            if (ModelState.IsValid)
            {
                db.DepartmentLists.Add(departmentList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.faculty_id = new SelectList(db.Faculties, "faculty_id", "faculty_name", departmentList.faculty_id);
            return View(departmentList);
        }
        // GET: Department/DepartmentLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentList departmentList = db.DepartmentLists.Find(id);
            if (departmentList == null)
            {
                return HttpNotFound();
            }
            ViewBag.faculty_id = new SelectList(db.Faculties, "faculty_id", "faculty_name", departmentList.faculty_id);
            return View(departmentList);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "department_id,department_name,faculty_id")] DepartmentList departmentList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departmentList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.faculty_id = new SelectList(db.Faculties, "faculty_id", "faculty_name", departmentList.faculty_id);
            return View(departmentList);
        }
        public ActionResult ListDepMem(int id)
        {
            Session["add-user-dep"] = db.Profiles.Where(p => p.department_id == null).ToList();
            var reportPeriods = db.DepartmentLists.Find(id);
            return View(reportPeriods);
        }
        [HttpPost]
        public ActionResult AddUser(int id, int khoa)
        {
            var user = db.Profiles.Find(id);
            user.department_id = khoa;

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return Content("Success");
        }
        public ActionResult DeleteDep(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var prof = db.Profiles.FirstOrDefault(t => t.account_id.Equals(id));
                prof.department_id = null;
                db.Entry(prof).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}