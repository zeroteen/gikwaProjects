namespace Grikwa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInstitutionCompanyTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Companies", "LocationID", "dbo.Locations");
            DropIndex("dbo.Companies", new[] { "LocationID" });
            CreateTable(
                "dbo.InstitutionCompanies",
                c => new
                    {
                        InstitutionCompanyID = c.Int(nullable: false, identity: true),
                        CompanyID = c.Int(nullable: false),
                        InstitutionID = c.Int(nullable: false),
                        LocationID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.InstitutionCompanyID)
                .ForeignKey("dbo.Companies", t => t.CompanyID, cascadeDelete: true)
                .ForeignKey("dbo.Institutions", t => t.InstitutionID, cascadeDelete: true)
                .ForeignKey("dbo.Locations", t => t.LocationID)
                .Index(t => t.CompanyID)
                .Index(t => t.InstitutionID)
                .Index(t => t.LocationID);
            
            DropColumn("dbo.Companies", "LocationID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Companies", "LocationID", c => c.String(maxLength: 128));
            DropForeignKey("dbo.InstitutionCompanies", "LocationID", "dbo.Locations");
            DropForeignKey("dbo.InstitutionCompanies", "InstitutionID", "dbo.Institutions");
            DropForeignKey("dbo.InstitutionCompanies", "CompanyID", "dbo.Companies");
            DropIndex("dbo.InstitutionCompanies", new[] { "LocationID" });
            DropIndex("dbo.InstitutionCompanies", new[] { "InstitutionID" });
            DropIndex("dbo.InstitutionCompanies", new[] { "CompanyID" });
            DropTable("dbo.InstitutionCompanies");
            CreateIndex("dbo.Companies", "LocationID");
            AddForeignKey("dbo.Companies", "LocationID", "dbo.Locations", "LocationID");
        }
    }
}
