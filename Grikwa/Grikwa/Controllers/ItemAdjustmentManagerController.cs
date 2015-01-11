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
    public class ItemAdjustmentManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /ItemAdjustmentManager/
        public async Task<ActionResult> Index()
        {
            var itemadjustments = db.ItemAdjustments.Include(i => i.Employee).Include(i => i.Item);
            return View(await itemadjustments.ToListAsync());
        }

        // GET: /ItemAdjustmentManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemAdjustment itemadjustment = await db.ItemAdjustments.FindAsync(id);
            if (itemadjustment == null)
            {
                return HttpNotFound();
            }
            return View(itemadjustment);
        }

        // GET: /ItemAdjustmentManager/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeID = new SelectList(db.Users, "Id", "UserName");
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "Name");
            return View();
        }

        // POST: /ItemAdjustmentManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="ItemAdjustmentID,Quantity,Discount,ItemID,EmployeeID,Reason,EffectDate")] ItemAdjustment itemadjustment)
        {
            if (ModelState.IsValid)
            {
                db.ItemAdjustments.Add(itemadjustment);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(db.Users, "Id", "UserName", itemadjustment.EmployeeID);
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "Name", itemadjustment.ItemID);
            return View(itemadjustment);
        }

        // GET: /ItemAdjustmentManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemAdjustment itemadjustment = await db.ItemAdjustments.FindAsync(id);
            if (itemadjustment == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = new SelectList(db.Users, "Id", "UserName", itemadjustment.EmployeeID);
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "Name", itemadjustment.ItemID);
            return View(itemadjustment);
        }

        // POST: /ItemAdjustmentManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="ItemAdjustmentID,Quantity,Discount,ItemID,EmployeeID,Reason,EffectDate")] ItemAdjustment itemadjustment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemadjustment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Users, "Id", "UserName", itemadjustment.EmployeeID);
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "Name", itemadjustment.ItemID);
            return View(itemadjustment);
        }

        // GET: /ItemAdjustmentManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemAdjustment itemadjustment = await db.ItemAdjustments.FindAsync(id);
            if (itemadjustment == null)
            {
                return HttpNotFound();
            }
            return View(itemadjustment);
        }

        // POST: /ItemAdjustmentManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ItemAdjustment itemadjustment = await db.ItemAdjustments.FindAsync(id);
            db.ItemAdjustments.Remove(itemadjustment);
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
