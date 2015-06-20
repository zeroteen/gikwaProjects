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
    public class ConversationRoomManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /ConversationRoomManager/
        public async Task<ActionResult> Index()
        {
            var conversationrooms = db.ConversationRooms.Include(c => c.User1).Include(c => c.User2);
            return View(await conversationrooms.ToListAsync());
        }

        // GET: /ConversationRoomManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConversationRoom conversationroom = await db.ConversationRooms.FindAsync(id);
            if (conversationroom == null)
            {
                return HttpNotFound();
            }
            return View(conversationroom);
        }

        // GET: /ConversationRoomManager/Create
        public ActionResult Create()
        {
            ViewBag.User1ID = new SelectList(db.Users, "Id", "UserName");
            ViewBag.User2ID = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: /ConversationRoomManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="ConversationRoomID,Name,User1ID,User2ID")] ConversationRoom conversationroom)
        {
            if (ModelState.IsValid)
            {
                db.ConversationRooms.Add(conversationroom);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.User1ID = new SelectList(db.Users, "Id", "UserName", conversationroom.User1ID);
            ViewBag.User2ID = new SelectList(db.Users, "Id", "UserName", conversationroom.User2ID);
            return View(conversationroom);
        }

        // GET: /ConversationRoomManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConversationRoom conversationroom = await db.ConversationRooms.FindAsync(id);
            if (conversationroom == null)
            {
                return HttpNotFound();
            }
            ViewBag.User1ID = new SelectList(db.Users, "Id", "UserName", conversationroom.User1ID);
            ViewBag.User2ID = new SelectList(db.Users, "Id", "UserName", conversationroom.User2ID);
            return View(conversationroom);
        }

        // POST: /ConversationRoomManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="ConversationRoomID,Name,User1ID,User2ID")] ConversationRoom conversationroom)
        {
            if (ModelState.IsValid)
            {
                db.Entry(conversationroom).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.User1ID = new SelectList(db.Users, "Id", "UserName", conversationroom.User1ID);
            ViewBag.User2ID = new SelectList(db.Users, "Id", "UserName", conversationroom.User2ID);
            return View(conversationroom);
        }

        // GET: /ConversationRoomManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConversationRoom conversationroom = await db.ConversationRooms.FindAsync(id);
            if (conversationroom == null)
            {
                return HttpNotFound();
            }
            return View(conversationroom);
        }

        // POST: /ConversationRoomManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ConversationRoom conversationroom = await db.ConversationRooms.FindAsync(id);
            db.ConversationRooms.Remove(conversationroom);
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
