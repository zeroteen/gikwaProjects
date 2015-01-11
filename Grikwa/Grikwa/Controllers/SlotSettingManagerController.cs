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
    public class SlotSettingManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /SlotSettingManager/
        public async Task<ActionResult> Index()
        {
            return View(await db.SlotSettings.ToListAsync());
        }

        // GET: /SlotSettingManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SlotSetting slotsetting = await db.SlotSettings.FindAsync(id);
            if (slotsetting == null)
            {
                return HttpNotFound();
            }
            return View(slotsetting);
        }

        // GET: /SlotSettingManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /SlotSettingManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="SlotSettingID,StartTime,EndTime,MinutesRange,MaxQuantity")] SlotSetting slotsetting)
        {
            if (ModelState.IsValid)
            {
                db.SlotSettings.Add(slotsetting);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(slotsetting);
        }

        // GET: /SlotSettingManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SlotSetting slotsetting = await db.SlotSettings.FindAsync(id);
            if (slotsetting == null)
            {
                return HttpNotFound();
            }
            return View(slotsetting);
        }

        // POST: /SlotSettingManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="SlotSettingID,StartTime,EndTime,MinutesRange,MaxQuantity")] SlotSetting slotsetting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(slotsetting).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(slotsetting);
        }

        // GET: /SlotSettingManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SlotSetting slotsetting = await db.SlotSettings.FindAsync(id);
            if (slotsetting == null)
            {
                return HttpNotFound();
            }
            return View(slotsetting);
        }

        // POST: /SlotSettingManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SlotSetting slotsetting = await db.SlotSettings.FindAsync(id);
            db.SlotSettings.Remove(slotsetting);
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
