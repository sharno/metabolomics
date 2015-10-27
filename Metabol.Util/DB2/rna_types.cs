namespace Metabol.Util.DB2
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class rna_types
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte rna_type_id { get; set; }

        [Key]
        [StringLength(4)]
        public string name { get; set; }
    }
}
