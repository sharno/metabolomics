namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MetaboliteReactionStoichiometry")]
    public partial class MetaboliteReactionStoichiometry
    {
        public Guid id { get; set; }

        public Guid speciesId { get; set; }

        public double consumerStoch { get; set; }

        public double producerStoch { get; set; }

        public virtual Species Species { get; set; }
    }
}
