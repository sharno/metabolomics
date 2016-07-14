using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    public partial class MapReactionsProcessEntities
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

        public virtual processes processes { get; set; }

        public virtual Reaction Reaction { get; set; }
    }
}
