using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
//using System.ComponentModel.DataAnnotations;

namespace Grikwa.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        [MaxLength(6)]
        public string Intials { get; set; }

        [MaxLength(50)]
        public string Surname { get; set; }

        [MaxLength(10)]
        public string TitleID { get; set; }
        public Title Title { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string OptionalEmail { get; set; }

        public int QualificationID { get; set; }
        public Qualification Qualification { get; set; }

        public int InstitutionID { get; set; }
        public Institution Institution { get; set; }

        public int FacultyID { get; set; }
        public Faculty Faculty { get; set; }

        public int ResidentID { get; set; }
        public Resident Resident { get; set; }

        public bool Verified { get; set; }

        public bool AcceptedTerms { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime LastSeen { get; set; }

        public virtual byte[] Picture { get; set; }
    }

    public class UserMajor
    {
        public virtual int UserMajorID { get; set; }
        public virtual string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual int MajorID { get; set; }
        public virtual Major Major { get; set; }
    }

    public class Token
    {
        [Key]
        public string Id { get; set; }
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime IssueDate { get; set; }
    }

    public class Title
    {
        public string TitleID { get; set; }
        public string Description { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("Grikwa")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Grikwa.Migrations.Configuration>("Grikwa"));
        }

        #region DB Table Names
        
        public DbSet<Institution> Institutions { get; set; }

        public DbSet<MeetPlace> MeetPlaces { get; set; }

        public DbSet<Faculty> Faculties { get; set; }

        public DbSet<Major> Majors { get; set; }

        public DbSet<Qualification> Qualifications { get; set; }

        public DbSet<Resident> Residents { get; set; }

        public DbSet<Title> Titles { get; set; }

        public DbSet<Token> Tokens { get; set; }

        public DbSet<ConversationRoom> ConversationRooms { get; set; }

        public DbSet<ConversationRoomProduct> ConversationRoomProducts { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<UserMajor> UserMajors { get; set; }

        public DbSet<Institution_Faculty> Institution_Faculties { get; set; }

        public DbSet<Institution_Faculty_Qualification> Institution_Faculty_Qualifications { get; set; }

        public DbSet<Institution_Faculty_Qualification_Major> Institution_Faculty_Qualification_Majors { get; set; }

        public DbSet<Institution_Resident> Institution_Residents { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<InstitutionCompany> InstitutionCompanies { get; set; }

        public DbSet<CompanyEmployee> CompanyEmployees { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<Unit> Units { get; set; }

        public DbSet<ItemAdjustment> ItemAdjustments { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<CompanySale> CompanySales { get; set; }

        public DbSet<SlotSetting> SlotSettings { get; set; }

        public DbSet<CompanySlotSetting> CompanySlotSettings { get; set; }

        public DbSet<CompanyItem> CompanyItems { get; set; }

        #endregion
    }
}