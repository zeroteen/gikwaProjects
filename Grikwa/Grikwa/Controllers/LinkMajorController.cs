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
    public class LinkMajorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /LinkMajor/
        public async Task<ActionResult> Index()
        {
            var institution_faculty_qualification_majors = db.Institution_Faculty_Qualification_Majors.Include(i => i.Institution_Faculty_Qualification).Include(i => i.Major);
            return View(await institution_faculty_qualification_majors.ToListAsync());
        }

        // GET: /LinkMajor/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution_Faculty_Qualification_Major institution_faculty_qualification_major = await db.Institution_Faculty_Qualification_Majors.FindAsync(id);
            if (institution_faculty_qualification_major == null)
            {
                return HttpNotFound();
            }
            return View(institution_faculty_qualification_major);
        }

        // GET: /LinkMajor/Create
        public ActionResult Create()
        {
            ViewBag.IFQID = new SelectList(db.Institution_Faculty_Qualifications, "IFQID", "IFQID");
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "Name");
            return View();
        }

        // POST: /LinkMajor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="IFQMID,IFQID,MajorID")] Institution_Faculty_Qualification_Major institution_faculty_qualification_major)
        {
            if (ModelState.IsValid)
            {
                db.Institution_Faculty_Qualification_Majors.Add(institution_faculty_qualification_major);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IFQID = new SelectList(db.Institution_Faculty_Qualifications, "IFQID", "IFQID", institution_faculty_qualification_major.IFQID);
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "Name", institution_faculty_qualification_major.MajorID);
            return View(institution_faculty_qualification_major);
        }

        // GET: /LinkMajor/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution_Faculty_Qualification_Major institution_faculty_qualification_major = await db.Institution_Faculty_Qualification_Majors.FindAsync(id);
            if (institution_faculty_qualification_major == null)
            {
                return HttpNotFound();
            }
            ViewBag.IFQID = new SelectList(db.Institution_Faculty_Qualifications, "IFQID", "IFQID", institution_faculty_qualification_major.IFQID);
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "Name", institution_faculty_qualification_major.MajorID);
            return View(institution_faculty_qualification_major);
        }

        // POST: /LinkMajor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="IFQMID,IFQID,MajorID")] Institution_Faculty_Qualification_Major institution_faculty_qualification_major)
        {
            if (ModelState.IsValid)
            {
                db.Entry(institution_faculty_qualification_major).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IFQID = new SelectList(db.Institution_Faculty_Qualifications, "IFQID", "IFQID", institution_faculty_qualification_major.IFQID);
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "Name", institution_faculty_qualification_major.MajorID);
            return View(institution_faculty_qualification_major);
        }

        // GET: /LinkMajor/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution_Faculty_Qualification_Major institution_faculty_qualification_major = await db.Institution_Faculty_Qualification_Majors.FindAsync(id);
            if (institution_faculty_qualification_major == null)
            {
                return HttpNotFound();
            }
            return View(institution_faculty_qualification_major);
        }

        // POST: /LinkMajor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Institution_Faculty_Qualification_Major institution_faculty_qualification_major = await db.Institution_Faculty_Qualification_Majors.FindAsync(id);
            db.Institution_Faculty_Qualification_Majors.Remove(institution_faculty_qualification_major);
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
