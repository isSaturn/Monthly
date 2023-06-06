using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
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
        public ActionResult Index()
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            var check_Faculty = db.Profiles.FirstOrDefault(x => x.account_id == accID);
            var data = db.Faculties.FirstOrDefault(y => y.faculty_id == check_Faculty.faculty_id);
            var check_Dep = db.Departments.Where(dep => dep.faculty_id == data.faculty_id);
            return View(check_Dep);
        }
        public ActionResult Department()
        {
            //Session["add-user-dep"] = db.Profiles.Where(p => p.department_id == null).ToList();
            //var dep = db.Departments.Find(id);
            //return View(dep);
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
            ViewBag.departmentReport = db.DepartmentReports.FirstOrDefault(f => f.department_report_id == id);

            return View(form);
        }
        public FileResult DownLoad(string file_path)
        {
            //string path = Server.MapPath(file_path);
            //string filename = Path.GetFileName("swocjt297owtotjy8orluwoiteo1efImport.xlsx");
            //string fullPath = Path.Combine(path, filename);
            return File(file_path, "application/vnd.ms-excel", "File.xlsx");
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
            ViewBag.DepDetail = id;
            return View(form);

        }
        [HttpPost]
        public ActionResult DepartmentReportEdit(HttpPostedFileBase fileMinhChung, string data, int? reportperiodid)
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            Claim claim = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role);
            string roleName = (claim != null) ? claim.Value : string.Empty;
            DepartmentReport pr = new DepartmentReport();
            pr.report_period_id = (int)reportperiodid;
            var pers = db.DepartmentReports.FirstOrDefault(r => r.account_id.Equals(accID));
            var periodId = db.DepartmentReports.Where(c => c.report_period_id == pr.report_period_id);
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
            pr.user_department = crProfile.Department.department_name;
            pr.user_faculty = crProfile.Faculty.faculty_name;
            db.DepartmentReports.Add(pr);
            db.SaveChanges();

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