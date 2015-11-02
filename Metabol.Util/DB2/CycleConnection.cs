namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CycleConnection")]
    public partial class CycleConnection
    {
        [Key]
        [Column(Order = 0)]
        public Guid cycleId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid metaboliteId { get; set; }

        public byte roleId { get; set; }

        public double stoichiometry { get; set; }

        public bool isReversible { get; set; }

        public virtual Cycle Cycle { get; set; }

        public virtual Species Species { get; set; }
    }
}
