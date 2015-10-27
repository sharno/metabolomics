namespace Metabol.Util.DB2
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("FunctionDefinition")]
    public partial class FunctionDefinition
    {
        public Guid id { get; set; }

        public Guid modelId { get; set; }

        [Required]
        [StringLength(100)]
        public string sbmlId { get; set; }

        public string name { get; set; }

        [Column(TypeName = "xml")]
        [Required]
        public string lambda { get; set; }

        public virtual Model Model { get; set; }

        public virtual Sbase Sbase { get; set; }
    }
}
