namespace CyclesCacher.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class rna_types
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte rna_type_id { get; set; }

        [Key]
        [StringLength(4)]
        public string name { get; set; }
    }
}
