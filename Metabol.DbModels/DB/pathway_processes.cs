using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
    public partial class pathway_processes
    {
        [Key]
        [Column(Order = 0)]
        public Guid pathway_id { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid process_id { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        public virtual pathway pathway { get; set; }

        public virtual process process { get; set; }
    }
}
