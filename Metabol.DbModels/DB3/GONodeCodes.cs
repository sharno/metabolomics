using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    public partial class GONodeCodes
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(7)]
        public string goid { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string nodeCode { get; set; }

        public virtual go_terms go_terms { get; set; }
    }
}
