namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ReactionSpecy
    {
        public Guid id { get; set; }

        public Guid reactionId { get; set; }

        public Guid speciesId { get; set; }

        public byte roleId { get; set; }

        public double stoichiometry { get; set; }

        public Guid? stoichiometryMathId { get; set; }

        [StringLength(500)]
        public string sbmlId { get; set; }

        public string name { get; set; }

        public virtual Reaction Reaction { get; set; }

        public virtual ReactionSpeciesRole ReactionSpeciesRole { get; set; }

        public virtual Sbase Sbase { get; set; }

        public virtual Species Species { get; set; }

        public virtual StoichiometryMath StoichiometryMath { get; set; }
    }
}
