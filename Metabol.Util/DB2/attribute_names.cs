namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class attribute_names
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int attributeId { get; set; }

        [Key]
        [StringLength(100)]
        public string name { get; set; }
    }
}
