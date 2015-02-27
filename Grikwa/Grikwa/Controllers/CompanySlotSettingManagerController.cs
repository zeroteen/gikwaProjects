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
    public class CompanySlotSettingManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /CompanySlotSettingManager/
        public async Task<ActionResult> Index()
        {
            var companyslotsettings = db.CompanySlotSettings.Include(c => c.Company).Include(c => c.SlotSetting);
            return View(await companyslotsettings.ToListAsync());
        }

        // GET: /CompanySlotSettingManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanySlotSetting companyslotsetting = await db.CompanySlotSettings.FindAsync(id);
            if (companyslotsetting == null)
            {
                return HttpNotFound();
            }
            return View(companyslotsetting);
        }

        // GET: /CompanySlotSettingManager/Create
        public ActionResult Create()
        {
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name");
            ViewBag.SlotSettingID = new SelectList(db.SlotSettings, "SlotSettingID", "SlotSettingID");
            return View();
        }

        // POST: /CompanySlotSettingManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="CompanySlotSettingID,CompanyID,SlotSettingID,ActiveDate")] CompanySlotSetting companyslotsetting)
        {
            if (ModelState.IsValid)
            {
                db.CompanySlotSettings.Add(companyslotsetting);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", companyslotsetting.CompanyID);
            ViewBag.SlotSettingID = new SelectList(db.SlotSettings, "SlotSettingID", "SlotSettingID", companyslotsetting.SlotSettingID);
            return View(companyslotsetting);
        }

        // GET: /CompanySlotSettingManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanySlotSetting companyslotsetting = await db.CompanySlotSettings.FindAsync(id);
            if (companyslotsetting == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", companyslotsetting.CompanyID);
            ViewBag.SlotSettingID = new SelectList(db.SlotSettings, "SlotSettingID", "SlotSettingID", companyslotsetting.SlotSettingID);
            return View(companyslotsetting);
        }

        // POST: /CompanySlotSettingManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="CompanySlotSettingID,CompanyID,SlotSettingID,ActiveDate")] CompanySlotSetting companyslotsetting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companyslotsetting).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", companyslotsetting.CompanyID);
            ViewBag.SlotSettingID = new SelectList(db.SlotSettings, "SlotSettingID", "SlotSettingID", companyslotsetting.SlotSettingID);
            return View(companyslotsetting);
        }

        // GET: /CompanySlotSettingManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanySlotSetting companyslotsetting = await db.CompanySlotSettings.FindAsync(id);
            if (companyslotsetting == null)
            {
                return HttpNotFound();
            }
            return View(companyslotsetting);
        }

        // POST: /CompanySlotSettingManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CompanySlotSetting companyslotsetting = await db.CompanySlotSettings.FindAsync(id);
            db.CompanySlotSettings.Remove(companyslotsetting);
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
