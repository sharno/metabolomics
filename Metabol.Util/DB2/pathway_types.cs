namespace Metabol.Util.DB2
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class pathway_types
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte pathway_type_id { get; set; }

        [Key]
        [StringLength(100)]
        public string name { get; set; }
    }
}
