using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Grikwa.Models;
using Infrastructure.Implementations;

namespace Grikwa.Controllers
{
    [Authorize]
    public class InstitutionManagerController : BaseController
    {

        // GET: /InstitutionManager/
        public ActionResult Index()
        {
            return View(db.Institutions.ToList());
        }

        // GET: /InstitutionManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return HttpNotFound();
            }
            return View(institution);
        }

        // GET: /InstitutionManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /InstitutionManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="InstitutionID,Name,abbreviation,Extension1,Extension2,Extension3,Extension4,Extension5")] Institution institution)
        {
            if (ModelState.IsValid)
            {
                if (db.Institutions.FirstOrDefault(x => x.abbreviation.ToLower() == institution.abbreviation.ToLower()) != null)
                {
                    db.Institutions.Add(institution);
                    db.SaveChanges();
                    var blobStorage = new BlobMethods(storageAccountName, storageAccountKey, institution.abbreviation);
                }
                return RedirectToAction("Index");
            }

            return View(institution);
        }

        // GET: /InstitutionManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return HttpNotFound();
            }
            return View(institution);
        }

        // POST: /InstitutionManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="InstitutionID,Name,abbreviation,Extension1,Extension2,Extension3,Extension4,Extension5")] Institution institution)
        {
            if (ModelState.IsValid)
            {
                db.Entry(institution).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(institution);
        }

        // GET: /InstitutionManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return HttpNotFound();
            }
            return View(institution);
        }

        // POST: /InstitutionManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Institution institution = db.Institutions.Find(id);
            db.Institutions.Remove(institution);
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
