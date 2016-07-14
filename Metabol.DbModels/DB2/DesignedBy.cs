using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    [Table("DesignedBy")]
    public partial class DesignedBy
    {
        public Guid Id { get; set; }

        public Guid ModelMetadataId { get; set; }

        public Guid AuthorId { get; set; }

        public virtual Author Author { get; set; }

        public virtual ModelMetadata ModelMetadata { get; set; }
    }
}
