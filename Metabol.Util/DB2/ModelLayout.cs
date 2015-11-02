namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ModelLayout")]
    public partial class ModelLayout
    {
        public Guid id { get; set; }

        public string layout { get; set; }

        public virtual Model Model { get; set; }
    }
}
