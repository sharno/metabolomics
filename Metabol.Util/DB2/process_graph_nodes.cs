namespace Metabol.Util.DB2
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class process_graph_nodes
    {
        public Guid? pathwayId { get; set; }

        [Key]
        public Guid genericProcessId { get; set; }

        public Guid? graphNodeId { get; set; }
    }
}
