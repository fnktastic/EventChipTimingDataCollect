namespace ReaderDataCollector.Migrations
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
                        ID = c.Int(nullable: false, identity: true),
                        Host = c.String(),
                        Port = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Readings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TotalReadings = c.Int(nullable: false),
                        TimingPoint = c.String(),
                        FileName = c.String(),
                        StartedDateTime = c.DateTime(),
                        EndedDateTime = c.DateTime(),
                        ReaderID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Readers", t => t.ReaderID, cascadeDelete: true)
                .Index(t => t.ReaderID);
            
            CreateTable(
                "dbo.Reads",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EPC = c.String(),
                        Time = c.DateTime(nullable: false),
                        PeakRssiInDbm = c.String(),
                        AntennaNumber = c.String(),
                        ReaderNumber = c.String(),
                        IpAddress = c.String(),
                        UniqueReadID = c.String(),
                        TimingPoint = c.String(),
                        SeenCount = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                        ReadingID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Readings", t => t.ReadingID, cascadeDelete: true)
                .Index(t => t.ReadingID);
            
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
            DropForeignKey("dbo.Reads", "ReadingID", "dbo.Readings");
            DropForeignKey("dbo.Readings", "ReaderID", "dbo.Readers");
            DropIndex("dbo.Reads", new[] { "ReadingID" });
            DropIndex("dbo.Readings", new[] { "ReaderID" });
            DropTable("dbo.Settings");
            DropTable("dbo.Reads");
            DropTable("dbo.Readings");
            DropTable("dbo.Readers");
        }
    }
}
