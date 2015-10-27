namespace Metabol.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Reaction")]
    public partial class Reaction
    {
<<<<<<< HEAD:CyclesCacher/DB/Reaction.cs
=======
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reaction()
        {
            this.CycleReactions = new HashSet<CycleReaction>();
        }

>>>>>>> 93cf39bbf206afaa818d103f4dd0b1b9dbe08a0d:Metabol.Util/DB/Reaction.cs
        public Guid id { get; set; }

        public Guid modelId { get; set; }

        public string sbmlId { get; set; }

        public string name { get; set; }

        public bool reversible { get; set; }

        public bool fast { get; set; }

        public Guid? kineticLawId { get; set; }
    }
}
