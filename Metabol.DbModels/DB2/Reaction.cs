using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    [Table("Reaction")]
    public partial class Reaction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reaction()
        {
            MapReactionECNumber = new HashSet<MapReactionECNumber>();
            MapReactionsProcessEntities = new HashSet<MapReactionsProcessEntities>();
            FixedReactions = new HashSet<FixedReactions>();
            ReactionBound = new HashSet<ReactionBound>();
            ReactionSpecies = new HashSet<ReactionSpecies>();
        }

        public Guid id { get; set; }

        public Guid modelId { get; set; }

        public string sbmlId { get; set; }

        public string name { get; set; }

        public bool reversible { get; set; }

        public bool fast { get; set; }

        public Guid? kineticLawId { get; set; }

        public virtual KineticLaw KineticLaw { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapReactionECNumber> MapReactionECNumber { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapReactionsProcessEntities> MapReactionsProcessEntities { get; set; }

        public virtual Model Model { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FixedReactions> FixedReactions { get; set; }

        public virtual Sbase Sbase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReactionBound> ReactionBound { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReactionSpecies> ReactionSpecies { get; set; }
    }
}
