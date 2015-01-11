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
    public class QualificationManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /QualificationManager/
        public async Task<ActionResult> Index()
        {
            return View(await db.Qualifications.ToListAsync());
        }

        // GET: /QualificationManager/Qualifications
        [AllowAnonymous]
        public async Task<ActionResult> Qualifications(int? InstitutionID)
        {
            if (InstitutionID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<Institution_Faculty_Qualification> ifqs = await db.Institution_Faculty_Qualifications.Where(w => w.Institution_Faculty.InstitutionID == InstitutionID).ToListAsync();
            var qualifications = ifqs.Select(q => new { QualificationID = q.QualificationID, Name = q.Qualification.FullName, Code = q.Qualification.Code }).Distinct();

            return Json(qualifications, JsonRequestBehavior.AllowGet);

        }

        // GET: /QualificationManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Qualification qualification = await db.Qualifications.FindAsync(id);
            if (qualification == null)
            {
                return HttpNotFound();
            }
            return View(qualification);
        }

        // GET: /QualificationManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /QualificationManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="QualificationID,Code,FullName")] Qualification qualification)
        {
            if (ModelState.IsValid)
            {
                db.Qualifications.Add(qualification);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(qualification);
        }

        // GET: /QualificationManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Qualification qualification = await db.Qualifications.FindAsync(id);
            if (qualification == null)
            {
                return HttpNotFound();
            }
            return View(qualification);
        }

        // POST: /QualificationManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="QualificationID,Code,FullName")] Qualification qualification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qualification).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(qualification);
        }

        // GET: /QualificationManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Qualification qualification = await db.Qualifications.FindAsync(id);
            if (qualification == null)
            {
                return HttpNotFound();
            }
            return View(qualification);
        }

        // POST: /QualificationManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Qualification qualification = await db.Qualifications.FindAsync(id);
            db.Qualifications.Remove(qualification);
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
