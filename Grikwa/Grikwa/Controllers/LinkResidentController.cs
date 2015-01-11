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
    public class LinkResidentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /LinkResident/
        public async Task<ActionResult> Index()
        {
            var institution_resident = db.Institution_Residents.Include(i => i.Institution).Include(i => i.Resident);
            return View(await institution_resident.ToListAsync());
        }

        // GET: /LinkResident/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution_Resident institution_resident = await db.Institution_Residents.FindAsync(id);
            if (institution_resident == null)
            {
                return HttpNotFound();
            }
            return View(institution_resident);
        }

        // GET: /LinkResident/Create
        public ActionResult Create()
        {
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name");
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "Name");
            return View();
        }

        // POST: /LinkResident/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="IRID,InstitutionID,ResidentID")] Institution_Resident institution_resident)
        {
            if (ModelState.IsValid)
            {
                db.Institution_Residents.Add(institution_resident);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name", institution_resident.InstitutionID);
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "Name", institution_resident.ResidentID);
            return View(institution_resident);
        }

        // GET: /LinkResident/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution_Resident institution_resident = await db.Institution_Residents.FindAsync(id);
            if (institution_resident == null)
            {
                return HttpNotFound();
            }
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name", institution_resident.InstitutionID);
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "Name", institution_resident.ResidentID);
            return View(institution_resident);
        }

        // POST: /LinkResident/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="IRID,InstitutionID,ResidentID")] Institution_Resident institution_resident)
        {
            if (ModelState.IsValid)
            {
                db.Entry(institution_resident).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name", institution_resident.InstitutionID);
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "Name", institution_resident.ResidentID);
            return View(institution_resident);
        }

        // GET: /LinkResident/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution_Resident institution_resident = await db.Institution_Residents.FindAsync(id);
            if (institution_resident == null)
            {
                return HttpNotFound();
            }
            return View(institution_resident);
        }

        // POST: /LinkResident/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Institution_Resident institution_resident = await db.Institution_Residents.FindAsync(id);
            db.Institution_Residents.Remove(institution_resident);
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
