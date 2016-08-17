using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    public partial class process_entity_roles
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte role_id { get; set; }

        [Key]
        [StringLength(18)]
        public string name { get; set; }
    }
}
