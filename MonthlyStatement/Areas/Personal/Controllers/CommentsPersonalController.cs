using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Personal.Controllers
{
    public class CommentsPersonalController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        [HttpPost]
        // GET: Personal/CommentsPersonal
        public ActionResult PostComment(string CommentText)
        {
            string emails = User.Identity.Name;
            string userId = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(emails.ToLower().Trim())).Id;
            Comment c = new Comment();
            c.comment_content = CommentText;
            c.comment_date = DateTime.Now;
            c.account_id = userId;
            db.Comments.Add(c);
            db.SaveChanges();
            return RedirectToAction("Personal", "Report");
        }
        
    }
}