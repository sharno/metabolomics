using System;

namespace Metabol.DbModels.DB
{
    public partial class common_molecules
    {
        public Guid id { get; set; }

        public virtual basic_molecules basic_molecules { get; set; }
    }
}
