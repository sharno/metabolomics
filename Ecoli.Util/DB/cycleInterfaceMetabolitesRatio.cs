namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class cycleInterfaceMetabolitesRatio
    {
        [Key]
        [Column(Order = 0)]
        public Guid cycleId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid metabolite1 { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid metabolite2 { get; set; }

        [Key]
        [Column(Order = 3)]
        public double ratio { get; set; }

        public virtual Cycle Cycle { get; set; }

        public virtual Species Species { get; set; }

        public virtual Species Species1 { get; set; }
    }
}
