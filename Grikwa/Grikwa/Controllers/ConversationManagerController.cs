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
    public class ConversationManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /ConversationManager/
        public async Task<ActionResult> Index()
        {
            var conversations = db.Conversations.Include(c => c.FromUser).Include(c => c.ToUser);
            return View(await conversations.ToListAsync());
        }

        // GET: /ConversationManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conversation conversation = await db.Conversations.FindAsync(id);
            if (conversation == null)
            {
                return HttpNotFound();
            }
            return View(conversation);
        }

        // GET: /ConversationManager/Create
        public ActionResult Create()
        {
            ViewBag.FromUserID = new SelectList(db.Users, "Id", "UserName");
            ViewBag.ToUserID = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: /ConversationManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="ConversationID,FromUserID,ToUserID,ProductID,Text,Time,MessageStatus")] Conversation conversation)
        {
            if (ModelState.IsValid)
            {
                db.Conversations.Add(conversation);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.FromUserID = new SelectList(db.Users, "Id", "UserName", conversation.FromUserID);
            ViewBag.ToUserID = new SelectList(db.Users, "Id", "UserName", conversation.ToUserID);
            return View(conversation);
        }

        // GET: /ConversationManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conversation conversation = await db.Conversations.FindAsync(id);
            if (conversation == null)
            {
                return HttpNotFound();
            }
            ViewBag.FromUserID = new SelectList(db.Users, "Id", "UserName", conversation.FromUserID);
            ViewBag.ToUserID = new SelectList(db.Users, "Id", "UserName", conversation.ToUserID);
            return View(conversation);
        }

        // POST: /ConversationManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="ConversationID,FromUserID,ToUserID,ProductID,Text,Time,MessageStatus")] Conversation conversation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(conversation).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.FromUserID = new SelectList(db.Users, "Id", "UserName", conversation.FromUserID);
            ViewBag.ToUserID = new SelectList(db.Users, "Id", "UserName", conversation.ToUserID);
            return View(conversation);
        }

        // GET: /ConversationManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conversation conversation = await db.Conversations.FindAsync(id);
            if (conversation == null)
            {
                return HttpNotFound();
            }
            return View(conversation);
        }

        // POST: /ConversationManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Conversation conversation = await db.Conversations.FindAsync(id);
            db.Conversations.Remove(conversation);
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
