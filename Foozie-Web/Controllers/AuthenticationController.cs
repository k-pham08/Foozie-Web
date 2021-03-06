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
    public class AuthenticationController : Controller
    {
        private FoozieEntity db = new FoozieEntity();

        // GET: Authentication
        public ActionResult Index()
        {
            return View(db.USERs.ToList());
        }

        // GET: Authentication/Details/5
        public ActionResult Details(Guid? id)
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

        // GET: Authentication/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Authentication/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "user_id,type,first_name,last_name,email,phone,username,password")] USER uSER)
        {
            if (ModelState.IsValid)
            {
                var users = db.USERs.ToList();
                var check = false;
                foreach(var user in users)
                {
                    if(uSER.email == user.email || uSER.phone == user.phone || uSER.username == user.username)
                    {
                        check = true;
                        break;
                    }
                }
                if(!check)
                {
                    uSER.user_id = Guid.NewGuid();
                    uSER.type = "USER";
                    uSER.password = GetMD5(uSER.password);
                    db.USERs.Add(uSER);
                    db.SaveChanges();
                } else
                {
                    ViewBag.error = "Tài khoản đã tồn tại!";
                    return View();
                }
                return RedirectToAction("Login");
            }

            return View(uSER);
        }

        // POST: Authentication/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                
                string f_password = GetMD5(password);
                var data = db.USERs.Where(s => s.username.Equals(username) && s.password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    if (data.FirstOrDefault().type == "USER")
                    {
                        Session["idUser"] = data.FirstOrDefault().user_id;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        HttpCookie adminCookie = new HttpCookie("admin");
                        adminCookie.Value = data.FirstOrDefault().username;
                        adminCookie.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Add(adminCookie);
                        return RedirectToAction("Dashboard", "Admin");
                    }
                }
                else
                {
                    ViewBag.error = "Sai tên đăng nhập hoặc mật khẩu";
                    return View();
                }

                
            }
            return RedirectToAction("Login");
        }

        public ActionResult LogOut()
        {
            Session.RemoveAll();
            if (Request.Cookies["admin"] != null)
            {
                HttpCookie admin = Request.Cookies["admin"];

                admin.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(admin);
            }
            return RedirectToAction("Index", "Home");
        }
        
        // GET: Authentication/Edit/5
        public ActionResult Edit(Guid? id)
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

        // POST: Authentication/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,type,first_name,last_name,email,phone,username,password")] USER uSER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uSER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(uSER);
        }

        // GET: Authentication/Delete/5
        public ActionResult Delete(Guid? id)
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

        // POST: Authentication/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            USER uSER = db.USERs.Find(id);
            db.USERs.Remove(uSER);
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

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byteToString = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byteToString += targetData[i].ToString("x2");

            }
            return byteToString;

        }
    }
}
