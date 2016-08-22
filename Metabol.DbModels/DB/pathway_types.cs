namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pathway_types
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte pathway_type_id { get; set; }

        [Key]
        [StringLength(100)]
        public string name { get; set; }
    }
}
