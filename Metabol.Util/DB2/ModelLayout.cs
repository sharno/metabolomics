namespace Metabol.Util.DB2
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ModelLayout")]
    public partial class ModelLayout
    {
        public Guid id { get; set; }

        public string layout { get; set; }

        public virtual Model Model { get; set; }
    }
}
