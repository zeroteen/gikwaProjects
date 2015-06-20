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
    
    public class CompanySaleManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /CompanySaleManager/
        public async Task<ActionResult> Index()
        {
            var companysales = db.CompanySales.Include(c => c.Employee).Include(c => c.Order);
            return View(await companysales.ToListAsync());
        }

        // GET: /CompanySaleManager/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanySale companysale = await db.CompanySales.FindAsync(id);
            if (companysale == null)
            {
                return HttpNotFound();
            }
            return View(companysale);
        }

        // GET: /CompanySaleManager/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeID = new SelectList(db.Users, "Id", "UserName");
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNumber");
            return View();
        }

        // POST: /CompanySaleManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="CompanySaleID,EmployeeID,OrderID,SaleTime")] CompanySale companysale)
        {
            if (ModelState.IsValid)
            {
                db.CompanySales.Add(companysale);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(db.Users, "Id", "UserName", companysale.EmployeeID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNumber", companysale.OrderID);
            return View(companysale);
        }

        // GET: /CompanySaleManager/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanySale companysale = await db.CompanySales.FindAsync(id);
            if (companysale == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = new SelectList(db.Users, "Id", "UserName", companysale.EmployeeID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNumber", companysale.OrderID);
            return View(companysale);
        }

        // POST: /CompanySaleManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="CompanySaleID,EmployeeID,OrderID,SaleTime")] CompanySale companysale)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companysale).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Users, "Id", "UserName", companysale.EmployeeID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNumber", companysale.OrderID);
            return View(companysale);
        }

        // GET: /CompanySaleManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanySale companysale = await db.CompanySales.FindAsync(id);
            if (companysale == null)
            {
                return HttpNotFound();
            }
            return View(companysale);
        }

        // POST: /CompanySaleManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CompanySale companysale = await db.CompanySales.FindAsync(id);
            db.CompanySales.Remove(companysale);
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
