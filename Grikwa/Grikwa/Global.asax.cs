using Grikwa.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
//using Microsoft.AspNet.SignalR;

namespace Grikwa
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //RouteTable.Routes.MapHubs();
            //GlobalHost.
            
            AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }

    public class GrikwaStoreDBInitializer
        : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {

            // fill institutions
            context.Institutions.Add(new Institution()
                                    {
                                        Name = "University of Cape Town",
                                        abbreviation = "UCT",
                                        Extension1 = "myuct.ac.za"
                                    });
            context.Institutions.Add(new Institution()
                                    {   
                                        Name = "University of Western Cape",
                                        abbreviation = "UWC",
                                        Extension1 = "myuwc.ac.za"
                                    });
            context.Institutions.Add(new Institution()
            {
                Name = "Cape Peninsula University of Technology",
                abbreviation = "CPUT",
                Extension1 = "mycput.ac.za"
            });

            // fill faculties
            context.Faculties.Add(new Faculty() { Name = "Science" });
            context.Faculties.Add(new Faculty() {Name = "Commerce" });
            context.Faculties.Add(new Faculty() { Name = "Law" });
            context.Faculties.Add(new Faculty() { Name = "Engineering" });

            // fill qualifications
            context.Qualifications.Add(new Qualification() { Code = "bsc", FullName = "Bachelor of Science" });
            context.Qualifications.Add(new Qualification() { Code = "beng", FullName = "Bachelor of Engineering" });
            context.Qualifications.Add(new Qualification() { Code = "bcom", FullName = "Bachelor of Commerce" });
            context.Qualifications.Add(new Qualification() { Code = "llb", FullName = "Bachelor of Law" });

            // fill majors
            context.Majors.Add(new Major() { Name = "Computer Science" });
            context.Majors.Add(new Major() { Name = "Information Systems" });
            context.Majors.Add(new Major() { Name = "3D Game Design" });
            context.Majors.Add(new Major() { Name = "Mathematics" });
            context.Majors.Add(new Major() { Name = "Applied Mathematics" });
            context.Majors.Add(new Major() { Name = "Statistics" });
            context.Majors.Add(new Major() { Name = "Physical Science" });

            // fill residents
            context.Residents.Add(new Resident() { Name = "Leo Marquard Hall"});
            context.Residents.Add(new Resident() { Name = "Liesbeeck Gardens"});
            context.Residents.Add(new Resident() { Name = "Forest Hill"});
            context.Residents.Add(new Resident() { Name = "Tugwel"});
            context.Residents.Add(new Resident() { Name = "Kopano"});
            context.Residents.Add(new Resident() { Name = "Baxter Hall"});
            context.Residents.Add(new Resident() { Name = "College House"});
            context.Residents.Add(new Resident() { Name = "University House"});

            // fill titles
            context.Titles.Add(new Title() { TitleID = "Mr" });
            context.Titles.Add(new Title() { TitleID = "Miss" });
            context.Titles.Add(new Title() { TitleID = "Mrs" });
            context.Titles.Add(new Title() { TitleID = "Ms" });
            context.Titles.Add(new Title() { TitleID = "Prof" });
            context.Titles.Add(new Title() { TitleID = "Dr" });

            // fill categories
            context.Categories.Add(new Category() { Name = "Fashion" });
            context.Categories.Add(new Category() { Name = "Electronics" });
            context.Categories.Add(new Category() { Name = "Service" });

            // fill 
            
            // fill users
            var user = new ApplicationUser()
                                        {
                                            UserName = "mplmas002",
                                            Email = "mplmas002@myuct.ac.za",
                                            Faculty = new Faculty() { Name = "Humanity" },
                                            Intials = "MB",
                                            //PasswordHash = "123456",
                                            Surname = "Mapaila",
                                            Title = new Title() { TitleID = "Hr", Description = "Hero" },
                                            Qualification = new Qualification() { Code = "BA", FullName = "Bachelor in Arts" },
                                            //InstitutionResident = new Institution_Resident()
                                            //{
                                                Institution = new Institution() { abbreviation = "wits", Name = "Wits University", Extension1 = "wits.ac.za" },
                                                Resident = new Resident() { Name = "Woolsack Resident" }
                                            //}
                                        };

            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            UserManager.Create(user, "springs35");

            // link some institutions and faculties
            Institution_Faculty InF = new Institution_Faculty()
                                            {
                                                Faculty = new Faculty() { Name = "Information System" },
                                                Institution = new Institution { abbreviation = "up", Extension1 = "myup.ac.za", Name = "University Of Pretoria" }
                                            };
            //context.Institution_Faculties.Add(InF);
            

            // link some institutions, faculties and qualifications
            Institution_Faculty_Qualification IFQ = new Institution_Faculty_Qualification()
                                                            {
                                                                Institution_Faculty = InF,
                                                                Qualification = new Qualification() {Code = "PhD", FullName = "Doctor of Philosophy"}
                                                            };
            //context.Institution_Faculty_Qualifications.Add(IFQ);

            // link some institutions, faculties, qualifications and majors
            context.Institution_Faculty_Qualification_Majors.Add(new Institution_Faculty_Qualification_Major()
                                                                {
                                                                    Institution_Faculty_Qualification = IFQ,
                                                                    Major = new Major() {Name = "Applied Mathematics"}
                                                                });

            base.Seed(context);
        }
    }
}
