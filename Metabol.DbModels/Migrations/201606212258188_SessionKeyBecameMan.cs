using System.Data.Entity.Migrations;

namespace Metabol.DbModels.Migrations
{
    public partial class SessionKeyBecameMan : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ConcentrationChanges", "AnalysisModels_AnalysisId", "dbo.AnalysisModels");
            DropForeignKey("dbo.IterationModels", "AnalysisModels_AnalysisId", "dbo.AnalysisModels");
            DropIndex("dbo.ConcentrationChanges", new[] { "AnalysisModels_AnalysisId" });
            DropIndex("dbo.IterationModels", new[] { "AnalysisModels_AnalysisId" });
            RenameColumn(table: "dbo.ConcentrationChanges", name: "AnalysisModels_AnalysisId", newName: "AnalysisModels_SessionKey");
            RenameColumn(table: "dbo.IterationModels", name: "AnalysisModels_AnalysisId", newName: "AnalysisModels_SessionKey");
            DropPrimaryKey("dbo.AnalysisModels");
            AlterColumn("dbo.AnalysisModels", "SessionKey", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ConcentrationChanges", "AnalysisModels_SessionKey", c => c.String(maxLength: 128));
            AlterColumn("dbo.IterationModels", "AnalysisModels_SessionKey", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.AnalysisModels", "SessionKey");
            CreateIndex("dbo.ConcentrationChanges", "AnalysisModels_SessionKey");
            CreateIndex("dbo.IterationModels", "AnalysisModels_SessionKey");
            AddForeignKey("dbo.ConcentrationChanges", "AnalysisModels_SessionKey", "dbo.AnalysisModels", "SessionKey");
            AddForeignKey("dbo.IterationModels", "AnalysisModels_SessionKey", "dbo.AnalysisModels", "SessionKey");
            DropColumn("dbo.AnalysisModels", "AnalysisId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AnalysisModels", "AnalysisId", c => c.Guid(nullable: false, identity: true));
            DropForeignKey("dbo.IterationModels", "AnalysisModels_SessionKey", "dbo.AnalysisModels");
            DropForeignKey("dbo.ConcentrationChanges", "AnalysisModels_SessionKey", "dbo.AnalysisModels");
            DropIndex("dbo.IterationModels", new[] { "AnalysisModels_SessionKey" });
            DropIndex("dbo.ConcentrationChanges", new[] { "AnalysisModels_SessionKey" });
            DropPrimaryKey("dbo.AnalysisModels");
            AlterColumn("dbo.IterationModels", "AnalysisModels_SessionKey", c => c.Guid());
            AlterColumn("dbo.ConcentrationChanges", "AnalysisModels_SessionKey", c => c.Guid());
            AlterColumn("dbo.AnalysisModels", "SessionKey", c => c.String(nullable: false));
            AddPrimaryKey("dbo.AnalysisModels", "AnalysisId");
            RenameColumn(table: "dbo.IterationModels", name: "AnalysisModels_SessionKey", newName: "AnalysisModels_AnalysisId");
            RenameColumn(table: "dbo.ConcentrationChanges", name: "AnalysisModels_SessionKey", newName: "AnalysisModels_AnalysisId");
            CreateIndex("dbo.IterationModels", "AnalysisModels_AnalysisId");
            CreateIndex("dbo.ConcentrationChanges", "AnalysisModels_AnalysisId");
            AddForeignKey("dbo.IterationModels", "AnalysisModels_AnalysisId", "dbo.AnalysisModels", "AnalysisId");
            AddForeignKey("dbo.ConcentrationChanges", "AnalysisModels_AnalysisId", "dbo.AnalysisModels", "AnalysisId");
        }
    }
}
