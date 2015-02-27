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
    public class CompanyItemManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /CompanyItemManager/
        public async Task<ActionResult> Index()
        {
            var companyitems = db.CompanyItems.Include(c => c.Company).Include(c => c.Item);
            return View(await companyitems.ToListAsync());
        }

        // GET: /CompanyItemManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyItem companyitem = await db.CompanyItems.FindAsync(id);
            if (companyitem == null)
            {
                return HttpNotFound();
            }
            return View(companyitem);
        }

        // GET: /CompanyItemManager/Create
        public ActionResult Create()
        {
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name");
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "Name");
            return View();
        }

        // POST: /CompanyItemManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="CompanyItemID,CompanyID,ItemID")] CompanyItem companyitem)
        {
            if (ModelState.IsValid)
            {
                db.CompanyItems.Add(companyitem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", companyitem.CompanyID);
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "Name", companyitem.ItemID);
            return View(companyitem);
        }

        // GET: /CompanyItemManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyItem companyitem = await db.CompanyItems.FindAsync(id);
            if (companyitem == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", companyitem.CompanyID);
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "Name", companyitem.ItemID);
            return View(companyitem);
        }

        // POST: /CompanyItemManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="CompanyItemID,CompanyID,ItemID")] CompanyItem companyitem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companyitem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", companyitem.CompanyID);
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "Name", companyitem.ItemID);
            return View(companyitem);
        }

        // GET: /CompanyItemManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyItem companyitem = await db.CompanyItems.FindAsync(id);
            if (companyitem == null)
            {
                return HttpNotFound();
            }
            return View(companyitem);
        }

        // POST: /CompanyItemManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CompanyItem companyitem = await db.CompanyItems.FindAsync(id);
            db.CompanyItems.Remove(companyitem);
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
