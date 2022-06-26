using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Foozie_Web.Models;

namespace Foozie_Web.Controllers
{
    public class VoucherController : Controller
    {
        private FoozieEntity db = new FoozieEntity();

        // GET: Voucher
        public ActionResult Index()
        {
            var vouchers = db.VOUCHERs.Where(v => v.used < v.max_used && v.expired >= DateTime.Now).ToList();
            return View(vouchers);
        }
    }
}
