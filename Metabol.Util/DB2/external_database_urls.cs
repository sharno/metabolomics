namespace Metabol.Util.DB2
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class external_database_urls
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int external_database_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(16)]
        public string type { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(256)]
        public string url_template { get; set; }
    }
}
