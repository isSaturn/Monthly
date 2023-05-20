using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Secretary.Controllers
{
    [Authorize(Roles = "Thư ký")]
    public class ReportPeriodsController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Secretary/ReportPeriods
        public ActionResult Index(int id)
        {
            var reportPeriods = db.ReportPeriods.Where(r => r.report_year_id == id).ToList();
            return View(reportPeriods);
        }
        // GET: Secretaryy/ReportPeriods/Edit
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

        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(ReportPeriod reportPeriod)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reportPeriod).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ThongBao"] = "Sửa kỳ báo cáo thành công";
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
        public ActionResult FormDepartmentEdit(int? idReport, string data)
        {
            var form = db.FormDepartmentReports.Find(idReport);
            if (form != null)
            {
                if (string.IsNullOrEmpty(data))
                    return Json("Vui lòng chọn danh mục", JsonRequestBehavior.AllowGet);
                db.FormDepartmentReportDetails.RemoveRange(form.FormDepartmentReportDetails);
                db.Entry(form).State = EntityState.Modified;
                db.SaveChanges();
                if (data.IndexOf("-") != -1)
                {
                    foreach (var item in data.Split('-'))
                    {
                        FormDepartmentReportDetail formDepartmentReportDetail = new FormDepartmentReportDetail();
                        formDepartmentReportDetail.form_department_report_id = (int)idReport;
                        formDepartmentReportDetail.category_id = Convert.ToInt32(item);
                        db.FormDepartmentReportDetails.Add(formDepartmentReportDetail);
                    }
                    db.SaveChanges();
                }
                return Content("Success");
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
        public ActionResult FormPersonalEdit(int? idReport, string data)
        {
            var form = db.FormPersonalReports.Find(idReport);
            if (form != null)
            {
                if (form.FormPersonalReportDetails.Where(f => f.PersonalReportDetails.Count() > 0).Count() > 0)
                    return Content("Exist");

                if (string.IsNullOrEmpty(data))
                    return Content("Vui lòng chọn danh mục");
                db.FormPersonalReportDetails.RemoveRange(form.FormPersonalReportDetails);
                db.Entry(form).State = EntityState.Modified;
                db.SaveChanges();
                if (data.IndexOf("-") != -1)
                {
                    foreach (var item in data.Split('-'))
                    {
                        FormPersonalReportDetail formPersonalReportDetail = new FormPersonalReportDetail();
                        formPersonalReportDetail.form_personal_report_id = (int)idReport;
                        formPersonalReportDetail.category_id = Convert.ToInt32(item);
                        db.FormPersonalReportDetails.Add(formPersonalReportDetail);
                    }
                    db.SaveChanges();
                }
                return Content("Success");
            }
            return Content("Không tồn tại");
        }
        //staff view edit

        public ActionResult FormStaffEdit(int id)
        {
            var form = db.FormStaffReports.Find(id);
            if (form != null)
            {
                Session["FormStaffEdit-lstCategory"] = db.Categories.ToList();
                return View(form);

            }
            return RedirectToAction("Index", "ReportPeriods");
        }
        //personal edit

        [HttpPost]
        public ActionResult FormStaffEdit(int? idReport, string data)
        {
            var form = db.FormStaffReports.Find(idReport);
            if (form != null)
            {
                if (form.FormStaffReportDetails.Where(f => f.StaffReportDetails.Count() > 0).Count() > 0)
                    return Content("Exist");

                if (string.IsNullOrEmpty(data))
                    return Content("Vui lòng chọn danh mục");
                db.FormStaffReportDetails.RemoveRange(form.FormStaffReportDetails);
                db.Entry(form).State = EntityState.Modified;
                db.SaveChanges();
                if (data.IndexOf("-") != -1)
                {
                    foreach (var item in data.Split('-'))
                    {
                        FormStaffReportDetail formStaffReportDetail = new FormStaffReportDetail();
                        formStaffReportDetail.form_staff_report_id = (int)idReport;
                        formStaffReportDetail.category_id = Convert.ToInt32(item);
                        db.FormStaffReportDetails.Add(formStaffReportDetail);
                    }
                    db.SaveChanges();
                }
                return Content("Success");
            }
            return Content("Không tồn tại");
        }
        //department detail
        public ActionResult FormDepartmentDetail(int id)
        {
            var formDepartmentDetail = db.FormDepartmentReportDetails.Where(f => f.form_department_report_id == id).ToList();
            if (!formDepartmentDetail.Any())
            {
                ViewBag.FormDep = true;
            }
            return View(formDepartmentDetail);
        }
        //personal detail
        public ActionResult FormPersonalDetail(int id)
        {
            var formPersonalDetail = db.FormPersonalReportDetails.Where(f => f.form_personal_report_id == id).ToList();

            if (!formPersonalDetail.Any())
            {
                ViewBag.FormPer = true;
            }
            return View(formPersonalDetail);
        }
        //staff detail
        public ActionResult FormStaffDetail(int id)
        {
            var formStaffDetail = db.FormStaffReportDetails.Where(f => f.form_staff_report_id == id).ToList();
            if (!formStaffDetail.Any())
            {
                ViewBag.FormSta = true;
            }
            return View(formStaffDetail);
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
