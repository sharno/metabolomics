namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class molecular_entity_types
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte type_id { get; set; }

        [Key]
        [StringLength(50)]
        public string name { get; set; }
    }
}
