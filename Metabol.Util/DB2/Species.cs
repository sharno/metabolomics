namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Species
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Species()
        {
            MapSpeciesMolecularEntities = new HashSet<MapSpeciesMolecularEntity>();
            MetaboliteReactionCounts = new HashSet<MetaboliteReactionCount>();
            MetaboliteReactionStoichiometries = new HashSet<MetaboliteReactionStoichiometry>();
            ReactionSpecies = new HashSet<ReactionSpecy>();
            CycleConnections = new HashSet<CycleConnection>();
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
        public virtual ICollection<MapSpeciesMolecularEntity> MapSpeciesMolecularEntities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MetaboliteReactionCount> MetaboliteReactionCounts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MetaboliteReactionStoichiometry> MetaboliteReactionStoichiometries { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReactionSpecy> ReactionSpecies { get; set; }

        public virtual Sbase Sbase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CycleConnection> CycleConnections { get; set; }

        public virtual SpeciesType SpeciesType { get; set; }

        public virtual UnitDefinition UnitDefinition { get; set; }
    }
}
