namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InitialAssignment")]
    public partial class InitialAssignment
    {
        public Guid id { get; set; }

        public Guid modelId { get; set; }

        [Required]
        [StringLength(100)]
        public string symbol { get; set; }

        [Column(TypeName = "xml")]
        [Required]
        public string math { get; set; }

        public virtual Model Model { get; set; }

        public virtual Sbase Sbase { get; set; }
    }
}
