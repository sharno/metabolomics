using System.Data.Entity.Migrations;

namespace Metabol.DbModels.Migrations
{
    public partial class UnknownChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.IterationModels", "AnalysisModels_SessionKey", "dbo.AnalysisModels");
            DropIndex("dbo.IterationModels", new[] { "AnalysisModels_SessionKey" });
            DropTable("dbo.IterationModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.IterationModels",
                c => new
                    {
                        IterationId = c.Guid(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        Fba = c.Int(nullable: false),
                        Time = c.Double(nullable: false),
                        NodesJson = c.String(),
                        LinksJson = c.String(),
                        AnalysisModels_SessionKey = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.IterationId);
            
            CreateIndex("dbo.IterationModels", "AnalysisModels_SessionKey");
            AddForeignKey("dbo.IterationModels", "AnalysisModels_SessionKey", "dbo.AnalysisModels", "SessionKey");
        }
    }
}
