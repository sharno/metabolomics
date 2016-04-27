namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MetaboliteReactionCount")]
    public partial class MetaboliteReactionCount
    {
        public Guid id { get; set; }

        public Guid speciesId { get; set; }

        public int consumerCount { get; set; }

        public int producerCount { get; set; }

        public virtual Species Species { get; set; }
    }
}
