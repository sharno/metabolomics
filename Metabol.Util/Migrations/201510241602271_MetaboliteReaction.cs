namespace Metabol.Util.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class MetaboliteReaction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MetaboliteReactionCount",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        speciesId = c.Guid(nullable: false),
                        consumerCount = c.Int(nullable: false),
                        producerCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Species", t => t.speciesId, cascadeDelete: true)
                .Index(t => t.speciesId);

            CreateTable(
                "dbo.MetaboliteReactionStoichiometry",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        speciesId = c.Guid(nullable: false),
                        consumerStoch = c.Double(nullable: false),
                        producerStoch = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Species", t => t.speciesId, cascadeDelete: true)
                .Index(t => t.speciesId);
        }

        public override void Down()
        {
        }
    }
}
