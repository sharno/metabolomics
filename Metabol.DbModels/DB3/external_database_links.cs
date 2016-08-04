using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    public partial class external_database_links
    {
        [Key]
        [Column(Order = 0)]
        public Guid local_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int external_database_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string id_in_external_database { get; set; }

        [StringLength(100)]
        public string name_in_external_database { get; set; }
    }
}
