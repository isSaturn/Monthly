using Microsoft.AspNet.Identity;
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

            string ID_User = User.Identity.Name;
            var profiles = db.AspNetUsers.FirstOrDefault(u => u.Email.Equals(ID_User)).Profiles.First();
            if (profiles != null)
            {
                return View(profiles);
            }
            return View(HttpNotFound());

        }
        public ActionResult Edit_Profile(string bomon, string name )
        {
            string ID_User = User.Identity.Name;
            var user = db.AspNetUsers.FirstOrDefault(u => u.Email.Equals(ID_User));
            var profile = db.Profiles.FirstOrDefault(p => p.account_id == user.Id);
            if (!string.IsNullOrWhiteSpace(name))
            {
                profile.user_name = name;
            }
            if (!string.IsNullOrWhiteSpace(bomon))
            {
                profile.department = bomon;
            }

            db.Entry(profile).State = EntityState.Modified;
            db.SaveChanges();

            Session["notification"] = "Successfully Edited Profile";
            return RedirectToAction("Index", "Profile");
        }
    }
}