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
    public class MajorManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /MajorManager/
        public async Task<ActionResult> Index()
        {
            return View(await db.Majors.ToListAsync());
        }

        // GET: /MajorManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Major major = await db.Majors.FindAsync(id);
            if (major == null)
            {
                return HttpNotFound();
            }
            return View(major);
        }

        // GET: /MajorManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /MajorManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="MajorID,Name")] Major major)
        {
            if (ModelState.IsValid)
            {
                db.Majors.Add(major);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(major);
        }

        // GET: /MajorManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Major major = await db.Majors.FindAsync(id);
            if (major == null)
            {
                return HttpNotFound();
            }
            return View(major);
        }

        // POST: /MajorManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="MajorID,Name")] Major major)
        {
            if (ModelState.IsValid)
            {
                db.Entry(major).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(major);
        }

        // GET: /MajorManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Major major = await db.Majors.FindAsync(id);
            if (major == null)
            {
                return HttpNotFound();
            }
            return View(major);
        }

        // POST: /MajorManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Major major = await db.Majors.FindAsync(id);
            db.Majors.Remove(major);
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
