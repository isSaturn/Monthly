using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    [Authorize]

    public class ProfileController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();
        // GET: Admin/MyProfile
        public ActionResult Index()
        {
            var profile = db.AspNetUsers.Find(Session["ID_User"]);
            return View(profile);
        }
        public ActionResult Edit_Profile(string name, string department, string majors, HttpPostedFileBase avt)
        {
            string ID_User = Session["ID_User"].ToString();
            Profile profile = db.Profiles.Find(ID_User);
            if (!string.IsNullOrWhiteSpace(name))
            {
                profile.user_name = name;
            }
            if (!string.IsNullOrWhiteSpace(department))
            {
                profile.department = department;
            }
            if (!string.IsNullOrWhiteSpace(majors))
            {
                profile.majors = majors;
            }


            string oldfilePath = profile.avatar;
            if (avt != null && avt.ContentLength > 0)
            {
                string time = DateTime.Now.ToString("yymmssfff");
                var fileName = System.IO.Path.GetFileName(avt.FileName);
                string filePath = "~/assets/user/avatar/" + time + fileName;
                avt.SaveAs(Server.MapPath(filePath));
                profile.avatar = "~/Template/app-assets/img/Avatar/" + time + avt.FileName;
                string fullPath = Request.MapPath(oldfilePath);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            else
            {
                if (Session["Avt"] != null)
                {
                    profile.avatar = Session["Avt"].ToString();
                }
                else
                {
                    profile.avatar = null;
                }

            }
            db.Entry(profile).State = EntityState.Modified;
            db.SaveChanges();

            Session["Avt"] = profile.avatar;
            Session["notification"] = "Successfully Edited Profile";
            return RedirectToAction("MyProfile");

        }
    }
}