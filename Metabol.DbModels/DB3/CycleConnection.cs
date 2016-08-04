using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    [Table("CycleConnection")]
    public partial class CycleConnection
    {
        [Key]
        [Column(Order = 0)]
        public Guid cycleId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid metaboliteId { get; set; }

        [Key]
        [Column(Order = 2)]
        public byte roleId { get; set; }

        [Key]
        [Column(Order = 3)]
        public double stoichiometry { get; set; }

        [Key]
        [Column(Order = 4)]
        public bool isReversible { get; set; }

        public virtual Cycle Cycle { get; set; }

        public virtual Species Species { get; set; }
    }
}
