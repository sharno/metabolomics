namespace Metabol.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnalysisAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Analyses",
                c => new
                    {
                        AnalysisId = c.Guid(nullable: false, identity: true),
                        LastIteration = c.Int(nullable: false),
                        Name = c.String(),
                        SessionKey = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AnalysisId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.AspNetUsers", "Affiliation", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Analyses", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Analyses", new[] { "User_Id" });
            DropColumn("dbo.AspNetUsers", "Affiliation");
            DropTable("dbo.Analyses");
        }
    }
}
