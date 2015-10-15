namespace CyclesCacher.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Reaction")]
    public partial class Reaction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reaction()
        {
            CycleReactions = new HashSet<CycleReaction>();
        }

        public Guid id { get; set; }

        public Guid modelId { get; set; }

        public string sbmlId { get; set; }

        public string name { get; set; }

        public bool reversible { get; set; }

        public bool fast { get; set; }

        public Guid? kineticLawId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CycleReaction> CycleReactions { get; set; }

        public override bool Equals(object obj)
        {
            return this.id == ((Reaction)obj).id;
        }
    }
}
