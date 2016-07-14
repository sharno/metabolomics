using System.Data.Entity.Migrations;

namespace Metabol.DbModels.Migrations
{
    public partial class IterationFluxesFixed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IterationModels", "FluxesJson", c => c.String());
            AddColumn("dbo.IterationModels", "ConstraintsJson", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.IterationModels", "ConstraintsJson");
            DropColumn("dbo.IterationModels", "FluxesJson");
        }
    }
}
