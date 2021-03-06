using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
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
