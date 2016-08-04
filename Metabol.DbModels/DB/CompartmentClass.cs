using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
    [Table("CompartmentClass")]
    public partial class CompartmentClass
    {
        public Guid id { get; set; }

        public string name { get; set; }

        public Guid? parentid { get; set; }
    }
}
