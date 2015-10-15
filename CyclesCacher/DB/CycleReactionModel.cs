namespace CyclesCacher.DB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CycleReactionModel : DbContext
    {
        public CycleReactionModel()
            : base("name=CycleReactionModel1")
        {
        }

        public virtual DbSet<Cycle> Cycles { get; set; }
        public virtual DbSet<CycleReaction> CycleReactions { get; set; }
        public virtual DbSet<Reaction> Reactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cycle>()
                .HasMany(e => e.CycleReactions)
                .WithRequired(e => e.Cycle)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reaction>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<Reaction>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Reaction>()
                .HasMany(e => e.CycleReactions)
                .WithRequired(e => e.Reaction)
                .WillCascadeOnDelete(false);
        }
    }
}
