using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    public partial class external_databases
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        [StringLength(256)]
        public string fullname { get; set; }

        [StringLength(50)]
        public string url { get; set; }
    }
}
