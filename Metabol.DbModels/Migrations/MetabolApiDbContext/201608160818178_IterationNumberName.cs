namespace Metabol.DbModels.Migrations.MetabolApiDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IterationNumberName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IterationModels", "IterationNumber", c => c.Int(nullable: false));
            DropColumn("dbo.IterationModels", "IterantionNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IterationModels", "IterantionNumber", c => c.Int(nullable: false));
            DropColumn("dbo.IterationModels", "IterationNumber");
        }
    }
}
