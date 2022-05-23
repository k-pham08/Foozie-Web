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
            if (Request.Cookies["admin"] != null)
            {
                List<ORDER> orders = db.ORDERs.ToList();
                return View(orders);
            }
            return RedirectToAction("Login", "Authentication");
        }

        public ActionResult Order(string start, string end)
        {
            var orders = db.ORDERs.Where(o => o.status != "Waiting");

            if (!String.IsNullOrEmpty(start) && !String.IsNullOrEmpty(end))
            {
                DateTime startDate = DateTime.Parse(start);
                DateTime endDate = DateTime.Parse(end);
                orders = orders.Where(o => o.date >= startDate && o.date <= endDate);
            }
            return View(orders.ToList());
        }

        public ActionResult EditOrder(Guid? id)
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
            ViewBag.user_id = new SelectList(db.USERs, "user_id", "type", oRDER.user_id);
            return View(oRDER);
        }

        // POST: ORDER/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder([Bind(Include = "order_id,date,status,address,total,user_id")] ORDER oRDER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oRDER).State = EntityState.Modified;
                oRDER.date = DateTime.Now;
                oRDER.status = "Payment";
                db.SaveChanges();
                return RedirectToAction("Order");
            }

            ViewBag.user_id = new SelectList(db.USERs, "user_id", "type", oRDER.user_id);
            return View(oRDER);
        }

        [HttpDelete]
        public ActionResult DeleteOrder(Guid orderId)
        {
            ORDER order = db.ORDERs.Find(orderId);
            List<ORDER_DETAIL> details = db.ORDER_DETAIL.Where(o => o.order_id == orderId).ToList();
            foreach (ORDER_DETAIL detail in details)
                db.ORDER_DETAIL.Remove(detail);
            db.ORDERs.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Order", "Admin");
        }

        public ActionResult Product(string search, string type)
        {
            var foods = db.FOODs.Include(f => f.FOOD_TYPE).ToList();
            if (!String.IsNullOrEmpty(search))
            {
                foods = foods.Where(f => f.name.ToLower().Contains(search.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(type) && type != "ALL")
            {
                foods = foods.Where(f => f.FOOD_TYPE.code == type).ToList();
            }
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

        public ActionResult User()
        {
            var users = db.USERs.Include(u => u.ORDERs).ToList();
            return View(users);
        }

        public ActionResult UserDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER users  = db.USERs.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser([Bind(Include = "user_id,type,first_name,last_name,email,phone,username,password")] USER uSER)
        {
            if (ModelState.IsValid)
            {
                uSER.user_id = Guid.NewGuid();
                db.USERs.Add(uSER);
                db.SaveChanges();
                return RedirectToAction("User");
            }

            return View(uSER);
        }

        public ActionResult EditUser(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER uSER = db.USERs.Find(id);
            if (uSER == null)
            {
                return HttpNotFound();
            }
            return View(uSER);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "user_id,type,first_name,last_name,email,phone,username,password")] USER uSER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uSER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("User");
            }
            return View(uSER);
        }

        [HttpDelete]
        public ActionResult DeleteUser(Guid userId)
        {
            USER user = db.USERs.Find(userId);
            List<ORDER> orders = db.ORDERs.Where(o => o.user_id == userId).ToList();
            foreach(ORDER order in orders)
            {
                List<ORDER_DETAIL> details = db.ORDER_DETAIL.Where(d => d.order_id == order.order_id).ToList();
                foreach (ORDER_DETAIL detail in details)
                    db.ORDER_DETAIL.Remove(detail);
                db.ORDERs.Remove(order);
            }
            db.USERs.Remove(user);
            db.SaveChanges();
            return RedirectToAction("User", "Admin");
        }

    }
}