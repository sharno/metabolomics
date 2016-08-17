using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    [Table("ModelLayout")]
    public partial class ModelLayout
    {
        public Guid id { get; set; }

        public string layout { get; set; }

        public virtual Model Model { get; set; }
    }
}
