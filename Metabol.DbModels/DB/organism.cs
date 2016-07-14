using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
    [Table("organisms")]
    public partial class organism
    {
        public Guid id { get; set; }

        [StringLength(20)]
        public string taxonomy_id { get; set; }

        public int cM_unit_length { get; set; }
    }
}
