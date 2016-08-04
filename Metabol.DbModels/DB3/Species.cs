using System;
using System.Collections.Generic;

namespace Metabol.DbModels.DB3
{
    public partial class Species
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Species()
        {
            Formula = new HashSet<Formula>();
            MapSpeciesMolecularEntities = new HashSet<MapSpeciesMolecularEntities>();
            MetaboliteReactionCount = new HashSet<MetaboliteReactionCount>();
            MetaboliteReactionStoichiometry = new HashSet<MetaboliteReactionStoichiometry>();
            ReactionSpecies = new HashSet<ReactionSpecies>();
            CycleConnection = new HashSet<CycleConnection>();
        }

        public Guid id { get; set; }

        public Guid modelId { get; set; }

        public string sbmlId { get; set; }

        public string name { get; set; }

        public Guid? speciesTypeId { get; set; }

        public Guid compartmentId { get; set; }

        public double? initialAmount { get; set; }

        public double? initialConcentration { get; set; }

        public Guid? substanceUnitsId { get; set; }

        public bool hasOnlySubstanceUnits { get; set; }

        public bool boundaryCondition { get; set; }

        public int? charge { get; set; }

        public bool constant { get; set; }

        public virtual Compartment Compartment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Formula> Formula { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapSpeciesMolecularEntities> MapSpeciesMolecularEntities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MetaboliteReactionCount> MetaboliteReactionCount { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MetaboliteReactionStoichiometry> MetaboliteReactionStoichiometry { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReactionSpecies> ReactionSpecies { get; set; }

        public virtual Sbase Sbase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CycleConnection> CycleConnection { get; set; }

        public virtual SpeciesType SpeciesType { get; set; }

        public virtual UnitDefinition UnitDefinition { get; set; }
    }
}
