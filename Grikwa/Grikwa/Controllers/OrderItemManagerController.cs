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
    public class OrderItemManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /OrderItemManager/
        public async Task<ActionResult> Index()
        {
            var orderitems = db.OrderItems.Include(o => o.Item).Include(o => o.Order);
            return View(await orderitems.ToListAsync());
        }

        // GET: /OrderItemManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItem orderitem = await db.OrderItems.FindAsync(id);
            if (orderitem == null)
            {
                return HttpNotFound();
            }
            return View(orderitem);
        }

        // GET: /OrderItemManager/Create
        public ActionResult Create()
        {
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "Name");
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNumber");
            return View();
        }

        // POST: /OrderItemManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="OrderItemID,OrderID,ItemID,Discount")] OrderItem orderitem)
        {
            if (ModelState.IsValid)
            {
                db.OrderItems.Add(orderitem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "Name", orderitem.ItemID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNumber", orderitem.OrderID);
            return View(orderitem);
        }

        // GET: /OrderItemManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItem orderitem = await db.OrderItems.FindAsync(id);
            if (orderitem == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "Name", orderitem.ItemID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNumber", orderitem.OrderID);
            return View(orderitem);
        }

        // POST: /OrderItemManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="OrderItemID,OrderID,ItemID,Discount")] OrderItem orderitem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderitem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "Name", orderitem.ItemID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNumber", orderitem.OrderID);
            return View(orderitem);
        }

        // GET: /OrderItemManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItem orderitem = await db.OrderItems.FindAsync(id);
            if (orderitem == null)
            {
                return HttpNotFound();
            }
            return View(orderitem);
        }

        // POST: /OrderItemManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OrderItem orderitem = await db.OrderItems.FindAsync(id);
            db.OrderItems.Remove(orderitem);
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
