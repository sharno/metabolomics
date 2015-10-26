namespace Metabol.Util.DB2
{
    using System.ComponentModel.DataAnnotations;

    public partial class ec_go
    {
        [Key]
        [StringLength(50)]
        public string ec_number { get; set; }

        [StringLength(10)]
        public string go_id { get; set; }
    }
}
