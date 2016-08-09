namespace Metabol.DbModels.Migrations.MetabolApiDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IterationNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IterationModels", "IterantionNumber", c => c.Int(nullable: false));
            DropColumn("dbo.IterationModels", "IterationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IterationModels", "IterationId", c => c.Guid(nullable: false));
            DropColumn("dbo.IterationModels", "IterantionNumber");
        }
    }
}
