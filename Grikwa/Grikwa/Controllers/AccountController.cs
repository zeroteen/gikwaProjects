using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Grikwa.Models;
using System.Net.Mail;
using System.Web.Security;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Globalization;
using System.IO;
using System.Drawing.Imaging;

namespace Grikwa.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        #region Properties
        public ApplicationDbContext db { get; private set; }
        public UserManager<ApplicationUser> UserManager { get; private set; }
        #endregion

        #region Constructor
        public AccountController()
            : this(new ApplicationDbContext())
        {

        }

        public AccountController(ApplicationDbContext context)
        {
            db = context;
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }
        #endregion

        #region Account Access
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null && user.Verified == true)
                {
                    await SignInAsync(user, model.RememberMe);
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    Session.Add("userName", user.TitleID + " " + user.Intials + " " + user.Surname);
                    Session.Add("userID", user.Id);
                    Session.Add("institutionID", user.InstitutionID);

                    if (Session != null && Session["categories"] == null)
                    {
                        Session.Add("categories", (from c in db.Categories
                                                   select c).ToList());
                    }

                    if (returnUrl == null)
                    {
                        // get user institutuin code
                        var institution = await db.Institutions.FindAsync(user.InstitutionID);

                        Session.Add("currentInstitution", institution.abbreviation);

                        return RedirectToAction("Index", "NoticeBoard");
                    }
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Account Management

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your new password has been set."
                : message == ManageMessageId.ProfileSaved ? "Changes to your profile were suceesfully saved."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            return View();
        }

        #region Password Management
        //
        // GET: Account/RequestResetToken
        [AllowAnonymous]
        public ActionResult RequestResetToken()
        {
            ViewBag.Success = false;
            return View();
        }

        //
        // POST: Account/RequestResetToken
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> RequestResetToken(RequestTokenModel model)
        {
            var success = false;

            if (ModelState.IsValid)
            {

                ApplicationUser user = null;
                #region validate email
                bool emailValid = false;
                string username = "";
                string extension = "";
                string email = model.Email.ToLower();
                try
                {
                    username = model.Email.Substring(0, model.Email.IndexOf('@')).ToLower();
                    extension = model.Email.Substring(model.Email.IndexOf('@') + 1).ToLower();

                    var userInfo = (from u in db.Users
                                    where u.UserName.Equals(username) && u.Email.Equals(email) && u.Verified == true
                                    select u);

                    // validate the email extension and username the student entered
                    var count = await userInfo.CountAsync();
                    if (count > 0)
                    {
                        emailValid = true;
                        user = await userInfo.FirstAsync();
                    }
                    else
                    {
                        emailValid = false;
                        ModelState.AddModelError("Email", "The student email address entered is not valid");
                    }
                }
                catch (Exception e)
                {
                    emailValid = false;
                    Trace.WriteLine(e.Message, "Reset Token Request");
                    ModelState.AddModelError("Email", "The student email address entered is not valid");
                }
                #endregion

                #region send reset email
                if (emailValid)
                {

                    string clientName = user.TitleID + " " + user.Intials + " " + user.Surname;

                    success = NotificationsHelper.SendPasswordResetEmail(user.Email, clientName, this.ControllerContext);
                    if (!success)
                    {
                        Trace.WriteLine(String.Format("*** WARNING:  A reset email to '{0}' failed.", user.Email));
                        ModelState.AddModelError("", "An error occured while sending a reset password email. Please try again later");
                    }
                }
                #endregion
            }

            ViewBag.Success = success;
            return View();
        }

        /// <summary>
        /// GET: /Account/Reset
        /// </summary>
        /// <returns>Reset password view</returns>
        [AllowAnonymous]
        public async Task<ActionResult> Reset(string email, string token)
        {

            if (token == null || email == null)
            {
                return RedirectToAction("Index", "NoticeBoard");
            }
            else
            {
                if (AccountHelper.IsTokenValid(token, 5, email))
                {
                    var userInfo = db.Users.Include(u => u.Institution).Where(t => t.Email.Equals(email));

                    var count = await userInfo.CountAsync();
                    if (count < 1)
                    {
                        Trace.WriteLine(String.Format("*** WARNING:  A user tried to reset their password but the email address used '{0}' does not exist in the database.", email));
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // get user
                        var user = await userInfo.FirstAsync();

                        // set session variables
                        Session.Add("userName", user.TitleID + " " + user.Intials + " " + user.Surname);
                        Session.Add("userID", user.Id);
                        Session.Add("institutionID", user.InstitutionID);
                        Session.Add("currentInstitution", user.Institution.abbreviation);

                        // remove the old user password and sign the user in to set a new password
                        await UserManager.RemovePasswordAsync(user.Id);
                        await SignInAsync(user, false);
                        FormsAuthentication.SetAuthCookie(user.UserName, false);

                        return RedirectToAction("ManagePassword");
                    }
                }
                else
                {
                    Trace.WriteLine(String.Format("*** WARNING:  A user with email '{0}' is trying to reset their password with expired token.", email));
                    return View("ForbiddenReset");
                }
            }

        }

        //
        // GET: /Account/Manage
        public ActionResult ManagePassword()
        {
            ViewBag.HasLocalPassword = HasPassword();
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManagePassword(ManagePasswordViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

        #region User Management

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                #region validate email
                Institution institution = db.Institutions.Find(model.Institution);
                Resident resident = db.Residents.Find(1);
                Faculty faculty = db.Faculties.Find(1);
                Qualification qualification = db.Qualifications.Find(1);

                bool emailValid = false;
                string username = "";
                string extension = "";

                try
                {
                    username = model.Email.Substring(0, model.Email.IndexOf('@')).ToLower();
                    extension = model.Email.Substring(model.Email.IndexOf('@') + 1).ToLower();

                    // validate the email extension the student entered
                    if (institution.Extension1.Equals(extension) || institution.Extension2.Equals(extension)
                        || institution.Extension3.Equals(extension) || institution.Extension4.Equals(extension)
                        || institution.Extension5.Equals(extension))
                    {
                        emailValid = true;
                    }
                    else
                    {
                        emailValid = false;
                        ModelState.AddModelError("Email", "The student email address entered is not valid");
                    }
                }
                catch (Exception e)
                {
                    emailValid = false;
                    Trace.WriteLine(e.Message, "Invalid Email During Registration: " + model.Email);
                    ModelState.AddModelError("Email", "The student email address entered is not valid");
                }
                #endregion

                #region create user and send verification email
                if (emailValid)
                {

                    // set text info to be able to capitalize the product name
                    TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                    var user = new ApplicationUser()
                    {
                        UserName = username,
                        Email = model.Email,
                        Institution = institution,
                        Faculty = faculty,
                        Qualification = qualification,
                        Resident = resident,
                        Verified = false,
                        RegistrationDate = DateTime.Now,
                        LastSeen = DateTime.Now
                    };

                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {

                        // get user info
                        var unverifiedUser = UserManager.FindByName(user.UserName);
                        var userFullName = user.TitleID + " " + user.Intials + " " + user.Surname;

                        // send verification email
                        bool success = NotificationsHelper.SendEmailWithVerificationToken(user.Email, userFullName, user.UserName, ControllerContext);
                        if (!success)
                        {
                            ModelState.AddModelError("", "An error occured while sending the verifiaction email. We will try to send the verification again soon.");
                        }
                        else
                        {
                            Session.Add("currentInstitution", institution.abbreviation);
                            Session.Add("verifyName", userFullName);
                            return RedirectToAction("Verification", "Account");
                        }

                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
                #endregion
            }

            // recreate form data
            model.Institutions = GetInstitutions();
            // If we got this far, something failed, redisplay form
            return View("~/Views/Home/Index.cshtml", model);
        }

        [AllowAnonymous]
        public ActionResult Verification()
        {
            if (Session != null && Session["verifyName"] != null)
            {
                ViewBag.Name = Session["verifyName"];
                return View();
            }
            ViewBag.Name = "(What are you trying to do?)";
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> Verify(string email, string token)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Index", "NoticeBoard");
            }
            else if (AccountHelper.IsTokenValid(token, 120, email))
            {
                // get user info
                var userInfo = db.Users.Include(u => u.Institution).Where(t => t.Email.Equals(email));

                // check if user exist
                var count = await userInfo.CountAsync();
                if (count < 1)
                {
                    Trace.WriteLine(String.Format("*** WARNING:  A user tried to verify themselves but the email address used '{0}' does not exist in the database.", email));
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    // get user
                    var user = await userInfo.FirstAsync();

                    // set session variables
                    Session.Add("userName", user.TitleID + " " + user.Intials + " " + user.Surname);
                    Session.Add("userID", user.Id);
                    Session.Add("institutionID", user.InstitutionID);
                    Session.Add("currentInstitution", user.Institution.abbreviation);

                    // verify user
                    user.Verified = true;
                    await UserManager.UpdateAsync(user);

                    await SignInAsync(user, false);
                    FormsAuthentication.SetAuthCookie(user.UserName, false);

                    if (Session != null && Session["categories"] == null)
                    {
                        Session.Add("categories", (from c in db.Categories
                                                   select c).ToList());
                    }

                    // go to institution NoticeBoard
                    return RedirectToAction("Index", "NoticeBoard");
                }
            }

            // something went wrong
            Trace.WriteLine("*** WARNING: Verification of user with email: " + email + " failed.", "Verification Failed");
            return RedirectToAction("Index", "NoticeBoard");
        }


        public async Task<ActionResult> ManageProfile()
        {

            // get current user details
            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            if (user == null) // user not found
            {
                return HttpNotFound();
            }

            // get user to edit
            var editUser = new ManageProfileViewModel()
            {
                Id = user.Id,
                Initials = user.Intials,
                Title = user.Title,
                Surname = user.Surname,
                HasPicture = user.Picture == null ? false : true
            };

            editUser.TitleSelectList = new SelectList(GetTitles(), "TitleID", "TitleID", (user.Title == null ? "0" : user.Title.TitleID));
            return View(editUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageProfile([Bind(Include = "ProfileImage,Id,Initials,Title,Surname,HasPicture")] ManageProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.Id);

                // user not found
                if (user == null)
                {
                    return HttpNotFound();
                }

                if (User.Identity.Name.Equals(user.UserName)) // the rightful user
                {

                    #region get profile image
                    // fill in the product image
                    if (model.ProfileImage != null)
                    {
                        // get image
                        byte[] thumbnailImage = null;
                        try
                        {
                            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(model.ProfileImage.InputStream);
                            System.Drawing.Image.GetThumbnailImageAbort callback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallBack);
                            System.Drawing.Image thumbNailImg = originalImage.GetThumbnailImage(100, 100, callback, IntPtr.Zero);

                            // get small image
                            MemoryStream ms2 = new MemoryStream();
                            thumbNailImg.Save(ms2, ImageFormat.Png);
                            thumbnailImage = ms2.ToArray();
                            ms2.Dispose();

                            // free memory
                            thumbNailImg.Dispose();
                            originalImage.Dispose();
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message, "Profile Image Creation");
                        }

                        user.Picture = thumbnailImage;
                    }
                    #endregion

                    #region edit user
                    // set text info to be able to capitalize the product name
                    TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                    // get title
                    var title = await db.Titles.FindAsync(model.Title.TitleID);

                    // modify user details
                    user.Surname = ti.ToTitleCase(model.Surname);
                    user.Title = title;
                    user.Intials = ti.ToUpper(model.Initials);

                    // save changes
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        // set users session info
                        SetCurrentUserSessionInfo(Request, Session);

                        return RedirectToAction("Manage", new { Message = ManageMessageId.ProfileSaved });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                    #endregion
                }
                else // not actual user
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            model.TitleSelectList = new SelectList(GetTitles(), "TitleID", "TitleID", (model.Title == null ? "0" : model.Title.TitleID));

            return View(model); // something went wrong, redisplay the edit form
        }

        private bool ThumbnailCallBack()
        {
            return false;
        }

        #endregion

        #endregion

        #region Admin Stuff
        //[Authorize(Roles="Administrator")]
        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult RestoreRooms()
        {
            ViewBag.Number = ChatRooms.RestoreRooms();
            return View();
        }

        public ActionResult GetActiveChatRooms()
        {
            var rooms = ChatRooms.GetAll();

            return View(rooms);
        }

        public async Task<ActionResult> GetOnlineUsers()
        {
            var userList = await (from u in db.Users
                                  where u.Verified == true
                                  select u).ToListAsync();

            var now = DateTime.Now;

            var onlineUsers = (from u in userList
                               let d = now.Subtract(u.LastSeen)
                               where d.TotalMinutes < 3
                               select new PendingUser()
                               {
                                   FullName = u.TitleID + " " + u.Intials + " " + u.Surname,
                                   UserID = u.UserName
                               }).ToList();

            return View(onlineUsers);

        }

        public async Task<ActionResult> GetVerifiedUsers()
        {
            var verifiedUsers = from u in db.Users
                                where u.Verified == true
                                select new UserViewModel()
                                {
                                    UserName = u.UserName,
                                    FullName = u.TitleID + " " + u.Intials + " " + u.Surname
                                };

            return View(await verifiedUsers.ToListAsync());
        }

        public async Task<ActionResult> GetUnverifiedUsers()
        {
            var unverifiedUsers = from u in db.Users
                                  where u.Verified == false
                                  select new UserViewModel()
                                  {
                                      UserName = u.UserName,
                                      FullName = u.TitleID + " " + u.Intials + " " + u.Surname
                                  };

            return View(await unverifiedUsers.ToListAsync());
        }

        public async Task<ActionResult> GiveRole(string id)
        {

            if (id != null)
            {
                using (var context = new ApplicationDbContext())
                {

                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                    // create role
                    if (!roleManager.RoleExists("Administrator"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("Administrator"));
                    }

                    // add user to role
                    var user = await userManager.FindByNameAsync(id);
                    userManager.AddToRole(user.Id, "Administrator");
                }
            }

            ViewBag.Name = id;
            return View();
        }

        public async Task<ActionResult> SendBulkResetEmail()
        {

            // get users
            var users = from u in db.Tokens
                        where u.User.Verified == true
                        select new SendBulkTokenEmailModel
                        {
                            TitleID = u.User.TitleID,
                            Intials = u.User.Intials,
                            Surname = u.User.Surname,
                            UserName = u.User.UserName,
                            TokenID = u.Id,
                            Email = u.User.Email,
                            IssueDate = u.IssueDate
                        };

            foreach (var user in users)
            {
                string clientName = user.TitleID + " " + user.Intials + " " + user.Surname;

                bool success = NotificationsHelper.SendPasswordResetEmail(user.Email, clientName, ControllerContext);

                if (!success)
                {
                    Trace.WriteLine("Failed to send reset email to: " + user.Email + " during bulk email sending", "Reset Bulk Emails");
                }
            }

            ViewBag.Title = "Bulk Reset Emails";

            return View("SendBulkVerificationEmail", await users.ToListAsync());
        }

        public async Task<ActionResult> SendBulkVerificationEmail()
        {

            var users = from u in db.Tokens
                        where u.User.Verified == false
                        select new SendBulkTokenEmailModel
                        {
                            TitleID = u.User.TitleID,
                            Intials = u.User.Intials,
                            Surname = u.User.Surname,
                            UserName = u.User.UserName,
                            TokenID = u.Id,
                            Email = u.User.Email,
                            IssueDate = u.IssueDate
                        };

            foreach (var user in users)
            {
                string clientName = user.TitleID + " " + user.Intials + " " + user.Surname;

                bool success = NotificationsHelper.SendEmailWithVerificationToken(user.Email, clientName, user.UserName, ControllerContext);

                if (!success)
                {
                    Trace.WriteLine("Failed to send verification email to: " + user.Email + " during bulk email sending", "Verification Bulk Emails");
                }
            }

            ViewBag.Title = "Bulk Verification Emails";

            return View(await users.ToListAsync());
        }

        #endregion

        #region OtherMethods
        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);

        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private List<Institution> GetInstitutions()
        {
            var institutions = db.Institutions.ToList();

            return institutions;
        }

        private IEnumerable<Qualification> GetQualifications(int InstitutionID)
        {
            List<Institution_Faculty_Qualification> ifqs = db.Institution_Faculty_Qualifications.Where(w => w.Institution_Faculty.InstitutionID == InstitutionID).ToList();
            var qualifications = ifqs.Select(q => new Qualification { QualificationID = q.QualificationID, FullName = q.Qualification.FullName, Code = q.Qualification.Code }).Distinct();

            return qualifications;
        }

        private List<Title> GetTitles()
        {
            var titles = db.Titles.ToList();

            return titles;
        }

        [AllowAnonymous]
        public async Task<ActionResult> ProfilePicture(string id)
        {

            // bad request
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // get user info
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var imageBytes = user.Picture;

            return File(imageBytes, "image/png");
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            ProfileSaved,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        public static void SetCurrentUserSessionInfo(HttpRequestBase Request, HttpSessionStateBase Session)
        {
            if (Request.IsAuthenticated)
            {
                using (var db1 = new ApplicationDbContext())
                {

                    // get user
                    var user = (from u in db1.Users
                                where u.UserName.Equals(Request.RequestContext.HttpContext.User.Identity.Name)
                                select new
                                {
                                    Name = u.TitleID + " " + u.Intials + " " + u.Surname,
                                    Id = u.Id,
                                    InstitutionID = u.Institution.InstitutionID
                                }).First();

                    // set session info
                    Session.Add("userName", user.Name);
                    Session.Add("userID", user.Id);
                    Session.Add("institutionID", user.InstitutionID);
                }
            }
        }

        #endregion
    }
}