﻿using MonthlyStatement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;


namespace MonthlyStatement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        public ActionResult Index()
        {

            return View();
        }
    }
}