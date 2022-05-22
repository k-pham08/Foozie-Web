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
            var oRDERs = db.ORDERs.Where(o => o.status != "Waiting");
            return View(oRDERs.ToList());
        }

        public ActionResult Order()
        {
            var orders = db.ORDERs.Where(o => o.status != "Waiting");
            return View(orders.ToList());
        }

        public ActionResult Product()
        {
            var foods = db.FOODs.Include(f => f.FOOD_TYPE).ToList();
            return View(foods);
        }

        public ActionResult AddProduct()
        {
            ViewBag.type_id = new SelectList(db.FOOD_TYPE, "type_id", "name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct([Bind(Include = "food_id,name,description,thumbnail,price,is_delete,type_id")] FOOD fOOD)
        {
            if (ModelState.IsValid)
            {
                fOOD.food_id = Guid.NewGuid();
                db.FOODs.Add(fOOD);
                db.SaveChanges();
                return RedirectToAction("Product");
            }

            ViewBag.type_id = new SelectList(db.FOOD_TYPE, "type_id", "name", fOOD.type_id);
            return View(fOOD);
        }

        public ActionResult EditProduct(Guid? id)
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
        public ActionResult EditProduct([Bind(Include = "food_id,name,description,thumbnail,price,is_delete,type_id")] FOOD fOOD)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fOOD).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Product");
            }
            ViewBag.type_id = new SelectList(db.FOOD_TYPE, "type_id", "name", fOOD.type_id);
            return View(fOOD);
        }

        [HttpDelete]
        public ActionResult DeleteFood(Guid foodId)
        {
            FOOD food = db.FOODs.Find(foodId);
            db.FOODs.Remove(food);
            db.SaveChanges();
            return RedirectToAction("Product", "Admin");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult User()
        {
            var users = db.USERs.Include(u => u.ORDERs).ToList();
            return View(users);
        }


    }
}