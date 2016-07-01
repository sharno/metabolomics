using System.Data.Entity.Migrations;

namespace Metabol.DbModels.Migrations
{
    public partial class RevertBack : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IterationModels", "NodesJson", c => c.String());
            AddColumn("dbo.IterationModels", "LinksJson", c => c.String());
            DropColumn("dbo.IterationModels", "MetabolicNetworkJson");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IterationModels", "MetabolicNetworkJson", c => c.String());
            DropColumn("dbo.IterationModels", "LinksJson");
            DropColumn("dbo.IterationModels", "NodesJson");
        }
    }
}
