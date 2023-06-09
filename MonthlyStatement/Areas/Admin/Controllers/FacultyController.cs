using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Quản trị viên,Thư ký,Ban phòng khoa")]

    public class FacultyController : Controller
    {
        private ApplicationAccountManager _userManager;
        CP25Team04Entities db = new CP25Team04Entities();

        public FacultyController()
        {
        }
        public FacultyController(ApplicationAccountManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationAccountManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationAccountManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult AddMember()
        {
            return View();
        }

        // GET: Admin/Faculty
        public ActionResult Index()
        {
           /* string manguoidung = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault().Id;
            int makhoa = (int)db.Profiles.Where(p => p.account_id == manguoidung).FirstOrDefault().faculty_id;
            var data = db.Faculties.Where(y => y.faculty_id == makhoa).ToList();*/

            return View(db.Faculties.ToList());
        }

        // GET: Admin/Faculty/Create
        public ActionResult Create()
        {
            return View(new Faculty());
        }

        // POST: Admin/Faculty/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "faculty_id,faculty_name")] Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                db.Faculties.Add(faculty);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(faculty);
        }

        // GET: Admin/Faculty/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faculty faculty = db.Faculties.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }
            return View(faculty);
        }

        // POST: Admin/Faculty/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "faculty_id,faculty_name")] Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                db.Entry(faculty).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(faculty);
        }

        [HttpGet]
        public ActionResult AttendenceList(int id)
        {
            Session["add-user-faculty"] = db.Profiles.Where(p => p.faculty_id == null).ToList();
            var reportPeriods = db.Faculties.Find(id);
            return View(reportPeriods);
        }
        [HttpPost]
        public ActionResult AddUser(int id, int khoa)
        {
            var user = db.Profiles.Find(id);
            user.faculty_id = khoa;

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return Content("Success");
        }
        [HttpPost]
        public ActionResult ImportUser(HttpPostedFileBase lstUser)
        {
            string filePath = string.Empty;
            if (lstUser != null)
            {
                string path = Server.MapPath("~/assets/FileImport/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filePath = path + Path.GetFileName(lstUser.FileName);
                string extension = Path.GetExtension(lstUser.FileName);
                lstUser.SaveAs(filePath);

                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls":
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx":
                        conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                        break;
                }
                DataTable dtExcel = new DataTable();
                //đọc dữ liệu từ excel
                try
                {
                    conString = string.Format(conString, filePath);
                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;
                                connExcel.Open();
                                DataTable dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[1].Field<string>("TABLE_NAME");
                                connExcel.Close();

                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * from [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dtExcel);
                                connExcel.Close();
                            }
                        }
                    }
                }
                catch
                {
                    return Content("INCORRECT");
                }

                DataRow dr = dtExcel.Rows[0];
                dr.Delete();
                dtExcel.AcceptChanges();

                if (dtExcel.Rows.Count != 6)
                {
                    foreach (DataRow data in dtExcel.Rows)
                    {
                        string error = "";
                        int i = 1;
                        // 1 Validation Email
                        if (data[2].ToString().Trim().Length < 1)
                        {
                            error += i + ". Không tìm thấy Email#";
                            i++;
                        }
                        else if ((data[2].ToString().Trim().IndexOf("@vlu.edu.vn") == -1 && data[2].ToString().Trim().IndexOf("@vanlanguni.vn") == -1))
                        {
                            error += i + ". Email không đúng định dạng#";
                            i++;
                        }
                        else
                        {
                            bool checkEmail;
                            try
                            {
                                MailAddress m = new MailAddress(data[2].ToString().Trim());
                                checkEmail = true;
                            }
                            catch (FormatException)
                            {
                                checkEmail = false;
                            }

                            if (checkEmail == false)
                            {
                                error += i + ". Địa chỉ Email chưa đúng định dạng.#";
                                i++;
                            }
                        }
                        // 2 Validation Khoa
                        string khoas = data[3].ToString().Trim();
                        var khoa = db.Faculties.FirstOrDefault(f => f.faculty_name.ToLower().Equals(khoas.ToLower()));
                        if (khoa == null)
                        {
                            error += i + ". Không tìm thấy khoa.#";
                            i++;
                        }

                        string bomons = data[4].ToString().Trim();
                        var bomon = db.DepartmentLists.FirstOrDefault(f => f.department_name.ToLower().Equals(bomons.ToLower()));
                        if (bomon == null)
                        {
                            error += i + ". Không tìm thấy bộ môn.#";
                            i++;
                        }

                        // 3 Validation Chức danh
                        string roles = data[5].ToString().Trim();
                        var role = db.AspNetRoles.FirstOrDefault(r => r.Name.ToLower().Equals(roles.ToLower()));
                        if (role == null)
                        {
                            error += i + ". Không tìm thấy vai trò.#";
                            i++;
                        }
                        //check email đã có hay chưa
                        if (string.IsNullOrEmpty(error))
                        {
                            string emails = data[2].ToString().Trim();
                            var profiles = db.Profiles.FirstOrDefault(a => a.email.ToLower().Equals(emails.ToLower().Trim()));
                            if (profiles == null)
                            {
                                var pro = new Profile();
                                pro.user_name = data[1].ToString().Trim();
                                pro.email = data[2].ToString().Trim();
                                pro.faculty_id = khoa.faculty_id;
                                pro.department_id = bomon.department_id;
                                db.Profiles.Add(pro);
                                db.SaveChanges();

                            }
                            else
                            {
                                profiles.user_name = data[1].ToString().Trim();
                                profiles.email = data[2].ToString().Trim();
                                profiles.faculty_id = khoa.faculty_id;
                                profiles.department_id = bomon.department_id;
                                db.Entry(profiles).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            var aspNetUser = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim()));
                            if (aspNetUser != null)
                            {
                                //thay đổi role
                                var oldUser = UserManager.FindById(aspNetUser.Id);
                                var oldRole = UserManager.GetRoles(oldUser.Id).FirstOrDefault();

                                // Update user role
                                UserManager.RemoveFromRole(aspNetUser.Id, oldRole);
                                role.AspNetUsers.Add(aspNetUser);
                                db.SaveChanges();

                                var profile = aspNetUser.Profiles.FirstOrDefault();

                                //thay đổi khoa
                                profile.faculty_id = khoa.faculty_id;

                                //thay đổi bomon 
                                profile.department_id = bomon.department_id;

                                //thay đổi tên
                                string names = data[1].ToString().Trim();
                                profile.user_name = names;

                                db.Entry(profile).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return Content("INCORRECT");
            }
            return Content("DanhSach");
        }

        public ActionResult DeleteFaculty(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var prof = db.Profiles.FirstOrDefault(t => t.account_id.Equals(id));
                prof.faculty_id = null;
                db.Entry(prof).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}
