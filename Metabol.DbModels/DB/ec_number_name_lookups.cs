namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ec_number_name_lookups
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string ec_number { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid name_id { get; set; }

        public byte name_type_id { get; set; }

        public virtual ec_numbers ec_numbers { get; set; }

        public virtual molecular_entity_names molecular_entity_names { get; set; }
    }
}
