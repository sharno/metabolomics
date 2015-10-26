namespace Metabol.Util.DB2
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class attribute_names
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int attributeId { get; set; }

        [Key]
        [StringLength(100)]
        public string name { get; set; }
    }
}
