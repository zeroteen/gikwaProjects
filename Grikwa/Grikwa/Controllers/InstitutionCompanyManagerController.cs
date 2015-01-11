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
    public class InstitutionCompanyManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /InstitutionCompanyManager/
        public async Task<ActionResult> Index()
        {
            var institutioncompanies = db.InstitutionCompanies.Include(i => i.Company).Include(i => i.Institution).Include(i => i.Location);
            return View(await institutioncompanies.ToListAsync());
        }

        // GET: /InstitutionCompanyManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstitutionCompany institutioncompany = await db.InstitutionCompanies.FindAsync(id);
            if (institutioncompany == null)
            {
                return HttpNotFound();
            }
            return View(institutioncompany);
        }

        // GET: /InstitutionCompanyManager/Create
        public ActionResult Create()
        {
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name");
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name");
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "Name");
            return View();
        }

        // POST: /InstitutionCompanyManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="InstitutionCompanyID,CompanyID,InstitutionID,LocationID")] InstitutionCompany institutioncompany)
        {
            if (ModelState.IsValid)
            {
                db.InstitutionCompanies.Add(institutioncompany);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", institutioncompany.CompanyID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name", institutioncompany.InstitutionID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "Name", institutioncompany.LocationID);
            return View(institutioncompany);
        }

        // GET: /InstitutionCompanyManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstitutionCompany institutioncompany = await db.InstitutionCompanies.FindAsync(id);
            if (institutioncompany == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", institutioncompany.CompanyID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name", institutioncompany.InstitutionID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "Name", institutioncompany.LocationID);
            return View(institutioncompany);
        }

        // POST: /InstitutionCompanyManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="InstitutionCompanyID,CompanyID,InstitutionID,LocationID")] InstitutionCompany institutioncompany)
        {
            if (ModelState.IsValid)
            {
                db.Entry(institutioncompany).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", institutioncompany.CompanyID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name", institutioncompany.InstitutionID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "Name", institutioncompany.LocationID);
            return View(institutioncompany);
        }

        // GET: /InstitutionCompanyManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstitutionCompany institutioncompany = await db.InstitutionCompanies.FindAsync(id);
            if (institutioncompany == null)
            {
                return HttpNotFound();
            }
            return View(institutioncompany);
        }

        // POST: /InstitutionCompanyManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            InstitutionCompany institutioncompany = await db.InstitutionCompanies.FindAsync(id);
            db.InstitutionCompanies.Remove(institutioncompany);
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
