namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class process_graph_nodes
    {
        public Guid? pathwayId { get; set; }

        [Key]
        public Guid genericProcessId { get; set; }

        public Guid? graphNodeId { get; set; }
    }
}
