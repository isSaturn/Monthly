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
    [Authorize(Roles = "Quản trị viên")]
    public class ReportPeriodsController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Admin/ReportPeriods
        public ActionResult Index(int id)
        {
            return View(db.ReportPeriods.Where(i => i.report_year_id == id).ToList());
        }

        // GET: Admin/ReportPeriods/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportPeriod reportPeriod = db.ReportPeriods.Find(id);
            if (reportPeriod == null)
            {
                return HttpNotFound();
            }
            return View(reportPeriod);
        }

        [HttpGet]
        // GET: Admin/ReportPeriods/Create
        public ActionResult Create()
        {
            return View(new ReportPeriod());
        }

        // POST: ReportPeriods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public ActionResult Create([Bind(Include = "report_period_id,start_date,end_date,report_period_name,active")] ReportPeriod reportPeriod)
        {
            if (ModelState.IsValid)
            {
                db.ReportPeriods.Add(reportPeriod);
                db.SaveChanges();
                FormDepartmentReport formDepartmentReport = new FormDepartmentReport();
                formDepartmentReport.report_period_id = reportPeriod.report_period_id;
                db.FormDepartmentReports.Add(formDepartmentReport);

                FormPersonalReport formPersonalReport = new FormPersonalReport();
                formPersonalReport.report_period_id = reportPeriod.report_period_id;
                db.FormPersonalReports.Add(formPersonalReport);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(reportPeriod);
        }

        [HttpGet]
        // GET: Admin/ReportPeriods/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportPeriod reportPeriod = db.ReportPeriods.Find(id);
            if (reportPeriod == null)
            {
                return HttpNotFound();
            }
            return View(reportPeriod);
        }

        // POST: Admin/ReportPeriods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "report_period_id,start_date,end_date,report_period_name,deadline_date,report_year_id")] ReportPeriod reportPeriod)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reportPeriod).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reportPeriod);
        }

        [HttpGet]
        public ActionResult FormReports(int? id)
        {
            var form = db.ReportPeriods.Find(id);
            if (form != null)
            {
                return View(form);
            }
            return RedirectToAction("Index", "ReportPeriods");
        }
        public ActionResult FormDepartmentEdit(int id)
        {
            var form = db.FormDepartmentReports.Find(id);
            if (form != null)
            {
                Session["FormDepartmentEdit-lstCategory"] = db.Categories.ToList();
                return View(form);
            }
            return RedirectToAction("Index", "ReportPeriods");
        }
        public ActionResult FormPersonalEdit(int id)
        {
            var form = db.FormPersonalReports.FirstOrDefault(f => f.form_personal_report_id == id);
            if (form != null)
            {
                Session["FormPersonalEdit-lstCategory"] = db.Categories.ToList();
                return View(form);

            }
            return RedirectToAction("Index", "ReportPeriods");
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
