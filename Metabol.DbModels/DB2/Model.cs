using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    [Table("Model")]
    public partial class Model
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Model()
        {
            Compartment = new HashSet<Compartment>();
            CompartmentType = new HashSet<CompartmentType>();
            Constraint = new HashSet<Constraint>();
            Event = new HashSet<Event>();
            FunctionDefinition = new HashSet<FunctionDefinition>();
            InitialAssignment = new HashSet<InitialAssignment>();
            MapModelsPathways = new HashSet<MapModelsPathways>();
            ModelOrganism = new HashSet<ModelOrganism>();
            Parameter = new HashSet<Parameter>();
            Reaction = new HashSet<Reaction>();
            Rule = new HashSet<Rule>();
            SpeciesType = new HashSet<SpeciesType>();
            Unit = new HashSet<Unit>();
            UnitDefinition = new HashSet<UnitDefinition>();
        }

        public Guid id { get; set; }

        [StringLength(100)]
        public string sbmlId { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        public byte? sbmlLevel { get; set; }

        public byte? sbmlVersion { get; set; }

        public short? dataSourceId { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string sbmlFile { get; set; }

        [StringLength(100)]
        public string sbmlFileName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Compartment> Compartment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompartmentType> CompartmentType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Constraint> Constraint { get; set; }

        public virtual DataSource DataSource { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Event> Event { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FunctionDefinition> FunctionDefinition { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InitialAssignment> InitialAssignment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapModelsPathways> MapModelsPathways { get; set; }

        public virtual Sbase Sbase { get; set; }

        public virtual ModelLayout ModelLayout { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModelOrganism> ModelOrganism { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Parameter> Parameter { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reaction> Reaction { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rule> Rule { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SpeciesType> SpeciesType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Unit> Unit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UnitDefinition> UnitDefinition { get; set; }
    }
}
