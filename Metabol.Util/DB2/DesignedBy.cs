namespace Metabol.Util.DB2
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

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
