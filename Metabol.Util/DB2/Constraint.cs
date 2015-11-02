namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Constraint")]
    public partial class Constraint
    {
        public Guid id { get; set; }

        public Guid modelId { get; set; }

        [Column(TypeName = "xml")]
        [Required]
        public string math { get; set; }

        [Column(TypeName = "xml")]
        public string message { get; set; }

        public virtual Model Model { get; set; }

        public virtual Sbase Sbase { get; set; }
    }
}
