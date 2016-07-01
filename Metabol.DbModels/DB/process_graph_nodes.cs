using System;
using System.ComponentModel.DataAnnotations;

namespace Metabol.DbModels.DB
{
    public partial class process_graph_nodes
    {
        public Guid? pathwayId { get; set; }

        [Key]
        public Guid genericProcessId { get; set; }

        public Guid? graphNodeId { get; set; }
    }
}
