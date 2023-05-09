using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MonthlyStatement.Models;

namespace MonthlyStatement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Quản trị viên")]

    public class CategoriesController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        // GET: Admin/Categories
        public ActionResult Index()
        {
            var categories = db.Categories.Include(c => c.Categories1);
            return View(categories.ToList());
        }

        // GET: Admin/Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        [HttpGet]

        // GET: Admin/Categories/Create
        public ActionResult Create()
        {
            Session["category-lv1"] = db.Categories.Where(c => c.category_lv == 1).ToList();
            Session["category-lv2"] = db.Categories.Where(c => c.category_lv == 2).ToList();


            return View(new Category());
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(string content)
        {
            if (content.Split('-').Count() == 1)
            {
                Category category = new Category();
                category.category_lv = 1;
                category.category_content = content;
                db.Categories.Add(category);
                db.SaveChanges();

            }
            else if (content.Split('-').Count() == 2)
            {
                Category category = new Category();
                category.category_lv = 2;
                category.category_of_id = Convert.ToInt32(content.Split('-')[0]);
                category.category_content = content.Split('-')[1];
                db.Categories.Add(category);
                db.SaveChanges();
                Category category_lv3 = new Category();
                category_lv3.category_lv = 3;
                category_lv3.category_of_id = category.category_id;
                category_lv3.category_content = content.Split('-')[1];
                db.Categories.Add(category_lv3);
                db.SaveChanges();
            }
            else if (content.Split('-').Count() == 3)
            {
                Category category = new Category();
                category.category_lv = 3;
                category.category_of_id = Convert.ToInt32(content.Split('-')[1]);
                category.category_content = content.Split('-')[2];
                db.Categories.Add(category);
                db.SaveChanges();
            }
            else
            {
                return Content("error");
            }
            return Content("success");
        }

        [HttpGet]
        // GET: Admin/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.category_of_id = new SelectList(db.Categories.Where(c => c.category_of_id == null).ToList(), "category_id", "category_content", category.category_of_id);
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "category_id,category_of_id,category_content")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.category_of_id = new SelectList(db.Categories, "category_id", "category_content", category.category_of_id);
            return View(category);
        }
        [HttpPost]
        public ActionResult EditCategory(int id, string name)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return Content("Không tìm thấy danh mục");
            }
            else
            {
                category.category_content = name;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return Content("Success");
            }
        }

        [HttpGet]
        // GET: Admin/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
