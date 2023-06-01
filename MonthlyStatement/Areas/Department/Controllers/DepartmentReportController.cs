using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Department.Controllers
{
    [Authorize(Roles = "Bộ môn")]
    public class DepartmentReportController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();
        // GET: Department/Report
        public ActionResult Department()
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            var per = db.DepartmentReports.Where(p => p.account_id == accID).ToList();
            return View(per);
        }
        public ActionResult DepartmentDetail(int id)
        {
            int per = db.DepartmentReports.Find(id).report_period_id;
            var form = db.FormDepartmentReports.FirstOrDefault(f => f.report_period_id == per);
            ViewBag.DepDetail = id;
            return View(form);
        }
        public ActionResult DepartmentReportEdit(int id)
        {
            var current_time = DateTime.Now;
            var per = db.DepartmentReports.Find(id);
            var form = db.FormDepartmentReports.FirstOrDefault(f => f.report_period_id == per.report_period_id);
            var check = db.ReportPeriods.FirstOrDefault(d => d.start_date <= form.ReportPeriod.start_date && d.end_date >= form.ReportPeriod.end_date);
            var lstCatDate = db.FormPersonalReports.FirstOrDefault(c => c.ReportPeriod.start_date <= current_time && c.ReportPeriod.end_date >= current_time).form_personal_report_id;

            if (db.FormPersonalReportDetails.Any(n => n.form_personal_report_id == lstCatDate))
            {
                var formDetail = db.FormPersonalReportDetails.Where(f => f.form_personal_report_id == lstCatDate);
                List<PersonalReportDetail> contentPer = new List<PersonalReportDetail>();
                foreach (var item in formDetail)
                {
                    var perDetail = db.PersonalReportDetails.Where(p => p.form_personal_report_detail_id == item.form_personal_report_detail_id);
                    contentPer.AddRange(perDetail);
                }
                var lstCat = formDetail.Select(i => i.Category).ToList();
                ViewBag.CheckListCategory = lstCat;
                ViewBag.MappingContent = contentPer;
            }
            else
            {
                ViewBag.CheckMapping = true;
            }
            ViewBag.PeriodsId = check.report_period_id;
            return View(form);

        }
        [HttpPost]
        public ActionResult DepartmentReportEdit(HttpPostedFileBase fileMinhChung, string data, int? reportperiodid)
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            DepartmentReport pr = new DepartmentReport();
            pr.report_period_id = (int)reportperiodid;
            var pers = db.DepartmentReports.FirstOrDefault(r => r.account_id.Equals(accID));
            var periodId = db.DepartmentReports.Where(c => c.report_period_id == pr.report_period_id);

            pr.status = DateTime.Now.Day <= 21 ? "Đã báo cáo" : "Trễ báo cáo";
            pr.date_report = DateTime.Now;
            pr.account_id = accID;
            db.DepartmentReports.Add(pr);
            db.SaveChanges();

            foreach (var item in periodId)
            {
                db.Comments.Where(c => c.department_report_id == item.department_report_id).ToList().ForEach(c => c.department_report_id = pr.department_report_id);
            }
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Content("Error");
            }

            if (fileMinhChung != null)
            {
                if (fileMinhChung.ContentLength > 0)
                {
                    const string src = "abcdefghijklmnopqrstuvwxyz0123456789";
                    int length = 30;
                    var sb = new StringBuilder();
                    Random RNG = new Random();
                    for (var i = 0; i < length; i++)
                    {
                        var c = src[RNG.Next(0, src.Length)];
                        sb.Append(c);
                    }

                    string path = Path.Combine(Server.MapPath("~/assets/FileMinhChung/"), sb.ToString().Trim() + fileMinhChung.FileName); ;
                    fileMinhChung.SaveAs(path);
                    pr.file_path = path;
                }
            }
            db.DepartmentReportDetails.RemoveRange(pers.DepartmentReportDetails);
            db.DepartmentReports.Remove(pers);
            db.SaveChanges();

            if (data.IndexOf("~") != -1) //Có nhiều form detail
            {
                var lstFrmDetail = data.Split('~');
                foreach (var item in lstFrmDetail)
                {
                    if (item.IndexOf("-") != -1) //có nhiều nội dung trong form
                    {
                        string idFrm = item.Split('=')[0];
                        var lstNoiDung = item.Split('=')[1].Split('-');

                        foreach (var items in lstNoiDung)
                        {
                            DepartmentReportDetail perDetail = new DepartmentReportDetail();
                            perDetail.department_report_id = pr.department_report_id;
                            perDetail.form_department_report_detail_id = Int32.Parse(idFrm);
                            perDetail.department_report_content = items;
                            db.DepartmentReportDetails.Add(perDetail);
                        }
                    }
                    else
                    {
                        string idFrm = item.Split('=')[0];
                        var noiDung = item.Split('=')[1];

                        DepartmentReportDetail perDetail = new DepartmentReportDetail();
                        perDetail.department_report_id = pr.department_report_id;
                        perDetail.form_department_report_detail_id = Int32.Parse(idFrm);
                        perDetail.department_report_content = noiDung;
                        db.DepartmentReportDetails.Add(perDetail);
                    }
                }
            }
            else
            {
                if (data.IndexOf("-") != -1) //có nhiều nội dung trong form
                {
                    string idFrm = data.Split('=')[0];
                    var lstNoiDung = data.Split('=')[1].Split('-');

                    foreach (var items in lstNoiDung)
                    {
                        DepartmentReportDetail perDetail = new DepartmentReportDetail();
                        perDetail.department_report_id = pr.department_report_id;
                        perDetail.form_department_report_detail_id = Int32.Parse(idFrm);
                        perDetail.department_report_content = items;
                        db.DepartmentReportDetails.Add(perDetail);
                    }
                }
                else
                {
                    string idFrm = data.Split('=')[0];
                    var noiDung = data.Split('=')[1];

                    DepartmentReportDetail perDetail = new DepartmentReportDetail();
                    perDetail.department_report_id = pr.department_report_id;
                    perDetail.form_department_report_detail_id = Int32.Parse(idFrm);
                    perDetail.department_report_content = noiDung;
                    db.DepartmentReportDetails.Add(perDetail);
                }

            }
            db.SaveChanges();
            return Content("Success");
        }
    }
}