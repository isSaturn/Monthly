using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    public class FacultyController : Controller
    {

        CP25Team04Entities db = new CP25Team04Entities();

        // GET: Admin/Faculty
        public ActionResult Index()
        {
            return View(db.Faculties.ToList());
        }

        // GET: Admin/Faculty/Details/5
        /*public ActionResult Details(int? id)
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
        }*/

        // GET: Admin/Faculty/Create
        public ActionResult Create()
        {
            return View(new Faculty());
        }

        // POST: Admin/Faculty/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "faculty_id,faculty_name")] Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                db.Faculties.Add(faculty);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(faculty);
        }

        // GET: Admin/Faculty/Edit/5
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

        // GET: Admin/Faculty/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Faculty/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult AttendenceList(int? id)
        {
            var lstAtten = db.Profiles.Where(p => p.faculty_id == id).ToList();
            return View(lstAtten);
        }
    }
}
