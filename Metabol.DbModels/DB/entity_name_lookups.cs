namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class entity_name_lookups
    {
        [Key]
        [Column(Order = 0)]
        public Guid entity_id { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid name_id { get; set; }

        public byte name_type_id { get; set; }

        public virtual molecular_entities molecular_entities { get; set; }

        public virtual molecular_entity_names molecular_entity_names { get; set; }
    }
}
