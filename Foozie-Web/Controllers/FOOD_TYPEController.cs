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
    public class FOOD_TYPEController : Controller
    {
        private FoozieEntity db = new FoozieEntity();

        // GET: FOOD_TYPE
        public ActionResult Index()
        {
            return View(db.FOOD_TYPE.ToList());
        }

        // GET: FOOD_TYPE/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FOOD_TYPE fOOD_TYPE = db.FOOD_TYPE.Find(id);
            List<FOOD> foodList = db.FOODs.Where(f => f.type_id.CompareTo(fOOD_TYPE.type_id) == 0).ToList();
            
            if (fOOD_TYPE == null)
            {
                return HttpNotFound();
            }
            return View(foodList);
        }

        // GET: FOOD_TYPE/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FOOD_TYPE/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "type_id,name,code,description,is_delete")] FOOD_TYPE fOOD_TYPE)
        {
            if (ModelState.IsValid)
            {
                fOOD_TYPE.type_id = Guid.NewGuid();
                db.FOOD_TYPE.Add(fOOD_TYPE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fOOD_TYPE);
        }

        // GET: FOOD_TYPE/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FOOD_TYPE fOOD_TYPE = db.FOOD_TYPE.Find(id);
            if (fOOD_TYPE == null)
            {
                return HttpNotFound();
            }
            return View(fOOD_TYPE);
        }

        // POST: FOOD_TYPE/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "type_id,name,code,description,is_delete")] FOOD_TYPE fOOD_TYPE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fOOD_TYPE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fOOD_TYPE);
        }

        // GET: FOOD_TYPE/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FOOD_TYPE fOOD_TYPE = db.FOOD_TYPE.Find(id);
            if (fOOD_TYPE == null)
            {
                return HttpNotFound();
            }
            return View(fOOD_TYPE);
        }

        // POST: FOOD_TYPE/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            FOOD_TYPE fOOD_TYPE = db.FOOD_TYPE.Find(id);
            db.FOOD_TYPE.Remove(fOOD_TYPE);
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
