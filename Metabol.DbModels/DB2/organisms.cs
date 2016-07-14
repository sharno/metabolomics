using System;
using System.ComponentModel.DataAnnotations;

namespace Metabol.DbModels.DB2
{
    public partial class organisms
    {
        public Guid id { get; set; }

        [StringLength(20)]
        public string taxonomy_id { get; set; }

        public int cM_unit_length { get; set; }
    }
}
