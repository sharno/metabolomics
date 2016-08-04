using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
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
