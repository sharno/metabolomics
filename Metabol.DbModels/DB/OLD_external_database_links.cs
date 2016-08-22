namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OLD_external_database_links
    {
        [Key]
        [Column(Order = 0)]
        public Guid local_id { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid external_database_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string id_in_external_database { get; set; }

        public virtual OLD_external_databases OLD_external_databases { get; set; }
    }
}
