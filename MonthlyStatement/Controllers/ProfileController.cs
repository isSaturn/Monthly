﻿using Microsoft.AspNet.Identity;
using MonthlyStatement.Middleware;
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
        public ActionResult Edit_Profile(string name, HttpPostedFileBase avt)
        {
            string ID_User = User.Identity.Name;
            var user = db.AspNetUsers.FirstOrDefault(u => u.Email.Equals(ID_User));
            var profile = db.Profiles.FirstOrDefault(p => p.account_id == user.Id);
            if (!string.IsNullOrWhiteSpace(name))
            {
                profile.user_name = name;
            }

            string oldfilePath = profile.avatar;
            if (avt != null && avt.ContentLength > 0)
            {
                string time = DateTime.Now.ToString("yymmssfff");
                var fileName = System.IO.Path.GetFileName(avt.FileName);
                string filePath = "~/assets/user/avatar/" + time + fileName;
                avt.SaveAs(Server.MapPath(filePath));
                profile.avatar = "~/assets/user/avatar/" + time + avt.FileName;
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
            return RedirectToAction("Index", "Profile");
        }
    }
}