namespace Metabol.Util.DB2
{
    using System.ComponentModel.DataAnnotations;

    public partial class common_species
    {
        [Key]
        [StringLength(100)]
        public string name { get; set; }
    }
}
