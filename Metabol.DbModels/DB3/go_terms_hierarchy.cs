using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    public partial class go_terms_hierarchy
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(7)]
        public string ParentID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(7)]
        public string ChildID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string Type { get; set; }

        public int? TermLevel { get; set; }

        public bool? OnPathUnderCatalyticActivity { get; set; }
    }
}
