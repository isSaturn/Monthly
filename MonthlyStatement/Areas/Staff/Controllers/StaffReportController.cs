﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Staff.Controllers
{
    [Authorize(Roles = "Nhân viên")]
    public class StaffReportController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();
        // GET: Staff/Report
        public ActionResult Staff()
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            var per = db.StaffReports.Where(p => p.account_id == accID).ToList();
            return View(per);
        }
        public ActionResult StaffDetail(int id)
        {
            int per = db.StaffReports.Find(id).report_period_id;
            var form = db.FormStaffReports.FirstOrDefault(f => f.report_period_id == per);
            ViewBag.StaDetail = id;
            ViewBag.StaffReport = db.StaffReports.FirstOrDefault(f => f.staff_report_id == id);

            return View(form);
        }
        public FileResult DownLoad(string file_path)
        {
            //string path = Server.MapPath(file_path);
            //string filename = Path.GetFileName("swocjt297owtotjy8orluwoiteo1efImport.xlsx");
            //string fullPath = Path.Combine(path, filename);
            string ext = Path.GetExtension(file_path);
            string filename = "MyFile" + ext; // Make this dynamic from the actual file
            byte[] filedata = System.IO.File.ReadAllBytes(file_path);
            string contentType = MimeMapping.GetMimeMapping(file_path);

            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true
            };
            Response.AppendHeader("Content-Disposition", contentDisposition.ToString());

            return File(filedata, contentType);
        }
        public ActionResult StaffReportEdit(int id)
        {
            int per = db.StaffReports.Find(id).report_period_id;
            var form = db.FormStaffReports.FirstOrDefault(f => f.report_period_id == per);
            var check = db.ReportPeriods.FirstOrDefault(d => d.start_date <= form.ReportPeriod.start_date && d.end_date >= form.ReportPeriod.end_date);
            ViewBag.PeriodsId = check.report_period_id;
            ViewBag.StaDetail = id;

            return View(form);
        }
        [HttpPost]
        public ActionResult StaffReportEdit(HttpPostedFileBase fileMinhChung, string data, int? reportperiodid)
        {
            string emails = User.Identity.Name;
            string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            Claim claim = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role);
            string roleName = (claim != null) ? claim.Value : string.Empty;
            StaffReport pr = new StaffReport();
            pr.report_period_id = (int)reportperiodid;
            var pers = db.StaffReports.FirstOrDefault(r => r.account_id.Equals(accID));
            var periodId = db.StaffReports.Where(c => c.report_period_id == pr.report_period_id);
            var crProfile = db.Profiles.FirstOrDefault(n => n.account_id == accID);

            pr.status = DateTime.Now.Day <= 21 ? "Đã báo cáo" : "Trễ báo cáo";
            pr.date_report = DateTime.Now;
            pr.account_id = accID;
            pr.reporter = crProfile.user_name;
            pr.role_user = roleName;
            pr.user_code = crProfile.user_code;
            pr.user_department = crProfile.DepartmentList.department_name;
            pr.user_faculty = crProfile.Faculty.faculty_name;
            db.StaffReports.Add(pr);
            db.SaveChanges();

            foreach (var item in periodId)
            {
                db.Comments.Where(c => c.staff_report_id == item.staff_report_id).ToList().ForEach(c => c.staff_report_id = pr.staff_report_id);
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
            db.StaffReportDetails.RemoveRange(pers.StaffReportDetails);
            db.StaffReports.Remove(pers);
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
                            StaffReportDetail perDetail = new StaffReportDetail();
                            perDetail.staff_report_id = pr.staff_report_id;
                            perDetail.form_staff_report_detail_id = Int32.Parse(idFrm);
                            perDetail.staff_report_content = items;
                            db.StaffReportDetails.Add(perDetail);
                        }
                    }
                    else
                    {
                        string idFrm = item.Split('=')[0];
                        var noiDung = item.Split('=')[1];

                        StaffReportDetail perDetail = new StaffReportDetail();
                        perDetail.staff_report_id = pr.staff_report_id;
                        perDetail.form_staff_report_detail_id = Int32.Parse(idFrm);
                        perDetail.staff_report_content = noiDung;
                        db.StaffReportDetails.Add(perDetail);
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
                        StaffReportDetail perDetail = new StaffReportDetail();
                        perDetail.staff_report_id = pr.staff_report_id;
                        perDetail.form_staff_report_detail_id = Int32.Parse(idFrm);
                        perDetail.staff_report_content = items;
                        db.StaffReportDetails.Add(perDetail);
                    }
                }
                else
                {
                    string idFrm = data.Split('=')[0];
                    var noiDung = data.Split('=')[1];

                    StaffReportDetail perDetail = new StaffReportDetail();
                    perDetail.staff_report_id = pr.staff_report_id;
                    perDetail.form_staff_report_detail_id = Int32.Parse(idFrm);
                    perDetail.staff_report_content = noiDung;
                    db.StaffReportDetails.Add(perDetail);
                }

            }
            db.SaveChanges();
            return Content("Success");
        }
    }
}