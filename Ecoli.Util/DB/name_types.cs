namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class name_types
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte name_type_id { get; set; }

        [Key]
        [StringLength(50)]
        public string name { get; set; }
    }
}
