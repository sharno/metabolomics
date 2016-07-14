using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    public partial class ModelOrganism
    {
        [Key]
        [Column(Order = 0)]
        public Guid modelId { get; set; }

        public Guid? organismGroupId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NCBITaxonomyId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short qualifierId { get; set; }

        public virtual AnnotationQualifier AnnotationQualifier { get; set; }

        public virtual Model Model { get; set; }

        public virtual organism_groups organism_groups { get; set; }
    }
}
