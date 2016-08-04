using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
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
