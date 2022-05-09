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
    public class ORDER_DETAILController : Controller
    {
        private FoozieEntity db = new FoozieEntity();

        // GET: ORDER_DETAIL
        public ActionResult Index()
        {
            var oRDER_DETAIL = db.ORDER_DETAIL.Include(o => o.FOOD).Include(o => o.ORDER);
            return View(oRDER_DETAIL.ToList());
        }

        // GET: ORDER_DETAIL/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ORDER_DETAIL oRDER_DETAIL = db.ORDER_DETAIL.Find(id);
            if (oRDER_DETAIL == null)
            {
                return HttpNotFound();
            }
            return View(oRDER_DETAIL);
        }

        // GET: ORDER_DETAIL/Create
        public ActionResult Create()
        {
            ViewBag.food_id = new SelectList(db.FOODs, "food_id", "name");
            ViewBag.order_id = new SelectList(db.ORDERs, "order_id", "status");
            return View();
        }

        // POST: ORDER_DETAIL/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "order_id,food_id,quantity,note")] ORDER_DETAIL oRDER_DETAIL)
        {
            if (ModelState.IsValid)
            {
                oRDER_DETAIL.order_id = Guid.NewGuid();
                db.ORDER_DETAIL.Add(oRDER_DETAIL);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.food_id = new SelectList(db.FOODs, "food_id", "name", oRDER_DETAIL.food_id);
            ViewBag.order_id = new SelectList(db.ORDERs, "order_id", "status", oRDER_DETAIL.order_id);
            return View(oRDER_DETAIL);
        }

        // GET: ORDER_DETAIL/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ORDER_DETAIL oRDER_DETAIL = db.ORDER_DETAIL.Find(id);
            if (oRDER_DETAIL == null)
            {
                return HttpNotFound();
            }
            ViewBag.food_id = new SelectList(db.FOODs, "food_id", "name", oRDER_DETAIL.food_id);
            ViewBag.order_id = new SelectList(db.ORDERs, "order_id", "status", oRDER_DETAIL.order_id);
            return View(oRDER_DETAIL);
        }

        // POST: ORDER_DETAIL/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "order_id,food_id,quantity,note")] ORDER_DETAIL oRDER_DETAIL)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oRDER_DETAIL).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.food_id = new SelectList(db.FOODs, "food_id", "name", oRDER_DETAIL.food_id);
            ViewBag.order_id = new SelectList(db.ORDERs, "order_id", "status", oRDER_DETAIL.order_id);
            return View(oRDER_DETAIL);
        }

        // GET: ORDER_DETAIL/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ORDER_DETAIL oRDER_DETAIL = db.ORDER_DETAIL.Find(id);
            if (oRDER_DETAIL == null)
            {
                return HttpNotFound();
            }
            return View(oRDER_DETAIL);
        }

        // POST: ORDER_DETAIL/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ORDER_DETAIL oRDER_DETAIL = db.ORDER_DETAIL.Find(id);
            db.ORDER_DETAIL.Remove(oRDER_DETAIL);
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
