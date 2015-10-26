namespace Metabol.Util.DB2
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Sbase")]
    public partial class Sbase
    {
        public Guid id { get; set; }

        [StringLength(100)]
        public string metaId { get; set; }

        [StringLength(50)]
        public string sboTerm { get; set; }

        [Column(TypeName = "ntext")]
        public string notes { get; set; }

        [Column(TypeName = "ntext")]
        public string annotation { get; set; }

        public virtual Compartment Compartment { get; set; }

        public virtual CompartmentType CompartmentType { get; set; }

        public virtual Constraint Constraint { get; set; }

        public virtual Event Event { get; set; }

        public virtual EventAssignment EventAssignment { get; set; }

        public virtual EventDelay EventDelay { get; set; }

        public virtual EventTrigger EventTrigger { get; set; }

        public virtual FunctionDefinition FunctionDefinition { get; set; }

        public virtual InitialAssignment InitialAssignment { get; set; }

        public virtual KineticLaw KineticLaw { get; set; }

        public virtual Model Model { get; set; }

        public virtual Parameter Parameter { get; set; }

        public virtual Reaction Reaction { get; set; }

        public virtual ReactionSpecy ReactionSpecy { get; set; }

        public virtual Rule Rule { get; set; }

        public virtual Species Species { get; set; }

        public virtual SpeciesType SpeciesType { get; set; }

        public virtual StoichiometryMath StoichiometryMath { get; set; }

        public virtual Unit Unit { get; set; }

        public virtual UnitDefinition UnitDefinition { get; set; }
    }
}
