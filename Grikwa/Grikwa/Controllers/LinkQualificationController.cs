using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Grikwa.Models;

namespace Grikwa.Controllers
{
    [Authorize]
    public class LinkQualificationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /LinkQualification/
        public async Task<ActionResult> Index()
        {
            var institution_faculty_qualifications = db.Institution_Faculty_Qualifications.Include(i => i.Institution_Faculty).Include(i => i.Qualification);
            return View(await institution_faculty_qualifications.ToListAsync());
        }

        // GET: /LinkQualification/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution_Faculty_Qualification institution_faculty_qualification = await db.Institution_Faculty_Qualifications.FindAsync(id);
            if (institution_faculty_qualification == null)
            {
                return HttpNotFound();
            }
            return View(institution_faculty_qualification);
        }

        // GET: /LinkQualification/Create
        public ActionResult Create()
        {
            ViewBag.IFID = new SelectList(db.Institution_Faculties, "IFID", "IFID");
            ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "Code");
            return View();
        }

        // POST: /LinkQualification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="IFQID,IFID,QualificationID")] Institution_Faculty_Qualification institution_faculty_qualification)
        {
            if (ModelState.IsValid)
            {
                db.Institution_Faculty_Qualifications.Add(institution_faculty_qualification);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IFID = new SelectList(db.Institution_Faculties, "IFID", "IFID", institution_faculty_qualification.IFID);
            ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "Code", institution_faculty_qualification.QualificationID);
            return View(institution_faculty_qualification);
        }

        // GET: /LinkQualification/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution_Faculty_Qualification institution_faculty_qualification = await db.Institution_Faculty_Qualifications.FindAsync(id);
            if (institution_faculty_qualification == null)
            {
                return HttpNotFound();
            }
            ViewBag.IFID = new SelectList(db.Institution_Faculties, "IFID", "IFID", institution_faculty_qualification.IFID);
            ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "Code", institution_faculty_qualification.QualificationID);
            return View(institution_faculty_qualification);
        }

        // POST: /LinkQualification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="IFQID,IFID,QualificationID")] Institution_Faculty_Qualification institution_faculty_qualification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(institution_faculty_qualification).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IFID = new SelectList(db.Institution_Faculties, "IFID", "IFID", institution_faculty_qualification.IFID);
            ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "Code", institution_faculty_qualification.QualificationID);
            return View(institution_faculty_qualification);
        }

        // GET: /LinkQualification/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution_Faculty_Qualification institution_faculty_qualification = await db.Institution_Faculty_Qualifications.FindAsync(id);
            if (institution_faculty_qualification == null)
            {
                return HttpNotFound();
            }
            return View(institution_faculty_qualification);
        }

        // POST: /LinkQualification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Institution_Faculty_Qualification institution_faculty_qualification = await db.Institution_Faculty_Qualifications.FindAsync(id);
            db.Institution_Faculty_Qualifications.Remove(institution_faculty_qualification);
            await db.SaveChangesAsync();
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
