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
    [Authorize]
    public class ConversationRoomProductManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /ConversationRoomProductManager/
        public async Task<ActionResult> Index()
        {
            var conversationroomproducts = db.ConversationRoomProducts.Include(c => c.ConversationRoom).Include(c => c.Product);
            return View(await conversationroomproducts.ToListAsync());
        }

        // GET: /ConversationRoomProductManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConversationRoomProduct conversationroomproduct = await db.ConversationRoomProducts.FindAsync(id);
            if (conversationroomproduct == null)
            {
                return HttpNotFound();
            }
            return View(conversationroomproduct);
        }

        // GET: /ConversationRoomProductManager/Create
        public ActionResult Create()
        {
            ViewBag.ConversationRoomID = new SelectList(db.ConversationRooms, "ConversationRoomID", "Name");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name");
            return View();
        }

        // POST: /ConversationRoomProductManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Id,ConversationRoomID,ProductID")] ConversationRoomProduct conversationroomproduct)
        {
            if (ModelState.IsValid)
            {
                db.ConversationRoomProducts.Add(conversationroomproduct);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ConversationRoomID = new SelectList(db.ConversationRooms, "ConversationRoomID", "Name", conversationroomproduct.ConversationRoomID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "UserID", conversationroomproduct.ProductID);
            return View(conversationroomproduct);
        }

        // GET: /ConversationRoomProductManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConversationRoomProduct conversationroomproduct = await db.ConversationRoomProducts.FindAsync(id);
            if (conversationroomproduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConversationRoomID = new SelectList(db.ConversationRooms, "ConversationRoomID", "Name", conversationroomproduct.ConversationRoomID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "UserID", conversationroomproduct.ProductID);
            return View(conversationroomproduct);
        }

        // POST: /ConversationRoomProductManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,ConversationRoomID,ProductID")] ConversationRoomProduct conversationroomproduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(conversationroomproduct).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ConversationRoomID = new SelectList(db.ConversationRooms, "ConversationRoomID", "Name", conversationroomproduct.ConversationRoomID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "UserID", conversationroomproduct.ProductID);
            return View(conversationroomproduct);
        }

        // GET: /ConversationRoomProductManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConversationRoomProduct conversationroomproduct = await db.ConversationRoomProducts.FindAsync(id);
            if (conversationroomproduct == null)
            {
                return HttpNotFound();
            }
            return View(conversationroomproduct);
        }

        // POST: /ConversationRoomProductManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ConversationRoomProduct conversationroomproduct = await db.ConversationRoomProducts.FindAsync(id);
            db.ConversationRoomProducts.Remove(conversationroomproduct);
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
