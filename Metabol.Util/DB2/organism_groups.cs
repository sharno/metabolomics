namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class organism_groups
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public organism_groups()
        {
            this.genes = new HashSet<gene>();
            this.MapModelsPathways = new HashSet<MapModelsPathway>();
            this.ModelOrganism = new HashSet<ModelOrganism>();
            this.catalyzes = new HashSet<catalyze>();
            this.organism_groups1 = new HashSet<organism_groups>();
        }

        public Guid id { get; set; }

        [StringLength(100)]
        public string scientific_name { get; set; }

        [StringLength(100)]
        public string common_name { get; set; }

        public Guid? parent_id { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        public bool is_organism { get; set; }

        [StringLength(500)]
        public string nodeLabel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<gene> genes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapModelsPathway> MapModelsPathways { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModelOrganism> ModelOrganism { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<catalyze> catalyzes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<organism_groups> organism_groups1 { get; set; }

        public virtual organism_groups organism_groups2 { get; set; }
    }
}
