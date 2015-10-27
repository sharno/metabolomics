namespace Metabol.Util.DB2
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MetaboliteReactionCount")]
    public class MetaboliteReactionCount
    {
        [Key]
        public Guid id { get; set; }

        public Guid speciesId { get; set; }

        [ForeignKey("speciesId")]
        public Species Species { get; set; }

        public int consumerCount { get; set; }

        public int producerCount { get; set; }
    }
}
