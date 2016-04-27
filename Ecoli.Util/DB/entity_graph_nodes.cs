namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class entity_graph_nodes
    {
        public Guid? pathwayId { get; set; }

        [Key]
        public Guid entityId { get; set; }

        public Guid? graphNodeId { get; set; }
    }
}
