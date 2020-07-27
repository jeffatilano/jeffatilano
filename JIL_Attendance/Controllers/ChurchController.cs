using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JIL_Attendance.Models;

namespace JIL_Attendance.Controllers
{
    public class ChurchController : Controller
    {
        private DefaultConnection db = new DefaultConnection();

        // GET: Church
        public ActionResult Index()
        {
            return View(db.main_Church.ToList());
        }

        // GET: Church/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            main_Church main_Church = db.main_Church.Find(id);
            if (main_Church == null)
            {
                return HttpNotFound();
            }
            return View(main_Church);
        }

        // GET: Church/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Church/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ChurchID,Church,ChurchPastor,CompleteAddress,Email,Phone")] main_Church main_Church)
        {
            if (ModelState.IsValid)
            {
                db.main_Church.Add(main_Church);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(main_Church);
        }

        // GET: Church/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            main_Church main_Church = db.main_Church.Find(id);
            if (main_Church == null)
            {
                return HttpNotFound();
            }
            return View(main_Church);
        }

        // POST: Church/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ChurchID,Church,ChurchPastor,CompleteAddress,Email,Phone")] main_Church main_Church)
        {
            if (ModelState.IsValid)
            {
                db.Entry(main_Church).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(main_Church);
        }

        // GET: Church/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            main_Church main_Church = db.main_Church.Find(id);
            if (main_Church == null)
            {
                return HttpNotFound();
            }
            return View(main_Church);
        }

        // POST: Church/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            main_Church main_Church = db.main_Church.Find(id);
            db.main_Church.Remove(main_Church);
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
