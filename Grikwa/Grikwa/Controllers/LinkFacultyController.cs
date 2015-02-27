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
    [RequireHttps]
    [Authorize]
    public class LinkFacultyController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /LinkFaculty/
        public async Task<ActionResult> Index()
        {
            var institution_faculties = db.Institution_Faculties.Include(i => i.Faculty).Include(i => i.Institution);
            return View(await institution_faculties.ToListAsync());
        }

        // GET: /LinkFaculty/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution_Faculty institution_faculty = await db.Institution_Faculties.FindAsync(id);
            if (institution_faculty == null)
            {
                return HttpNotFound();
            }
            return View(institution_faculty);
        }

        // GET: /LinkFaculty/Create
        public ActionResult Create()
        {
            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Name");
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name");
            return View();
        }

        // POST: /LinkFaculty/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="IFID,InstitutionID,FacultyID")] Institution_Faculty institution_faculty)
        {
            if (ModelState.IsValid)
            {
                db.Institution_Faculties.Add(institution_faculty);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Name", institution_faculty.FacultyID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name", institution_faculty.InstitutionID);
            return View(institution_faculty);
        }

        // GET: /LinkFaculty/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution_Faculty institution_faculty = await db.Institution_Faculties.FindAsync(id);
            if (institution_faculty == null)
            {
                return HttpNotFound();
            }
            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Name", institution_faculty.FacultyID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name", institution_faculty.InstitutionID);
            return View(institution_faculty);
        }

        // POST: /LinkFaculty/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="IFID,InstitutionID,FacultyID")] Institution_Faculty institution_faculty)
        {
            if (ModelState.IsValid)
            {
                db.Entry(institution_faculty).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Name", institution_faculty.FacultyID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name", institution_faculty.InstitutionID);
            return View(institution_faculty);
        }

        // GET: /LinkFaculty/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution_Faculty institution_faculty = await db.Institution_Faculties.FindAsync(id);
            if (institution_faculty == null)
            {
                return HttpNotFound();
            }
            return View(institution_faculty);
        }

        // POST: /LinkFaculty/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Institution_Faculty institution_faculty = await db.Institution_Faculties.FindAsync(id);
            db.Institution_Faculties.Remove(institution_faculty);
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
