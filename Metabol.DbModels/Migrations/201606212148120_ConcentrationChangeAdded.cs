using System.Data.Entity.Migrations;

namespace Metabol.DbModels.Migrations
{
    public partial class ConcentrationChangeAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConcentrationChanges",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Change = c.Int(nullable: false),
                        Value = c.Double(nullable: false),
                        AnalysisModels_AnalysisId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnalysisModels", t => t.AnalysisModels_AnalysisId)
                .Index(t => t.AnalysisModels_AnalysisId);
            
            AlterColumn("dbo.AnalysisModels", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.AnalysisModels", "SessionKey", c => c.String(nullable: false));
            DropColumn("dbo.AnalysisModels", "LastIteration");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AnalysisModels", "LastIteration", c => c.Int(nullable: false));
            DropForeignKey("dbo.ConcentrationChanges", "AnalysisModels_AnalysisId", "dbo.AnalysisModels");
            DropIndex("dbo.ConcentrationChanges", new[] { "AnalysisModels_AnalysisId" });
            AlterColumn("dbo.AnalysisModels", "SessionKey", c => c.String());
            AlterColumn("dbo.AnalysisModels", "Name", c => c.String());
            DropTable("dbo.ConcentrationChanges");
        }
    }
}
