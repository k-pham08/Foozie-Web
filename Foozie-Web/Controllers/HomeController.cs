using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Foozie_Web.Models;

namespace Foozie_Web.Controllers
{
    public class HomeController : Controller
    {
        private FoozieEntity db = new FoozieEntity();

        public ActionResult RenderBadge()
        {
            List<ORDER_DETAIL> details = db.ORDER_DETAIL.ToList();
            ViewBag.Qty = details.Count;
            return PartialView("RenderBadge");
        }

        public ActionResult Index([Bind(Include = "order_id,date,status,address,user_id")] ORDER oRDER)
        {
            var order = db.ORDERs.Any(o => o.status == "Waiting");
            if (!order && Session["idUser"] != null)
            {
                oRDER.order_id = Guid.NewGuid();
                oRDER.user_id = new Guid(Session["idUser"].ToString());
                oRDER.date = DateTime.Parse("01/01/2000");
                oRDER.status = "Waiting";
                oRDER.total = 0;
                db.ORDERs.Add(oRDER);
                db.SaveChanges();
            }
            return View(db.FOOD_TYPE.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}