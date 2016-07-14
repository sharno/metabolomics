using System.ComponentModel.DataAnnotations;

namespace Metabol.DbModels.DB
{
    public partial class common_species
    {
        [Key]
        [StringLength(100)]
        public string name { get; set; }
    }
}
