namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MapReactionECNumber")]
    public partial class MapReactionECNumber
    {
        [Key]
        [Column(Order = 0)]
        public Guid reactionId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string ecNumber { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short qualifierId { get; set; }

        public virtual AnnotationQualifier AnnotationQualifier { get; set; }

        public virtual ec_numbers ec_numbers { get; set; }

        public virtual Reaction Reaction { get; set; }
    }
}
