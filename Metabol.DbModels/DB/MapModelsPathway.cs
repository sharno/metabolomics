namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MapModelsPathway
    {
        [Key]
        [Column(Order = 0)]
        public Guid modelId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid pathwayId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short qualifierId { get; set; }

        public Guid? organismGroupId { get; set; }

        public virtual AnnotationQualifier AnnotationQualifier { get; set; }

        public virtual Model Model { get; set; }

        public virtual organism_groups organism_groups { get; set; }

        public virtual pathway pathway { get; set; }
    }
}
