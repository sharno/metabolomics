namespace CyclesCacher.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class process_entity_roles
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte role_id { get; set; }

        [Key]
        [StringLength(18)]
        public string name { get; set; }
    }
}
