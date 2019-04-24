namespace ReaderDataCollector.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReadandReadingModelChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Readings", "RaceName", c => c.String());
            AddColumn("dbo.Readings", "UserName", c => c.String());
            DropColumn("dbo.Reads", "SeenCount");
            DropColumn("dbo.Reads", "Rank");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reads", "Rank", c => c.Int(nullable: false));
            AddColumn("dbo.Reads", "SeenCount", c => c.Int(nullable: false));
            DropColumn("dbo.Readings", "UserName");
            DropColumn("dbo.Readings", "RaceName");
        }
    }
}
