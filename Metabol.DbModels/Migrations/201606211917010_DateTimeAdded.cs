using System.Data.Entity.Migrations;

namespace Metabol.DbModels.Migrations
{
    public partial class DateTimeAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AnalysisModels", "DateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AnalysisModels", "DateTime");
        }
    }
}
