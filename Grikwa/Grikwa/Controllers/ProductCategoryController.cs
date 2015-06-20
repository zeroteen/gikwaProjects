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
    public class ProductCategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /ProductCategory/
        public async Task<ActionResult> Index()
        {
            var productcategories = db.ProductCategories.Include(p => p.Category).Include(p => p.Product);
            return View(await productcategories.ToListAsync());
        }

        // GET: /ProductCategory/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productcategory = await db.ProductCategories.FindAsync(id);
            if (productcategory == null)
            {
                return HttpNotFound();
            }
            return View(productcategory);
        }

        // GET: /ProductCategory/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "UserID");
            return View();
        }

        // POST: /ProductCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="ProductCategoryID,ProductID,CategoryID")] ProductCategory productcategory)
        {
            if (ModelState.IsValid)
            {
                db.ProductCategories.Add(productcategory);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", productcategory.CategoryID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "UserID", productcategory.ProductID);
            return View(productcategory);
        }

        // GET: /ProductCategory/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productcategory = await db.ProductCategories.FindAsync(id);
            if (productcategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", productcategory.CategoryID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "UserID", productcategory.ProductID);
            return View(productcategory);
        }

        // POST: /ProductCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="ProductCategoryID,ProductID,CategoryID")] ProductCategory productcategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productcategory).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", productcategory.CategoryID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "UserID", productcategory.ProductID);
            return View(productcategory);
        }

        // GET: /ProductCategory/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productcategory = await db.ProductCategories.FindAsync(id);
            if (productcategory == null)
            {
                return HttpNotFound();
            }
            return View(productcategory);
        }

        // POST: /ProductCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProductCategory productcategory = await db.ProductCategories.FindAsync(id);
            db.ProductCategories.Remove(productcategory);
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
