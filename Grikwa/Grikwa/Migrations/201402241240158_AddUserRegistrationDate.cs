namespace Grikwa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserRegistrationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "RegistrationDate", c => c.DateTime(nullable:false,defaultValue:new DateTime(2014,2,1)));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "RegistrationDate");
        }
    }
}
