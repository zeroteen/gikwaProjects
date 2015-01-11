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
    public class SaleManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /SaleManager/
        public async Task<ActionResult> Index()
        {
            var sales = db.Sales.Include(s => s.Customer).Include(s => s.Product);
            return View(await sales.ToListAsync());
        }

        // GET: /SaleManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = await db.Sales.FindAsync(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            return View(sale);
        }

        // GET: /SaleManager/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Users, "Id", "UserName");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "UserID");
            ViewBag.SupplierID = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: /SaleManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="SaleID,ProductID,CustomerID,SupplierID,SalePrice")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                db.Sales.Add(sale);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Users, "Id", "UserName", sale.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "UserID", sale.ProductID);
            //ViewBag.SupplierID = new SelectList(db.Users, "Id", "UserName", sale.SupplierID);
            return View(sale);
        }

        // GET: /SaleManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = await db.Sales.FindAsync(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Users, "Id", "UserName", sale.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "UserID", sale.ProductID);
            //ViewBag.SupplierID = new SelectList(db.Users, "Id", "UserName", sale.SupplierID);
            return View(sale);
        }

        // POST: /SaleManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="SaleID,ProductID,CustomerID,SupplierID,SalePrice")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sale).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Users, "Id", "UserName", sale.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "UserID", sale.ProductID);
            //ViewBag.SupplierID = new SelectList(db.Users, "Id", "UserName", sale.SupplierID);
            return View(sale);
        }

        // GET: /SaleManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = await db.Sales.FindAsync(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            return View(sale);
        }

        // POST: /SaleManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Sale sale = await db.Sales.FindAsync(id);
            db.Sales.Remove(sale);
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
