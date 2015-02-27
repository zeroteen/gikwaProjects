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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Grikwa.Controllers
{

    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public UserManager<ApplicationUser> UserManager { get; private set; }

        public UsersController()
            : this(new ApplicationDbContext())
        {
        }

        public UsersController(ApplicationDbContext context)
        {
            db = context;
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: Users
        public async Task<ActionResult> Index()
        {
            var identityUsers = db.Users.Include(a => a.Faculty).Include(a => a.Institution).Include(a => a.Qualification).Include(a => a.Resident).Include(a => a.Title);
            return View(await identityUsers.ToListAsync());
        }

        // GET: Users/Details/5
        //public ActionResult Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ApplicationUser applicationUser = db.Users.Find(id);
        //    if (applicationUser == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(applicationUser);
        //}

        //// GET: Users/Create
        //public ActionResult Create()
        //{
        //    ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Name");
        //    ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name");
        //    ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "Code");
        //    ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "Name");
        //    ViewBag.TitleID = new SelectList(db.Titles, "TitleID", "Description");
        //    return View();
        //}

        //// POST: Users/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "Id,UserName,PasswordHash,SecurityStamp,Intials,Surname,TitleID,Email,OptionalEmail,QualificationID,InstitutionID,FacultyID,ResidentID,Verified,AcceptedTerms,RegistrationDate,LastSeen,Picture")] ApplicationUser applicationUser)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Users.Add(applicationUser);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Name", applicationUser.FacultyID);
        //    ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name", applicationUser.InstitutionID);
        //    ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "Code", applicationUser.QualificationID);
        //    ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "Name", applicationUser.ResidentID);
        //    ViewBag.TitleID = new SelectList(db.Titles, "TitleID", "Description", applicationUser.TitleID);
        //    return View(applicationUser);
        //}

        //// GET: Users/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ApplicationUser applicationUser = db.Users.Find(id);
        //    if (applicationUser == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Name", applicationUser.FacultyID);
        //    ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name", applicationUser.InstitutionID);
        //    ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "Code", applicationUser.QualificationID);
        //    ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "Name", applicationUser.ResidentID);
        //    ViewBag.TitleID = new SelectList(db.Titles, "TitleID", "Description", applicationUser.TitleID);
        //    return View(applicationUser);
        //}

        //// POST: Users/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,UserName,PasswordHash,SecurityStamp,Intials,Surname,TitleID,Email,OptionalEmail,QualificationID,InstitutionID,FacultyID,ResidentID,Verified,AcceptedTerms,RegistrationDate,LastSeen,Picture")] ApplicationUser applicationUser)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(applicationUser).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Name", applicationUser.FacultyID);
        //    ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "Name", applicationUser.InstitutionID);
        //    ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "Code", applicationUser.QualificationID);
        //    ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "Name", applicationUser.ResidentID);
        //    ViewBag.TitleID = new SelectList(db.Titles, "TitleID", "Description", applicationUser.TitleID);
        //    return View(applicationUser);
        //}

        //// GET: Users/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ApplicationUser applicationUser = db.Users.Find(id);
        //    if (applicationUser == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(applicationUser);
        //}

        //// POST: Users/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(string id)
        //{
        //    ApplicationUser applicationUser = db.Users.Find(id);
        //    db.Users.Remove(applicationUser);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

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
