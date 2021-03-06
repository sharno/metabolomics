using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    [Table("Rule")]
    public partial class Rule
    {
        public Guid id { get; set; }

        public Guid modelId { get; set; }

        [StringLength(100)]
        public string variable { get; set; }

        [Required]
        public string math { get; set; }

        public byte ruleTypeId { get; set; }

        public virtual Model Model { get; set; }

        public virtual RuleType RuleType { get; set; }

        public virtual Sbase Sbase { get; set; }
    }
}
