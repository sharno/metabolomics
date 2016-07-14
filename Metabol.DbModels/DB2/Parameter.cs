using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    [Table("Parameter")]
    public partial class Parameter
    {
        public Guid id { get; set; }

        public Guid? modelId { get; set; }

        public Guid? reactionId { get; set; }

        [Required]
        [StringLength(100)]
        public string sbmlId { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        public double? value { get; set; }

        public Guid? unitsId { get; set; }

        public bool constant { get; set; }

        public virtual Model Model { get; set; }

        public virtual Sbase Sbase { get; set; }

        public virtual UnitDefinition UnitDefinition { get; set; }
    }
}
