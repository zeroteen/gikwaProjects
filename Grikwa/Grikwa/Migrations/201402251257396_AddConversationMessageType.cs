namespace Grikwa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConversationMessageType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Conversations", "MessageType", c => c.Int(nullable: false,defaultValue:0));

            Sql("UPDATE dbo.Conversations SET MessageType = 1 WHERE Text LIKE '(Regarding product: %'");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Conversations", "MessageType");
        }
    }
}
