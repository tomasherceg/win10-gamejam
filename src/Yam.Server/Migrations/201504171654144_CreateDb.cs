namespace Yam.Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorldChanges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorldId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        X = c.Int(nullable: false),
                        Y = c.Int(nullable: false),
                        Z = c.Int(nullable: false),
                        TextureId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Worlds", t => t.WorldId, cascadeDelete: true)
                .Index(t => t.WorldId);
            
            CreateTable(
                "dbo.Worlds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorldChanges", "WorldId", "dbo.Worlds");
            DropIndex("dbo.WorldChanges", new[] { "WorldId" });
            DropTable("dbo.Worlds");
            DropTable("dbo.WorldChanges");
        }
    }
}
