namespace CyclesCacher
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CycleReactionModel : DbContext
    {
        public CycleReactionModel()
            : base("name=CycleReaction")
        {
        }

        public virtual DbSet<CycleModel> Cycles { get; set; }
        public virtual DbSet<Reaction> Reactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reaction>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<Reaction>()
                .Property(e => e.name)
                .IsUnicode(false);
        }
    }
}
