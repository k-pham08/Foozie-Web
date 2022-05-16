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
    public class ORDERController : Controller
    {
        private FoozieEntity db = new FoozieEntity();

        // GET: ORDER
        public ActionResult Index()
        {
            var oRDERs = db.ORDERs.Include(o => o.USER);
            return View(oRDERs.ToList());
        }

        // GET: ORDER/Details/5
        public ActionResult Details()
        {
            Guid cur_user = new Guid(Session["idUser"].ToString());
            var data = db.ORDERs.Where(o => o.user_id.Equals(cur_user) && o.status == "Waiting");
            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ORDER oRDER = data.FirstOrDefault();
            if (oRDER == null)
            {
                return HttpNotFound();
            }
            return View(oRDER);
        }

        // GET: ORDER/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.USERs, "user_id", "type");
            return View();
        }

        // POST: ORDER/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "order_id,date,status,address,user_id")] ORDER oRDER)
        {
            if (ModelState.IsValid)
            {
                oRDER.order_id = Guid.NewGuid();
                db.ORDERs.Add(oRDER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(db.USERs, "user_id", "type", oRDER.user_id);
            return View(oRDER);
        }

        // GET: ORDER/Edit/5
        public ActionResult Edit(Guid? order_id)
        {
            if (order_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ORDER oRDER = db.ORDERs.Find(order_id);
            if (oRDER == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.USERs, "user_id", "type", oRDER.user_id);
            return View(oRDER);
        }

        // POST: ORDER/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "order_id,date,status,address,total,user_id")] ORDER oRDER)
        {
            if (ModelState.IsValid)
            {      
                db.Entry(oRDER).State = EntityState.Modified;
                oRDER.status = "Payment";
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.user_id = new SelectList(db.USERs, "user_id", "type", oRDER.user_id);
            return View(oRDER);
        }

        // GET: ORDER/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ORDER oRDER = db.ORDERs.Find(id);
            if (oRDER == null)
            {
                return HttpNotFound();
            }
            return View(oRDER);
        }

        // POST: ORDER/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ORDER oRDER = db.ORDERs.Find(id);
            db.ORDERs.Remove(oRDER);
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
