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
    public class MessageManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /MessageManager/
        public async Task<ActionResult> Index()
        {
            var messages = db.Messages.Include(m => m.User);
            return View(await messages.ToListAsync());
        }

        // GET: /MessageManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // GET: /MessageManager/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: /MessageManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="MessageID,UserID,Text,Frequency")] Message message)
        {
            if (ModelState.IsValid)
            {
                db.Messages.Add(message);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName", message.UserID);
            return View(message);
        }

        // GET: /MessageManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName", message.UserID);
            return View(message);
        }

        // POST: /MessageManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="MessageID,UserID,Text,Frequency")] Message message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(message).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName", message.UserID);
            return View(message);
        }

        // GET: /MessageManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: /MessageManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Message message = await db.Messages.FindAsync(id);
            db.Messages.Remove(message);
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
