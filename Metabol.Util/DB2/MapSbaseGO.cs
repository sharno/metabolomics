namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MapSbaseGO")]
    public partial class MapSbaseGO
    {
        [Key]
        [Column(Order = 0)]
        public Guid sbaseId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(7)]
        public string goId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short qualifierId { get; set; }
    }
}
