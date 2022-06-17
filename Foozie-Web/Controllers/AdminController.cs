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
using System.IO;
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

        public ActionResult ChartDate()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<ORDER> orders = db.ORDERs.OrderBy(o => o.date).Where(o => o.status != "Waiting").ToList();
            var date = orders.Select(o => new { o.date.GetValueOrDefault().Year, o.date.GetValueOrDefault().Month, o.date.GetValueOrDefault().Day, o.total }).GroupBy(x => new
            {
                x.Year,
                x.Month,
                x.Day
            }, (key, group) => new
            {
                date = $"{key.Day}/{key.Month}/{key.Year}"
            }).ToList();
            //orders.OrderByDescending(o => o.date).Select(o => o.date.GetValueOrDefault().ToString("dd/MM/yyyy"))
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChartData()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<ORDER> orders = db.ORDERs.OrderBy(o => o.date).Where(o => o.status != "Waiting").ToList();
            var data = orders.Select(o => new { o.date.GetValueOrDefault().Year, o.date.GetValueOrDefault().Month, o.date.GetValueOrDefault().Day, o.total }).GroupBy(x => new
            {
                x.Year,
                x.Month,
                x.Day
            }, (key, group) => new
            {
                total = group.Sum(t => t.total)
            }).ToList();
            //orders.OrderByDescending(o => o.date).Select(o => o.total)
            return Json(data, JsonRequestBehavior.AllowGet);
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

        public ActionResult FoodOptions()
        {
            var types = db.FOOD_TYPE.ToList();
            return View(types);
        }

        public ActionResult FoodType()
        {
            var types = db.FOOD_TYPE.ToList();
            return View(types);
        }

        public ActionResult AddFoodType()
        {
            return View();
        }

        // POST: FOOD_TYPE1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFoodType(HttpPostedFileBase thumbnail, [Bind(Include = "type_id,name,code,description,is_delete,thumbnail")] FOOD_TYPE fOOD_TYPE)
        {
            if (ModelState.IsValid)
            {
                string path = Server.MapPath($"~/Images/food_type/");
                thumbnail.SaveAs(path + Path.GetFileName(thumbnail.FileName));
                fOOD_TYPE.thumbnail = thumbnail.FileName.ToString();
                fOOD_TYPE.type_id = Guid.NewGuid();
                db.FOOD_TYPE.Add(fOOD_TYPE);
                db.SaveChanges();
                return RedirectToAction("FoodType");
            }

            return View(fOOD_TYPE);
        }

        public ActionResult EditFoodType(Guid? id)
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
        public ActionResult EditFoodType(HttpPostedFileBase thumbnail, [Bind(Include = "type_id,name,code,description,is_delete")] FOOD_TYPE fOOD_TYPE)
        {
            if (ModelState.IsValid)
            {
                var type = db.FOOD_TYPE.Find(fOOD_TYPE.type_id);
                string fullPath = Server.MapPath("~/Images/food_type/" + type.thumbnail);
                string path = Server.MapPath($"~/Images/food_type/");
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                if (thumbnail.FileName != fOOD_TYPE.thumbnail)
                {
                    thumbnail.SaveAs(path + Path.GetFileName(thumbnail.FileName));
                    type.thumbnail = thumbnail.FileName.ToString();
                }
                type.name = fOOD_TYPE.name;
                type.code = fOOD_TYPE.code;
                type.description = fOOD_TYPE.description;
                db.SaveChanges();
                return RedirectToAction("FoodType");
            }
            return View(fOOD_TYPE);
        }

        public ActionResult Product(string search, string type)
        {
            var foods = db.FOODs.Include(f => f.FOOD_TYPE).ToList();
            if (!String.IsNullOrEmpty(search))
            {
                foods = foods.Where(f => f.name.ToLower().Contains(search.ToLower())).ToList();
            } else if (!String.IsNullOrEmpty(type) && type != "ALL")
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
        public ActionResult AddProduct(HttpPostedFileBase thumbnail,[Bind(Include = "food_id,name,description,thumbnail,price,is_delete,type_id")] FOOD fOOD)
        {
            if (ModelState.IsValid)
            {
                var type = db.FOOD_TYPE.Find(fOOD.type_id);
                string path = Server.MapPath($"~/Images/foods/{type.code}/");
                thumbnail.SaveAs(path + Path.GetFileName(thumbnail.FileName));
                fOOD.thumbnail = thumbnail.FileName.ToString();
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
        public ActionResult EditProduct(HttpPostedFileBase thumbnail, [Bind(Include = "food_id,name,description,thumbnail,price,is_delete,type_id")] FOOD fOOD)
        {
            if (ModelState.IsValid)
            {
                var food = db.FOODs.Find(fOOD.food_id);
                string fullPath = Server.MapPath($"~/Images/foods/{food.FOOD_TYPE.code}/{food.thumbnail}");
                string path = Server.MapPath($"~/Images/foods/{food.FOOD_TYPE.code}/");
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                if (thumbnail.FileName != fOOD.thumbnail)
                {
                    thumbnail.SaveAs(path + Path.GetFileName(thumbnail.FileName));
                    food.thumbnail = thumbnail.FileName.ToString();         
                }
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

        public ActionResult User(string search)
        {
            var users = db.USERs.Include(u => u.ORDERs).ToList();
            if (!String.IsNullOrEmpty(search))
            {
                users = users.Where(f => f.username.ToLower().Contains(search.ToLower())
                                    || f.first_name.ToLower().Contains(search.ToLower())
                                    || f.last_name.ToLower().Contains(search.ToLower())
                                    || f.email.ToLower().Contains(search.ToLower())
                                    || f.phone.ToLower().Contains(search.ToLower())).ToList();
            }
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

        public ActionResult AddUser()
        {
            return View();
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