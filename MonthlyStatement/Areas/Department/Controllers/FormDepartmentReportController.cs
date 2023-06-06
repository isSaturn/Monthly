using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Department.Controllers
{
    [Authorize(Roles = "Bộ môn")]
    public class FormDepartmentReportController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Department/Report
        public ActionResult Index()
        {
            {
                var current_time = DateTime.Now;
                var check = db.ReportPeriods.FirstOrDefault(d => d.start_date <= current_time && d.end_date >= current_time);
                string emails = User.Identity.Name;
                string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
                var lstCatDate = db.FormPersonalReports.FirstOrDefault(c => c.ReportPeriod.start_date <= current_time && c.ReportPeriod.end_date >= current_time).form_personal_report_id;
                var check_Faculty = db.Profiles.FirstOrDefault(x => x.account_id == accID);
                Session["faculty"] = check_Faculty.faculty_id;

                if (!db.FormDepartmentReportDetails.Any(f => f.FormDepartmentReport.report_period_id == check.report_period_id))
                {
                    ViewBag.CheckFormDep = true;
                }
                else if (db.DepartmentReports.Any(p => p.ReportPeriod.start_date <= current_time && p.ReportPeriod.end_date >= current_time && p.account_id == accID))
                {
                    ViewBag.CheckReportDep = true;
                }
                //noi dung danh muc cua thang
                if (db.FormPersonalReportDetails.Any(n => n.form_personal_report_id == lstCatDate))
                {
                    var formDetail = db.FormPersonalReportDetails.Where(form => form.form_personal_report_id == lstCatDate);
                    List<PersonalReportDetail> contentPer = new List<PersonalReportDetail>();
                    foreach (var item in formDetail)
                    {
                        var perDetail = db.PersonalReportDetails.Where(per => per.form_personal_report_detail_id == item.form_personal_report_detail_id);
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
                return View(check.FormDepartmentReports.First());
            }
        }
        [HttpPost]
        public ActionResult submitFormReport(HttpPostedFileBase fileMinhChung, string data, int? reportperiodid)
        {
            try
            {
                string emails = User.Identity.Name;
                string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
                Claim claim = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role);
                string roleName = (claim != null) ? claim.Value : string.Empty;
                var pers = db.PersonalReports.FirstOrDefault(r => r.account_id.Equals(accID));
                DepartmentReport pr = new DepartmentReport();
                var periodId = db.PersonalReports.Where(c => c.report_period_id == pr.report_period_id);
                var crProfile = db.Profiles.FirstOrDefault(n => n.account_id == accID);

                pr.report_period_id = (int)reportperiodid;
                pr.status = DateTime.Now.Day <= 21 ? "Đã báo cáo" : "Trễ báo cáo";
                pr.date_report = DateTime.Now;
                pr.status_secretary = "Chưa duyệt";
                pr.status_faculty = "Chưa duyệt";
                pr.account_id = accID;
                pr.reporter = crProfile.user_name;
                pr.role_user = roleName;
                pr.user_code = crProfile.user_code;
                pr.user_department = crProfile.DepartmentList.department_name;
                pr.user_faculty = crProfile.Faculty.faculty_name;
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
                db.DepartmentReports.Add(pr);
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
            catch
            {
                return Content("Error");
            }

        }
    }
}