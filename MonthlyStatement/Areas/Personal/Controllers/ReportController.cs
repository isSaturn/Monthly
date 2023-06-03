using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Personal.Controllers
{
    [Authorize(Roles = "Giảng viên")]
    public class ReportController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();
        // GET: Personal/Report
        public ActionResult Personal()
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            var per = db.PersonalReports.Where(p => p.account_id == accID).ToList();
            return View(per);
        }
        public ActionResult PersonalDetail(int id)
        {
            int per = db.PersonalReports.Find(id).report_period_id;
            var form = db.FormPersonalReports.FirstOrDefault(f => f.report_period_id == per);
            ViewBag.PerDetail = id;
            return View(form);
        }
        public ActionResult PersonalReportEdit(int id)
        {
            var per = db.PersonalReports.Find(id);
            var form = db.FormPersonalReports.FirstOrDefault(f => f.report_period_id == per.report_period_id);
            var check = db.ReportPeriods.FirstOrDefault(d => d.start_date <= form.ReportPeriod.start_date && d.end_date >= form.ReportPeriod.end_date);
            ViewBag.PeriodsId = check.report_period_id;
            ViewBag.PerDetail = id;
            return View(form);
        }
        [HttpPost]
        public ActionResult PersonalReportEdit(HttpPostedFileBase fileMinhChung, string data, int? reportperiodid)
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            PersonalReport pr = new PersonalReport();
            pr.report_period_id = (int)reportperiodid;
            var pers = db.PersonalReports.FirstOrDefault(r => r.account_id.Equals(accID));
            var periodId = db.PersonalReports.Where(c => c.report_period_id == pr.report_period_id);

            pr.status = DateTime.Now.Day <= 21 ? "Đã báo cáo" : "Trễ báo cáo";
            pr.date_report = DateTime.Now;
            pr.account_id = accID;
            db.PersonalReports.Add(pr);
            db.SaveChanges();

            foreach (var item in periodId)
            {
                db.Comments.Where(c => c.personal_report_id == item.personal_report_id).ToList().ForEach(c => c.personal_report_id = pr.personal_report_id);
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
            db.PersonalReportDetails.RemoveRange(pers.PersonalReportDetails);
            db.PersonalReports.Remove(pers);
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
                            PersonalReportDetail perDetail = new PersonalReportDetail();
                            perDetail.personal_report_id = pr.personal_report_id;
                            perDetail.form_personal_report_detail_id = Int32.Parse(idFrm);
                            perDetail.personal_report_content = items;
                            db.PersonalReportDetails.Add(perDetail);
                        }
                    }
                    else
                    {
                        string idFrm = item.Split('=')[0];
                        var noiDung = item.Split('=')[1];

                        PersonalReportDetail perDetail = new PersonalReportDetail();
                        perDetail.personal_report_id = pr.personal_report_id;
                        perDetail.form_personal_report_detail_id = Int32.Parse(idFrm);
                        perDetail.personal_report_content = noiDung;
                        db.PersonalReportDetails.Add(perDetail);
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
                        PersonalReportDetail perDetail = new PersonalReportDetail();
                        perDetail.personal_report_id = pr.personal_report_id;
                        perDetail.form_personal_report_detail_id = Int32.Parse(idFrm);
                        perDetail.personal_report_content = items;
                        db.PersonalReportDetails.Add(perDetail);
                    }
                }
                else
                {
                    string idFrm = data.Split('=')[0];
                    var noiDung = data.Split('=')[1];

                    PersonalReportDetail perDetail = new PersonalReportDetail();
                    perDetail.personal_report_id = pr.personal_report_id;
                    perDetail.form_personal_report_detail_id = Int32.Parse(idFrm);
                    perDetail.personal_report_content = noiDung;
                    db.PersonalReportDetails.Add(perDetail);
                }

            }
            db.SaveChanges();
            return Content("Success");
        }
    }
}