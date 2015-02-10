using Grikwa.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Diagnostics;
using System.Configuration;
using Infrastructure.Implementations;

namespace Grikwa.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult NewIndex()
        {
            return View();
        }


        public async Task<ActionResult> Index()
        {
            RegisterViewModel registerModel = new RegisterViewModel();
            registerModel.Institutions = await db.Institutions.ToListAsync();
            return View(registerModel);
        }

        public async Task<ActionResult> InstitutionImage(string code)
        {
            if (code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db1 = new ApplicationDbContext())
            {
                // get the product
                var image = from i in db1.Institutions
                            where i.abbreviation.Equals(code)
                            select i.Image;

                var count = await image.CountAsync();
                if (count < 1)
                {
                    return HttpNotFound("Image was not found"); // image was not found
                }

                byte[] imageBytes = await image.FirstAsync();

                return File(imageBytes, "image/png");
            }
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact([Bind(Include = "From,Name,Message")] ContactUsModel model)
        {

            if (ModelState.IsValid)
            {
                SendContactUsMessage(model.From, model.Name, model.Message);

                return View("MessageReceived");
            }

            return View(model);
        }

        private void SendContactUsMessage(string From, string Name, string Message)
        {

            try
            {
                MailMessage mail = new MailMessage();

                string from = "support@grikwa.co.za";
                string subject = "Grikwa Contact Us";
                string bodyHTML = "<h1><strong>Dear Grikwa Stuff</strong>.</h1>"
                                  + "<h3>Contact Us Messgae</h3>"
                                  + "<p><strong>Name</strong>: " + Name + "</p>"
                                  + "<p><strong>Email Address</strong>: " + From + "</p>"
                                  + "<p><strong>Message</strong>: " + Message + "</p>"
                                  + " <br/> <h5>Grikwa Team</h5>";
                string bodyText = "Dear Grikwa Stuuf. "
                                  + "Contact Us Message. Name:" + Name + ". Email Address: " + From + ". Message: " + Message
                                  + " .Grikwa Team";

                // To
                mail.To.Add("grikwa@gmail.com"); // grikwa email

                // From
                mail.From = new MailAddress(from);

                // Subject and multipart/alternative Body
                mail.Subject = subject;
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyText, null, MediaTypeNames.Text.Plain));
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyHTML, null, MediaTypeNames.Text.Html));

                SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("azure_a34c075f62bba74426624b9a65795a59@azure.com", "pqx33rsp"); // Enter senders User name and password
                smtp.Credentials = credentials;
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (SmtpFailedRecipientException smtpFailedRecipientException)
            {
                Trace.WriteLine(smtpFailedRecipientException.Message, "Contact Us Email From : " + From + " failed.");
            }
            catch (SmtpException smtpException)
            {
                Trace.WriteLine(smtpException.Message, "Contact Us Email From : " + From + " failed.");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message, "Contact Us Email From : " + From + " failed.");
            }
        }

        public ActionResult Help()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult PopulateDatabase(int start, int end, string inst, string pass)
        {

            // fill institutions
            //db.Institutions.Add(new Institution()
            //                        {
            //                            Name = "University of Cape Town",
            //                            abbreviation = "UCT",
            //                            Extension1 = "myuct.ac.za"
            //                        });
            //db.Institutions.Add(new Institution()
            //                        {
            //                            Name = "University of Western Cape",
            //                            abbreviation = "UWC",
            //                            Extension1 = "myuwc.ac.za"
            //                        });

            ////// fill faculties
            //db.Faculties.Add(new Faculty() { Name = "Science" });
            //db.Faculties.Add(new Faculty() { Name = "Commerce" });
            //db.Faculties.Add(new Faculty() { Name = "Law" });
            //db.Faculties.Add(new Faculty() { Name = "Engineering" });

            ////// fill qualifications
            //db.Qualifications.Add(new Qualification() { Code = "BSc", FullName = "Bachelor of Science" });
            //db.Qualifications.Add(new Qualification() { Code = "BEng", FullName = "Bachelor of Engineering" });
            //db.Qualifications.Add(new Qualification() { Code = "BCom", FullName = "Bachelor of Commerce" });
            //db.Qualifications.Add(new Qualification() { Code = "LLB", FullName = "Bachelor of Law" });

            ////// fill majors
            //db.Majors.Add(new Major() { Name = "Computer Science" });
            //db.Majors.Add(new Major() { Name = "Information Systems" });
            //db.Majors.Add(new Major() { Name = "3D Game Design" });
            //db.Majors.Add(new Major() { Name = "Mathematics" });
            //db.Majors.Add(new Major() { Name = "Applied Mathematics" });
            //db.Majors.Add(new Major() { Name = "Statistics" });
            //db.Majors.Add(new Major() { Name = "Physical Science" });

            //// fill residents
            //db.Residents.Add(new Resident() { Name = "Home" });
            //db.Residents.Add(new Resident() { Name = "Leo Marquard Hall" });
            //db.Residents.Add(new Resident() { Name = "Liesbeeck Gardens" });
            //db.Residents.Add(new Resident() { Name = "Forest Hill" });
            //db.Residents.Add(new Resident() { Name = "Tugwel" });
            //db.Residents.Add(new Resident() { Name = "Kopano" });
            //db.Residents.Add(new Resident() { Name = "Baxter Hall" });
            //db.Residents.Add(new Resident() { Name = "College House" });
            //db.Residents.Add(new Resident() { Name = "University House" });

            ////// fill titles
            //db.Titles.Add(new Title() { TitleID = "Mr", Description = "A man" });
            //db.Titles.Add(new Title() { TitleID = "Miss", Description = "Not marries woman" });
            //db.Titles.Add(new Title() { TitleID = "Mrs", Description = "Married woman" });
            //db.Titles.Add(new Title() { TitleID = "Ms", Description = "Not married anymore woman" });
            //db.Titles.Add(new Title() { TitleID = "Prof", Description = "A professor" });
            //db.Titles.Add(new Title() { TitleID = "Dr", Description = "A doctor" });

            //// fill categories
            //db.Categories.Add(new Category() { Name = "Textbooks", IconName = "icon-book", Code = "textbooks" });
            //db.Categories.Add(new Category() { Name = "Electronics", IconName = "icon-cogs", Code = "electronics" });
            //db.Categories.Add(new Category() { Name = "Kitchen and Lifestyle", IconName = "icon-food", Code = "k&l" });
            //db.Categories.Add(new Category() { Name = "Accommodation", IconName = "icon-map-marker", Code = "accommodation" });
            //db.Categories.Add(new Category() { Name = "Services", IconName = "icon-male", Code = "services" });
            //db.Categories.Add(new Category() { Name = "Other", IconName = "icon-star", Code = "other" });

            //db.SaveChangesAsync();
            Institution institution = db.Institutions.First(x => x.abbreviation == inst);
            Resident resident = db.Residents.Find(1);
            Faculty faculty = db.Faculties.Find(1);
            Qualification qualification = db.Qualifications.Find(1);

            for (var i = start; i<=end; i++){

                var user = new ApplicationUser()
                {
                    UserName = "grikwa"+inst+""+i,
                    Email = "grikwa@gmail.com",
                    Faculty = faculty,
                    Intials = "G",
                    Surname = "Grikwa"+i,
                    Qualification = qualification,
                    Institution = institution,
                    Resident = resident,
                    Verified = true,
                    LastSeen = DateTime.Now,
                    RegistrationDate = DateTime.Now
                };

                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));


                UserManager.Create(user, pass+""+i);
            }
            ////UserManager.CreateIdentity(user, "admin");

            return View();
        }

        public async Task<ActionResult> MoveImagesToBlobStorage(string startDate)
        {
            var storageAccountName = ConfigurationManager.AppSettings["StorageAccountName"];
            var storageAccountKey = ConfigurationManager.AppSettings["StorageAccountKey"];
            var start = DateTime.Parse(startDate);
            foreach(var product in db.Products.Where(x => x.DatePosted >= start).Include(x => x.User).Include(x => x.User.Institution))
            {
                if (product.FullSizeImage != null && product.ThumbnailImage != null)
                {
                    var containerName = product.User.Institution.abbreviation.ToLower();
                    var thumbnailName = Guid.NewGuid().ToString() + ".png";
                    var fullSizeName = Guid.NewGuid().ToString() + ".png";
                    var blobStorage = new BlobMethods(storageAccountName, storageAccountKey, containerName);
                    await blobStorage.UploadFromByteArrayAsync(product.ThumbnailImage, thumbnailName);
                    await blobStorage.UploadFromByteArrayAsync(product.FullSizeImage, fullSizeName);
                    product.ThumbnailImageName = thumbnailName;
                    product.FullSizeImageName = fullSizeName;
                    product.ThumbnailImage = null;
                    product.FullSizeImage = null;
                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                }
            }
            await db.SaveChangesAsync();
            return View();
        }

        public async Task<ActionResult> DeleteHiddenProducts()
        {
            var storageAccountName = ConfigurationManager.AppSettings["StorageAccountNameDev"];
            var storageAccountKey = ConfigurationManager.AppSettings["StorageAccountKeyDev"];
            foreach (var product in db.Products.Where(x => x.Visible == false).Include(x => x.User).Include(x => x.User.Institution))
            {
                if (!string.IsNullOrEmpty(product.ThumbnailImageName) && !string.IsNullOrEmpty(product.ThumbnailImageName)){
                    var containerName = product.User.Institution.abbreviation.ToLower();
                    var thumbnailName = product.ThumbnailImageName;
                    var fullSizeName = product.FullSizeImageName;
                    var blobStorage = new BlobMethods(storageAccountName, storageAccountKey, containerName);
                    await blobStorage.DeleteBlobAsync(thumbnailName);
                    await blobStorage.DeleteBlobAsync(fullSizeName);
                }
                db.Products.Remove(product);
            }
            await db.SaveChangesAsync();
            return View();
        }
    }
}