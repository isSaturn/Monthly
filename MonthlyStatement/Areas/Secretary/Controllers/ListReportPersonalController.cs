﻿using MonthlyStatement.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonthlyStatement.Areas.Secretary.Controllers
{
    [Authorize(Roles = "Thư ký")]

    public class ListReportPersonalController : Controller
    {
        CP25Team04Entities db = new CP25Team04Entities();

        // GET: Secretaryy/ListReportPersonal
        public ActionResult Index()
        {
            return View(db.PersonalReports.ToList());
        }
        public ActionResult Detail(int id)
        {
            var form = db.PersonalReports.Find(id);
            return View(form);
        }
       

    }
}