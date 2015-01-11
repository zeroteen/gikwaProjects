namespace Grikwa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFoodStoreTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Telephone = c.String(),
                        AdditionalTelephone = c.String(),
                        LocationID = c.String(maxLength: 128),
                        Logo = c.Binary(),
                        Background = c.Binary(),
                        VisibilitySatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyID)
                .ForeignKey("dbo.Locations", t => t.LocationID)
                .Index(t => t.LocationID);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.LocationID);
            
            CreateTable(
                "dbo.CompanyEmployees",
                c => new
                    {
                        CompanyEmployeeID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.String(maxLength: 128),
                        CompanyID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyEmployeeID)
                .ForeignKey("dbo.Companies", t => t.CompanyID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.EmployeeID)
                .Index(t => t.CompanyID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.CompanyItems",
                c => new
                    {
                        CompanyItemID = c.Int(nullable: false, identity: true),
                        CompanyID = c.Int(nullable: false),
                        ItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyItemID)
                .ForeignKey("dbo.Companies", t => t.CompanyID, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.ItemID, cascadeDelete: true)
                .Index(t => t.CompanyID)
                .Index(t => t.ItemID);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ItemID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ShortDescription = c.String(),
                        LongDescription = c.String(),
                        Price = c.Double(nullable: false),
                        UnitID = c.String(maxLength: 128),
                        ItemImage = c.Binary(),
                        ItemImage2 = c.Binary(),
                        ItemImage3 = c.Binary(),
                        VisibilitySatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ItemID)
                .ForeignKey("dbo.Units", t => t.UnitID)
                .Index(t => t.UnitID);
            
            CreateTable(
                "dbo.Units",
                c => new
                    {
                        UnitID = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.UnitID);
            
            CreateTable(
                "dbo.CompanySales",
                c => new
                    {
                        CompanySaleID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.String(maxLength: 128),
                        OrderID = c.Int(nullable: false),
                        SaleTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CompanySaleID)
                .ForeignKey("dbo.AspNetUsers", t => t.EmployeeID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.EmployeeID)
                .Index(t => t.OrderID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        OrderNumber = c.String(),
                        UserID = c.String(maxLength: 128),
                        CompanyID = c.Int(nullable: false),
                        OrderTime = c.DateTime(nullable: false),
                        CollectionTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Companies", t => t.CompanyID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.CompanyID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.CompanySlotSettings",
                c => new
                    {
                        CompanySlotSettingID = c.Int(nullable: false, identity: true),
                        CompanyID = c.Int(nullable: false),
                        SlotSettingID = c.Int(nullable: false),
                        ActiveDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CompanySlotSettingID)
                .ForeignKey("dbo.Companies", t => t.CompanyID, cascadeDelete: true)
                .ForeignKey("dbo.SlotSettings", t => t.SlotSettingID, cascadeDelete: true)
                .Index(t => t.CompanyID)
                .Index(t => t.SlotSettingID);
            
            CreateTable(
                "dbo.SlotSettings",
                c => new
                    {
                        SlotSettingID = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        MinutesRange = c.Int(nullable: false),
                        MaxQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SlotSettingID);
            
            CreateTable(
                "dbo.ItemAdjustments",
                c => new
                    {
                        ItemAdjustmentID = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        Discount = c.Single(nullable: false),
                        ItemID = c.Int(nullable: false),
                        EmployeeID = c.String(maxLength: 128),
                        Reason = c.String(),
                        EffectDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ItemAdjustmentID)
                .ForeignKey("dbo.AspNetUsers", t => t.EmployeeID)
                .ForeignKey("dbo.Items", t => t.ItemID, cascadeDelete: true)
                .Index(t => t.EmployeeID)
                .Index(t => t.ItemID);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        OrderItemID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        ItemID = c.Int(nullable: false),
                        Discount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderItemID)
                .ForeignKey("dbo.Items", t => t.ItemID, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.ItemID)
                .Index(t => t.OrderID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.OrderItems", "ItemID", "dbo.Items");
            DropForeignKey("dbo.ItemAdjustments", "ItemID", "dbo.Items");
            DropForeignKey("dbo.ItemAdjustments", "EmployeeID", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompanySlotSettings", "SlotSettingID", "dbo.SlotSettings");
            DropForeignKey("dbo.CompanySlotSettings", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.CompanySales", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Orders", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Orders", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.CompanySales", "EmployeeID", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompanyItems", "ItemID", "dbo.Items");
            DropForeignKey("dbo.Items", "UnitID", "dbo.Units");
            DropForeignKey("dbo.CompanyItems", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.CompanyEmployees", "EmployeeID", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompanyEmployees", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.Companies", "LocationID", "dbo.Locations");
            DropIndex("dbo.OrderItems", new[] { "OrderID" });
            DropIndex("dbo.OrderItems", new[] { "ItemID" });
            DropIndex("dbo.ItemAdjustments", new[] { "ItemID" });
            DropIndex("dbo.ItemAdjustments", new[] { "EmployeeID" });
            DropIndex("dbo.CompanySlotSettings", new[] { "SlotSettingID" });
            DropIndex("dbo.CompanySlotSettings", new[] { "CompanyID" });
            DropIndex("dbo.CompanySales", new[] { "OrderID" });
            DropIndex("dbo.Orders", new[] { "UserID" });
            DropIndex("dbo.Orders", new[] { "CompanyID" });
            DropIndex("dbo.CompanySales", new[] { "EmployeeID" });
            DropIndex("dbo.CompanyItems", new[] { "ItemID" });
            DropIndex("dbo.Items", new[] { "UnitID" });
            DropIndex("dbo.CompanyItems", new[] { "CompanyID" });
            DropIndex("dbo.CompanyEmployees", new[] { "EmployeeID" });
            DropIndex("dbo.CompanyEmployees", new[] { "CompanyID" });
            DropIndex("dbo.Companies", new[] { "LocationID" });
            DropTable("dbo.OrderItems");
            DropTable("dbo.ItemAdjustments");
            DropTable("dbo.SlotSettings");
            DropTable("dbo.CompanySlotSettings");
            DropTable("dbo.Orders");
            DropTable("dbo.CompanySales");
            DropTable("dbo.Units");
            DropTable("dbo.Items");
            DropTable("dbo.CompanyItems");
            DropTable("dbo.CompanyEmployees");
            DropTable("dbo.Locations");
            DropTable("dbo.Companies");
        }
    }
}
