using System.Data.Entity.Migrations;

namespace Metabol.DbModels.Migrations
{
    public partial class BringBackIterationModel : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.IterationId)
                .ForeignKey("dbo.AnalysisModels", t => t.AnalysisModels_SessionKey)
                .Index(t => t.AnalysisModels_SessionKey);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IterationModels", "AnalysisModels_SessionKey", "dbo.AnalysisModels");
            DropIndex("dbo.IterationModels", new[] { "AnalysisModels_SessionKey" });
            DropTable("dbo.IterationModels");
        }
    }
}
