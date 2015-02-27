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
    public class CompanyEmployeeManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /CompanyEmployeeManager/
        public async Task<ActionResult> Index()
        {
            var companyemployees = db.CompanyEmployees.Include(c => c.Company).Include(c => c.Employee);
            return View(await companyemployees.ToListAsync());
        }

        // GET: /CompanyEmployeeManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyEmployee companyemployee = await db.CompanyEmployees.FindAsync(id);
            if (companyemployee == null)
            {
                return HttpNotFound();
            }
            return View(companyemployee);
        }

        // GET: /CompanyEmployeeManager/Create
        public ActionResult Create()
        {
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name");
            ViewBag.EmployeeID = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: /CompanyEmployeeManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="CompanyEmployeeID,EmployeeID,CompanyID")] CompanyEmployee companyemployee)
        {
            if (ModelState.IsValid)
            {
                db.CompanyEmployees.Add(companyemployee);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", companyemployee.CompanyID);
            ViewBag.EmployeeID = new SelectList(db.Users, "Id", "UserName", companyemployee.EmployeeID);
            return View(companyemployee);
        }

        // GET: /CompanyEmployeeManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyEmployee companyemployee = await db.CompanyEmployees.FindAsync(id);
            if (companyemployee == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", companyemployee.CompanyID);
            ViewBag.EmployeeID = new SelectList(db.Users, "Id", "UserName", companyemployee.EmployeeID);
            return View(companyemployee);
        }

        // POST: /CompanyEmployeeManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="CompanyEmployeeID,EmployeeID,CompanyID")] CompanyEmployee companyemployee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companyemployee).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", companyemployee.CompanyID);
            ViewBag.EmployeeID = new SelectList(db.Users, "Id", "UserName", companyemployee.EmployeeID);
            return View(companyemployee);
        }

        // GET: /CompanyEmployeeManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyEmployee companyemployee = await db.CompanyEmployees.FindAsync(id);
            if (companyemployee == null)
            {
                return HttpNotFound();
            }
            return View(companyemployee);
        }

        // POST: /CompanyEmployeeManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CompanyEmployee companyemployee = await db.CompanyEmployees.FindAsync(id);
            db.CompanyEmployees.Remove(companyemployee);
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
