namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MapReactionsProcessEntity
    {
        [Key]
        [Column(Order = 0)]
        public Guid reactionId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid processId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short qualifierId { get; set; }

        public virtual AnnotationQualifier AnnotationQualifier { get; set; }

        public virtual process process { get; set; }

        public virtual Reaction Reaction { get; set; }
    }
}
