using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    [Table("Formula")]
    public partial class Formula
    {
        [Key]
        [Column(Order = 0)]
        public Guid speciesId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string atom { get; set; }

        public int numAtoms { get; set; }

        public virtual Species Species { get; set; }
    }
}
