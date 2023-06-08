using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Quản trị viên,Ban phòng khoa,Bộ môn,Thư ký")]

    public class ListReportPersonalController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();

        // GET: Admin/ListReportPersonal
        public ActionResult Index()
        {
            return View(db.PersonalReports.ToList());
        }
        public ActionResult Detail(int id)
        {
            var per = db.PersonalReports.Find(id);
            var form = db.FormPersonalReports.FirstOrDefault(f => f.report_period_id == per.report_period_id);
            ViewBag.accID = per.account_id;
            ViewBag.PerID = id;
            ViewBag.personalReport = db.PersonalReports.FirstOrDefault(f => f.personal_report_id == id);

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
    }
}