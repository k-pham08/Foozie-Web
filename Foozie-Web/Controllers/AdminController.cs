using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using Foozie_Web.Models;

namespace Foozie_Web.Controllers
{
    public class AdminController : Controller
    {

        private FoozieEntity db = new FoozieEntity();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Order()
        {
            var oRDERs = db.ORDERs.Where(o => o.status != "Waiting");
            return View(oRDERs.ToList());
        }
    }
}