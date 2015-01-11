namespace Grikwa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItemType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "ItemType", c => c.Int(nullable: false, defaultValue : 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "ItemType");
        }
    }
}
