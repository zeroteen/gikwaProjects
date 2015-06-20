using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Grikwa.Models
{
    
    [Authorize]
    public class TitleManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Title/
        public async Task<ActionResult> Index()
        {
            return View(await db.Titles.ToListAsync());
        }

        // GET: /Title/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Title title = await db.Titles.FindAsync(id);
            if (title == null)
            {
                return HttpNotFound();
            }
            return View(title);
        }

        // GET: /Title/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Title/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="TitleID,Description")] Title title)
        {
            if (ModelState.IsValid)
            {
                db.Titles.Add(title);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(title);
        }

        // GET: /Title/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Title title = await db.Titles.FindAsync(id);
            if (title == null)
            {
                return HttpNotFound();
            }
            return View(title);
        }

        // POST: /Title/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="TitleID,Description")] Title title)
        {
            if (ModelState.IsValid)
            {
                db.Entry(title).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(title);
        }

        // GET: /Title/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Title title = await db.Titles.FindAsync(id);
            if (title == null)
            {
                return HttpNotFound();
            }
            return View(title);
        }

        // POST: /Title/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Title title = await db.Titles.FindAsync(id);
            db.Titles.Remove(title);
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
