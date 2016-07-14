using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
    public partial class attribute_values
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int attributeId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid itemId { get; set; }

        [StringLength(800)]
        public string value { get; set; }

        [Column(TypeName = "text")]
        public string textValue { get; set; }
    }
}
