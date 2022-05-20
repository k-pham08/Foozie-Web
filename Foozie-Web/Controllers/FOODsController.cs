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
    public class FOODsController : Controller
    {
        private FoozieEntity db = new FoozieEntity();

        

        // GET: FOODs
        public ActionResult Index()
        {
            var fOODs = db.FOODs.Include(f => f.FOOD_TYPE);
            return View(fOODs.ToList());
        }

        // GET: FOODs/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FOOD fOOD = db.FOODs.Find(id);
            if (fOOD == null)
            {
                return HttpNotFound();
            }
            return View(fOOD);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(Guid id, string note, int quantity, ORDER_DETAIL oRDER_DETAIL, [Bind(Include = "order_id,date,status,address,user_id")] ORDER oRDER)
        {
            if (ModelState.IsValid)
            {
                Guid cur_user = new Guid(Session["idUser"].ToString());
                var data = db.ORDERs.Where(o => o.user_id.Equals(cur_user) && o.status == "Waiting");
                var order = db.ORDERs.Any(o => o.status == "Waiting");
                var orderDetail = db.ORDER_DETAIL.FirstOrDefault(model => model.food_id == id);
                if (!order)
                {
                    oRDER.order_id = Guid.NewGuid();
                    oRDER.user_id = new Guid(Session["idUser"].ToString());
                    oRDER.date = DateTime.Now;
                    oRDER.status = "Waiting";
                    oRDER.total = 0;
                    db.ORDERs.Add(oRDER);
                    db.SaveChanges();
                }
                if (orderDetail != null)
                {
                    orderDetail.quantity+= quantity;
                } else
                {
                    oRDER_DETAIL.order_id = data.First().order_id;
                    oRDER_DETAIL.food_id = id;
                    oRDER_DETAIL.quantity = quantity;
                    oRDER_DETAIL.note = note;
                    db.ORDER_DETAIL.Add(oRDER_DETAIL);
                }
                db.SaveChanges();
                ViewBag.success = "Thêm thành công";
                return RedirectToAction("Details", "Order");
            }

            ViewBag.food_id = new SelectList(db.FOODs, "food_id", "name", oRDER_DETAIL.food_id);
            ViewBag.order_id = new SelectList(db.ORDERs, "order_id", "status", oRDER_DETAIL.order_id);
            return RedirectToAction("Index", "Home");
        }

        // GET: FOODs/Create
        public ActionResult Create()
        {
            ViewBag.type_id = new SelectList(db.FOOD_TYPE, "type_id", "name");
            return View();
        }

        // POST: FOODs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "food_id,name,description,thumbnail,price,is_delete,type_id")] FOOD fOOD)
        {
            if (ModelState.IsValid)
            {
                fOOD.food_id = Guid.NewGuid();
                db.FOODs.Add(fOOD);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.type_id = new SelectList(db.FOOD_TYPE, "type_id", "name", fOOD.type_id);
            return View(fOOD);
        }

        // GET: FOODs/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FOOD fOOD = db.FOODs.Find(id);
            if (fOOD == null)
            {
                return HttpNotFound();
            }
            ViewBag.type_id = new SelectList(db.FOOD_TYPE, "type_id", "name", fOOD.type_id);
            return View(fOOD);
        }

        // POST: FOODs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "food_id,name,description,thumbnail,price,is_delete,type_id")] FOOD fOOD)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fOOD).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.type_id = new SelectList(db.FOOD_TYPE, "type_id", "name", fOOD.type_id);
            return View(fOOD);
        }

        // GET: FOODs/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FOOD fOOD = db.FOODs.Find(id);
            if (fOOD == null)
            {
                return HttpNotFound();
            }
            return View(fOOD);
        }

        // POST: FOODs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            FOOD fOOD = db.FOODs.Find(id);
            db.FOODs.Remove(fOOD);
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
