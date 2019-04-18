namespace ReaderDataCollector.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Readers",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Host = c.String(),
                        Port = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Readings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ReaderNumber = c.String(),
                        IPAddress = c.String(),
                        TimingPoint = c.String(),
                        TotalReads = c.Int(nullable: false),
                        FileName = c.String(),
                        StartedDateTime = c.DateTime(),
                        EndedDateTime = c.DateTime(),
                        ReaderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Readers", t => t.ReaderId, cascadeDelete: true)
                .Index(t => t.ReaderId);
            
            CreateTable(
                "dbo.Reads",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EPC = c.String(),
                        Time = c.DateTime(nullable: false),
                        Signal = c.String(),
                        AntennaNumber = c.String(),
                        SeenCount = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                        ReadingId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Readings", t => t.ReadingId, cascadeDelete: true)
                .Index(t => t.ReadingId);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reads", "ReadingId", "dbo.Readings");
            DropForeignKey("dbo.Readings", "ReaderId", "dbo.Readers");
            DropIndex("dbo.Reads", new[] { "ReadingId" });
            DropIndex("dbo.Readings", new[] { "ReaderId" });
            DropTable("dbo.Settings");
            DropTable("dbo.Reads");
            DropTable("dbo.Readings");
            DropTable("dbo.Readers");
        }
    }
}
