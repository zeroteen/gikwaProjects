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

namespace Grikwa.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            var institutions = from i in db.Institutions
                               select new InstitutionViewModel()
                               {
                                   Name = i.Name,
                                   Code = i.abbreviation
                               };

            return View(await institutions.ToListAsync());
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
                var image = from i in db.Institutions
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
        public ActionResult Contact([Bind(Include="From,Name,Message")] ContactUsModel model)
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
                                  + "<p><strong>Name</strong>: "+Name+"</p>"
                                  + "<p><strong>Email Address</strong>: "+From+"</p>"
                                  + "<p><strong>Message</strong>: "+Message+"</p>"
                                  + " <br/> <h5>Grikwa Team</h5>";
                string bodyText = "Dear Grikwa Stuuf. "
                                  + "Contact Us Message. Name:" + Name + ". Email Address: " + From+ ". Message: " + Message
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
                Trace.WriteLine(smtpFailedRecipientException.Message, "Contact Us Email From : " + From +" failed.");
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

        public ActionResult PopulateDatabase()
        {

            //// fill institutions
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
            //db.Faculties.Add(new Faculty() {Name = "Commerce" });
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
            //db.Residents.Add(new Resident() { Name = "Forest Hill"});
            //db.Residents.Add(new Resident() { Name = "Tugwel" });
            //db.Residents.Add(new Resident() { Name = "Kopano"});
            //db.Residents.Add(new Resident() { Name = "Baxter Hall" });
            //db.Residents.Add(new Resident() { Name = "College House" });
            //db.Residents.Add(new Resident() { Name = "University House" });

            ////// fill titles
            //db.Titles.Add(new Title() { TitleID = "Mr", Description="A man" });
            //db.Titles.Add(new Title() { TitleID = "Miss", Description="Not marries woman" });
            //db.Titles.Add(new Title() { TitleID = "Mrs" , Description="Married woman"});
            //db.Titles.Add(new Title() { TitleID = "Ms", Description="Not married anymore woman" });
            //db.Titles.Add(new Title() { TitleID = "Prof", Description="A professor" });
            //db.Titles.Add(new Title() { TitleID = "Dr", Description="A doctor" });

            //// fill categories
            //db.Categories.Add(new Category() { Name = "Textbooks", IconName = "icon-book", Code = "textbooks" });
            //db.Categories.Add(new Category() { Name = "Electronics", IconName = "icon-cogs", Code = "electronics" });
            //db.Categories.Add(new Category() { Name = "Kitchen and Lifestyle", IconName = "icon-food", Code = "k&l" });
            //db.Categories.Add(new Category() { Name = "Accommodation", IconName = "icon-map-marker", Code = "accommodation" });
            //db.Categories.Add(new Category() { Name = "Services", IconName = "icon-male", Code = "services" });
            //db.Categories.Add(new Category() { Name = "Other", IconName = "icon-star", Code = "other" });

            //db.SaveChangesAsync();

            //var user = new ApplicationUser()
            //{
            //    UserName = "mplmas009",
            //    Email = "mplmas009@myuct.ac.za",
            //    Faculty = new Faculty() { Name = "Humanity" },
            //    Intials = "MB",
            //    Surname = "Mapaila",
            //    Title = new Title() { TitleID = "Sir", Description = "A Sir" },
            //    Qualification = new Qualification() { Code = "BA", FullName = "Bachelor in Arts" },
            //    Institution = new Institution() { abbreviation = "Stellies", Name = "Stellenbosch University", Extension1 = "sun.ac.za" },
            //    Resident = new Resident() { Name = "Woolsack Resident" },
            //    Verified = true
            //};

            //UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            
            
            //UserManager.Create(user, "springs35");
            ////UserManager.CreateIdentity(user, "admin");

            return View();
        }
    }
}