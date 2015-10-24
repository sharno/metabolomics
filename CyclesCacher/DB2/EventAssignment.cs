namespace CyclesCacher.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventAssignment")]
    public partial class EventAssignment
    {
        public Guid id { get; set; }

        public Guid eventId { get; set; }

        [Required]
        [StringLength(100)]
        public string variable { get; set; }

        [Column(TypeName = "xml")]
        [Required]
        public string math { get; set; }

        public virtual Event Event { get; set; }

        public virtual Sbase Sbase { get; set; }
    }
}
