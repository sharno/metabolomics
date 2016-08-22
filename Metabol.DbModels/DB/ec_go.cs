namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ec_go
    {
        [Key]
        [StringLength(50)]
        public string ec_number { get; set; }

        [StringLength(10)]
        public string go_id { get; set; }
    }
}
