using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    public partial class name_types
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte name_type_id { get; set; }

        [Key]
        [StringLength(50)]
        public string name { get; set; }
    }
}
