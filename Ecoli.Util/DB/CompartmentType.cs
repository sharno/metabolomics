namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CompartmentType")]
    public partial class CompartmentType
    {
        public Guid id { get; set; }

        public Guid modelId { get; set; }

        [StringLength(100)]
        public string sbmlId { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        public virtual Model Model { get; set; }

        public virtual Sbase Sbase { get; set; }
    }
}
