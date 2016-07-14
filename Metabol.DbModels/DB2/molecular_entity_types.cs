using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    public partial class molecular_entity_types
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte type_id { get; set; }

        [Key]
        [StringLength(50)]
        public string name { get; set; }
    }
}
