﻿using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Staff.Controllers
{
    [Authorize(Roles = "Nhân viên")]
    public class FormStaffReportController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Staff/Report
        public ActionResult Index()
        {
            {
                var current_time = DateTime.Now;
                var check = db.ReportPeriods.FirstOrDefault(d => d.start_date <= current_time && d.end_date >= current_time);
                string emails = User.Identity.Name;
                string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
                if (!db.FormStaffReportDetails.Any(f => f.FormStaffReport.report_period_id == check.report_period_id))
                {
                    ViewBag.CheckFormSta = true;
                }
                else if (!db.StaffReports.Any(p => p.ReportPeriod != null && p.account_id != accID))
                {
                    ViewBag.Check = true;
                }

                ViewBag.PeriodsId = check.report_period_id;
                return View(check.FormStaffReports.First());
            }
        }
        [HttpPost]
        public ActionResult submitFormReport(HttpPostedFileBase fileMinhChung, string data, int? reportperiodid)
        {
            try
            {
                string emails = User.Identity.Name;
                string accID = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
                StaffReport pr = new StaffReport();
                pr.report_period_id = (int)reportperiodid;

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

                pr.status = DateTime.Now.Day <= 21 ? "Đã báo cáo" : "Trễ báo cáo";
                pr.date_report = DateTime.Now;
                pr.account_id = accID;
                db.StaffReports.Add(pr);
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
            catch
            {
                return Content("Error");
            }

        }

        public ActionResult ListReportYear()
        {
            var current_year = DateTime.Now.Year;
            int check = db.ReportYears.Where(y => y.year == current_year).Count();

            if (check < 1)
            {
                //cho phep thêm mới năm báo cáo
                Session["ReportYear-Check"] = true;
            }

            else
                //khong cho phep them moi
                Session["ReportYear-Check"] = false;
            return View(db.ReportYears.OrderByDescending(y => y.year).ToList());
        }

        public ActionResult ListReportPeriod(int id)
        {
            var reportPeriods = db.ReportPeriods.Where(r => r.report_year_id == id).ToList();
            return View(reportPeriods);
        }
    }
}