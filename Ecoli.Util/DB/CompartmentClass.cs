namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CompartmentClass")]
    public partial class CompartmentClass
    {
        public Guid id { get; set; }

        public string name { get; set; }

        public Guid? parentid { get; set; }
    }
}
