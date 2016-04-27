namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class molecular_entities
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public molecular_entities()
        {
            entity_name_lookups = new HashSet<entity_name_lookups>();
            MapSpeciesMolecularEntities = new HashSet<MapSpeciesMolecularEntity>();
            pathway_links = new HashSet<pathway_links>();
            process_entities = new HashSet<process_entities>();
        }

        public Guid id { get; set; }

        public byte type_id { get; set; }

        [StringLength(255)]
        public string name { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        public virtual basic_molecules basic_molecules { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<entity_name_lookups> entity_name_lookups { get; set; }

        public virtual gene_products gene_products { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapSpeciesMolecularEntity> MapSpeciesMolecularEntities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pathway_links> pathway_links { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<process_entities> process_entities { get; set; }
    }
}
