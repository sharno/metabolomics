namespace Metabol.Util.DB2
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MetaboliteReactionStoichiometry")]
    public class MetaboliteReactionStoichiometry
    {
        [Key]
        public Guid id { get; set; }

        public Guid speciesId { get; set; }

        [ForeignKey("speciesId")]
        public Species Species { get; set; }

        public double consumerStoch { get; set; }

        public double producerStoch { get; set; }
    }


}
