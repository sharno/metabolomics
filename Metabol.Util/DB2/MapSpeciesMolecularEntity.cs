namespace Metabol.Util.DB2
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class MapSpeciesMolecularEntity
    {
        [Key]
        [Column(Order = 0)]
        public Guid speciesId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short qualifierId { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid molecularEntityId { get; set; }

        public virtual AnnotationQualifier AnnotationQualifier { get; set; }

        public virtual molecular_entities molecular_entities { get; set; }

        public virtual Species Species { get; set; }
    }
}
