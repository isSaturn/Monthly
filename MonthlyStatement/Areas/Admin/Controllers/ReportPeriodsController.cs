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
            var reportPeriods = db.ReportPeriods.Where(r => r.report_year_id == id).ToList();
            return View(reportPeriods);
        }
        // GET: Admin/ReportPeriods/Edit
        [HttpGet]
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
            ViewBag.report_year_id = new SelectList(db.ReportYears, "report_year_id", "report_year_id", reportPeriod.report_year_id);
            return View(reportPeriod);
        }

        // POST: Admin/ReportPeriods/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "report_period_id,start_date,end_date,report_period_name,deadline_date,report_year_id")] ReportPeriod reportPeriod)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reportPeriod).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "ReportPeriods");
            }
            ViewBag.report_year_id = new SelectList(db.ReportYears, "report_year_id", "report_year_id", reportPeriod.report_year_id);
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
        //department view edit

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
        //department edit
        [HttpPost]
        public ActionResult FormDepartmentEdit(int idReport, string data)
        {
            var form = db.FormDepartmentReports.Find(idReport);
            if (form != null)
            {
                if (string.IsNullOrEmpty(data))
                    return Json("Vui lòng chọn danh mục", JsonRequestBehavior.AllowGet);
                db.FormDepartmentReportDetails.RemoveRange(form.FormDepartmentReportDetails);
                form.user_name = Session["Email"].ToString();
                db.Entry(form).State = EntityState.Modified;
                db.SaveChanges();
                if (data.IndexOf("-") != -1)
                {
                    foreach (var item in data.Split('-'))
                    {
                        FormDepartmentReportDetail formDepartmentReportDetail = new FormDepartmentReportDetail();
                        formDepartmentReportDetail.form_department_report_id = idReport;
                        formDepartmentReportDetail.category_id = Convert.ToInt32(item);
                        db.FormDepartmentReportDetails.Add(formDepartmentReportDetail);
                    }
                    db.SaveChanges();
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            return Json("Không tồn tại", JsonRequestBehavior.AllowGet);
        }
        //personal view edit

        public ActionResult FormPersonalEdit(int id)
        {
            var form = db.FormPersonalReports.Find(id);
            if (form != null)
            {
                Session["FormPersonalEdit-lstCategory"] = db.Categories.ToList();
                return View(form);

            }
            return RedirectToAction("Index", "ReportPeriods");
        }
        //personal edit

        [HttpPost]
        public ActionResult FormPersonalEdit(int idReport, string data)
        {
            var form = db.FormPersonalReports.Find(idReport);
            if (form != null)
            {
                if (string.IsNullOrEmpty(data))
                    return Json("Vui lòng chọn danh mục", JsonRequestBehavior.AllowGet);
                db.FormPersonalReportDetails.RemoveRange(form.FormPersonalReportDetails);
                db.Entry(form).State = EntityState.Modified;
                db.SaveChanges();
                if (data.IndexOf("-") != -1)
                {
                    foreach (var item in data.Split('-'))
                    {
                        FormPersonalReportDetail formPersonalReportDetail = new FormPersonalReportDetail();
                        formPersonalReportDetail.form_personal_report_id = idReport;
                        formPersonalReportDetail.category_id = Convert.ToInt32(item);
                        db.FormPersonalReportDetails.Add(formPersonalReportDetail);
                    }
                    db.SaveChanges();
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            return Json("Không tồn tại", JsonRequestBehavior.AllowGet);
        }
        //department detail
        public ActionResult FormDepartmentDetail(int id)
        {
            var formDepartmentDetail = db.FormDepartmentReportDetails.Where(f =>f.form_department_report_id == id).ToList();
            return View(formDepartmentDetail);
        }
        //personal detail
        public ActionResult FormPersonalDetail(int id)
        {
            var formPersonalDetail = db.FormPersonalReportDetails.Where(f => f.form_personal_report_id == id).ToList();
            return View(formPersonalDetail);
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
