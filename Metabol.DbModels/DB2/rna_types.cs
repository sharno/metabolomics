using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    public partial class rna_types
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte rna_type_id { get; set; }

        [Key]
        [StringLength(4)]
        public string name { get; set; }
    }
}
