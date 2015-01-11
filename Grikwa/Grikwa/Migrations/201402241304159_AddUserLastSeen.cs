namespace Grikwa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserLastSeen : DbMigration
    {
        public override void Up()
        {
            double defaultLastSeenDays = 1*(-1);
            DateTime time = DateTime.Now.AddDays(defaultLastSeenDays);
            AddColumn("dbo.AspNetUsers", "LastSeen", c => c.DateTime(nullable:false, defaultValue: new DateTime(time.Year,time.Month,time.Day)));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LastSeen");
        }
    }
}
