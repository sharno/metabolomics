using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
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
