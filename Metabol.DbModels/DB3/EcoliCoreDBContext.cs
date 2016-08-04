using System.Data.Entity;

namespace Metabol.DbModels.DB3
{
    public partial class EcoliCoreDBContext : DbContext
    {
        public EcoliCoreDBContext()
            : base("name=EcoliCoreDBContext")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AnnotationQualifier> AnnotationQualifier { get; set; }
        public virtual DbSet<attribute_names> attribute_names { get; set; }
        public virtual DbSet<attribute_values> attribute_values { get; set; }
        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<basic_molecules> basic_molecules { get; set; }
        public virtual DbSet<chromosomes> chromosomes { get; set; }
        public virtual DbSet<common_molecules> common_molecules { get; set; }
        public virtual DbSet<common_species> common_species { get; set; }
        public virtual DbSet<Compartment> Compartment { get; set; }
        public virtual DbSet<CompartmentType> CompartmentType { get; set; }
        public virtual DbSet<Constraint> Constraint { get; set; }
        public virtual DbSet<Cycle> Cycle { get; set; }
        public virtual DbSet<DataSource> DataSource { get; set; }
        public virtual DbSet<DesignedBy> DesignedBy { get; set; }
        public virtual DbSet<ec_number_name_lookups> ec_number_name_lookups { get; set; }
        public virtual DbSet<ec_numbers> ec_numbers { get; set; }
        public virtual DbSet<entity_name_lookups> entity_name_lookups { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<EventAssignment> EventAssignment { get; set; }
        public virtual DbSet<EventDelay> EventDelay { get; set; }
        public virtual DbSet<EventTrigger> EventTrigger { get; set; }
        public virtual DbSet<Formula> Formula { get; set; }
        public virtual DbSet<FunctionDefinition> FunctionDefinition { get; set; }
        public virtual DbSet<gene_encodings> gene_encodings { get; set; }
        public virtual DbSet<gene_products> gene_products { get; set; }
        public virtual DbSet<genes> genes { get; set; }
        public virtual DbSet<go_terms> go_terms { get; set; }
        public virtual DbSet<GONodeCodes> GONodeCodes { get; set; }
        public virtual DbSet<InitialAssignment> InitialAssignment { get; set; }
        public virtual DbSet<KineticLaw> KineticLaw { get; set; }
        public virtual DbSet<MapModelsPathways> MapModelsPathways { get; set; }
        public virtual DbSet<MapReactionECNumber> MapReactionECNumber { get; set; }
        public virtual DbSet<MapReactionsProcessEntities> MapReactionsProcessEntities { get; set; }
        public virtual DbSet<MapSpeciesMolecularEntities> MapSpeciesMolecularEntities { get; set; }
        public virtual DbSet<MetaboliteReactionCount> MetaboliteReactionCount { get; set; }
        public virtual DbSet<MetaboliteReactionStoichiometry> MetaboliteReactionStoichiometry { get; set; }
        public virtual DbSet<Model> Model { get; set; }
        public virtual DbSet<ModelLayout> ModelLayout { get; set; }
        public virtual DbSet<ModelMetadata> ModelMetadata { get; set; }
        public virtual DbSet<ModelOrganism> ModelOrganism { get; set; }
        public virtual DbSet<molecular_entities> molecular_entities { get; set; }
        public virtual DbSet<molecular_entity_names> molecular_entity_names { get; set; }
        public virtual DbSet<molecular_entity_types> molecular_entity_types { get; set; }
        public virtual DbSet<name_types> name_types { get; set; }
        public virtual DbSet<OLD_external_database_links> OLD_external_database_links { get; set; }
        public virtual DbSet<OLD_external_databases> OLD_external_databases { get; set; }
        public virtual DbSet<organism_groups> organism_groups { get; set; }
        public virtual DbSet<organisms> organisms { get; set; }
        public virtual DbSet<Parameter> Parameter { get; set; }
        public virtual DbSet<pathway_groups> pathway_groups { get; set; }
        public virtual DbSet<pathway_links> pathway_links { get; set; }
        public virtual DbSet<pathway_processes> pathway_processes { get; set; }
        public virtual DbSet<pathway_types> pathway_types { get; set; }
        public virtual DbSet<pathways> pathways { get; set; }
        public virtual DbSet<process_entities> process_entities { get; set; }
        public virtual DbSet<process_entity_roles> process_entity_roles { get; set; }
        public virtual DbSet<processes> processes { get; set; }
        public virtual DbSet<proteins> proteins { get; set; }
        public virtual DbSet<Reaction> Reaction { get; set; }
        public virtual DbSet<ReactionBound> ReactionBound { get; set; }
        public virtual DbSet<ReactionBoundFix> ReactionBoundFix { get; set; }
        public virtual DbSet<ReactionSpecies> ReactionSpecies { get; set; }
        public virtual DbSet<ReactionSpeciesRole> ReactionSpeciesRole { get; set; }
        public virtual DbSet<rna_types> rna_types { get; set; }
        public virtual DbSet<rnas> rnas { get; set; }
        public virtual DbSet<Rule> Rule { get; set; }
        public virtual DbSet<RuleType> RuleType { get; set; }
        public virtual DbSet<Sbase> Sbase { get; set; }
        public virtual DbSet<Species> Species { get; set; }
        public virtual DbSet<SpeciesType> SpeciesType { get; set; }
        public virtual DbSet<StoichiometryMath> StoichiometryMath { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<UnitDefinition> UnitDefinition { get; set; }
        public virtual DbSet<viewState> viewState { get; set; }
        public virtual DbSet<BlockedReactions> BlockedReactions { get; set; }
        public virtual DbSet<catalyzes> catalyzes { get; set; }
        public virtual DbSet<chromosome_bands> chromosome_bands { get; set; }
        public virtual DbSet<chromosomes_pathcase> chromosomes_pathcase { get; set; }
        public virtual DbSet<CompartmentClass> CompartmentClass { get; set; }
        public virtual DbSet<CycleConnection> CycleConnection { get; set; }
        public virtual DbSet<CycleReaction> CycleReaction { get; set; }
        public virtual DbSet<ec_go> ec_go { get; set; }
        public virtual DbSet<ec_go_orig> ec_go_orig { get; set; }
        public virtual DbSet<entity_graph_nodes> entity_graph_nodes { get; set; }
        public virtual DbSet<external_database_links> external_database_links { get; set; }
        public virtual DbSet<external_database_urls> external_database_urls { get; set; }
        public virtual DbSet<external_databases> external_databases { get; set; }
        public virtual DbSet<go_annotation_pathways> go_annotation_pathways { get; set; }
        public virtual DbSet<go_pathway_annotation_counts> go_pathway_annotation_counts { get; set; }
        public virtual DbSet<go_pathway_group_annotation_counts> go_pathway_group_annotation_counts { get; set; }
        public virtual DbSet<go_terms_hierarchy> go_terms_hierarchy { get; set; }
        public virtual DbSet<MapSbaseGO> MapSbaseGO { get; set; }
        public virtual DbSet<process_graph_nodes> process_graph_nodes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnnotationQualifier>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<AnnotationQualifier>()
                .HasMany(e => e.MapModelsPathways)
                .WithRequired(e => e.AnnotationQualifier)
                .HasForeignKey(e => e.qualifierId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AnnotationQualifier>()
                .HasMany(e => e.MapReactionECNumber)
                .WithRequired(e => e.AnnotationQualifier)
                .HasForeignKey(e => e.qualifierId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AnnotationQualifier>()
                .HasMany(e => e.MapReactionsProcessEntities)
                .WithRequired(e => e.AnnotationQualifier)
                .HasForeignKey(e => e.qualifierId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AnnotationQualifier>()
                .HasMany(e => e.ModelOrganism)
                .WithRequired(e => e.AnnotationQualifier)
                .HasForeignKey(e => e.qualifierId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AnnotationQualifier>()
                .HasMany(e => e.MapSpeciesMolecularEntities)
                .WithRequired(e => e.AnnotationQualifier)
                .HasForeignKey(e => e.qualifierId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<attribute_names>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<attribute_values>()
                .Property(e => e.value)
                .IsUnicode(false);

            modelBuilder.Entity<attribute_values>()
                .Property(e => e.textValue)
                .IsUnicode(false);

            modelBuilder.Entity<Author>()
                .HasMany(e => e.DesignedBy)
                .WithRequired(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<basic_molecules>()
                .HasOptional(e => e.common_molecules)
                .WithRequired(e => e.basic_molecules);

            modelBuilder.Entity<chromosomes>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<chromosomes>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<chromosomes>()
                .HasMany(e => e.genes)
                .WithOptional(e => e.chromosomes)
                .HasForeignKey(e => e.chromosome_id);

            modelBuilder.Entity<common_species>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Compartment>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<Compartment>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Compartment>()
                .HasMany(e => e.Compartment1)
                .WithOptional(e => e.Compartment2)
                .HasForeignKey(e => e.outside);

            modelBuilder.Entity<Compartment>()
                .HasMany(e => e.Species)
                .WithRequired(e => e.Compartment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompartmentType>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<CompartmentType>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Cycle>()
                .HasMany(e => e.CycleConnection)
                .WithRequired(e => e.Cycle)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Cycle>()
                .HasMany(e => e.CycleReaction)
                .WithRequired(e => e.Cycle)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DataSource>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<DataSource>()
                .Property(e => e.url)
                .IsUnicode(false);

            modelBuilder.Entity<DataSource>()
                .HasMany(e => e.Model)
                .WithOptional(e => e.DataSource)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ec_number_name_lookups>()
                .Property(e => e.ec_number)
                .IsUnicode(false);

            modelBuilder.Entity<ec_numbers>()
                .Property(e => e.ec_number)
                .IsUnicode(false);

            modelBuilder.Entity<ec_numbers>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<ec_numbers>()
                .Property(e => e.nodeCode)
                .IsUnicode(false);

            modelBuilder.Entity<ec_numbers>()
                .HasMany(e => e.ec_number_name_lookups)
                .WithRequired(e => e.ec_numbers)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ec_numbers>()
                .HasMany(e => e.MapReactionECNumber)
                .WithRequired(e => e.ec_numbers)
                .HasForeignKey(e => e.ecNumber)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<Event>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<EventAssignment>()
                .Property(e => e.variable)
                .IsUnicode(false);

            modelBuilder.Entity<EventTrigger>()
                .HasMany(e => e.Event)
                .WithRequired(e => e.EventTrigger)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Formula>()
                .Property(e => e.atom)
                .IsUnicode(false);

            modelBuilder.Entity<FunctionDefinition>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<FunctionDefinition>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<gene_encodings>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<gene_products>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<gene_products>()
                .HasMany(e => e.gene_encodings)
                .WithRequired(e => e.gene_products)
                .HasForeignKey(e => e.gene_product_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<gene_products>()
                .HasMany(e => e.catalyzes)
                .WithOptional(e => e.gene_products)
                .HasForeignKey(e => e.gene_product_id);

            modelBuilder.Entity<gene_products>()
                .HasOptional(e => e.proteins)
                .WithRequired(e => e.gene_products);

            modelBuilder.Entity<gene_products>()
                .HasOptional(e => e.rnas)
                .WithRequired(e => e.gene_products);

            modelBuilder.Entity<genes>()
                .Property(e => e.raw_address)
                .IsUnicode(false);

            modelBuilder.Entity<genes>()
                .Property(e => e.cytogenic_address)
                .IsUnicode(false);

            modelBuilder.Entity<genes>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<go_terms>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<go_terms>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<go_terms>()
                .HasMany(e => e.GONodeCodes)
                .WithRequired(e => e.go_terms)
                .HasForeignKey(e => e.goid)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<GONodeCodes>()
                .Property(e => e.goid)
                .IsUnicode(false);

            modelBuilder.Entity<GONodeCodes>()
                .Property(e => e.nodeCode)
                .IsUnicode(false);

            modelBuilder.Entity<InitialAssignment>()
                .Property(e => e.symbol)
                .IsUnicode(false);

            modelBuilder.Entity<KineticLaw>()
                .Property(e => e.math)
                .IsUnicode(false);

            modelBuilder.Entity<MapReactionECNumber>()
                .Property(e => e.ecNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Model>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<Model>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Model>()
                .Property(e => e.sbmlFileName)
                .IsUnicode(false);

            modelBuilder.Entity<Model>()
                .HasMany(e => e.Event)
                .WithRequired(e => e.Model)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Model>()
                .HasMany(e => e.FunctionDefinition)
                .WithRequired(e => e.Model)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Model>()
                .HasOptional(e => e.ModelLayout)
                .WithRequired(e => e.Model);

            modelBuilder.Entity<Model>()
                .HasMany(e => e.ModelOrganism)
                .WithRequired(e => e.Model)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Model>()
                .HasMany(e => e.Parameter)
                .WithOptional(e => e.Model)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Model>()
                .HasMany(e => e.Unit)
                .WithOptional(e => e.Model)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ModelMetadata>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<ModelMetadata>()
                .HasMany(e => e.DesignedBy)
                .WithRequired(e => e.ModelMetadata)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<molecular_entities>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<molecular_entities>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<molecular_entities>()
                .HasOptional(e => e.basic_molecules)
                .WithRequired(e => e.molecular_entities);

            modelBuilder.Entity<molecular_entities>()
                .HasMany(e => e.entity_name_lookups)
                .WithRequired(e => e.molecular_entities)
                .HasForeignKey(e => e.entity_id);

            modelBuilder.Entity<molecular_entities>()
                .HasOptional(e => e.gene_products)
                .WithRequired(e => e.molecular_entities);

            modelBuilder.Entity<molecular_entities>()
                .HasMany(e => e.MapSpeciesMolecularEntities)
                .WithRequired(e => e.molecular_entities)
                .HasForeignKey(e => e.molecularEntityId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<molecular_entities>()
                .HasMany(e => e.pathway_links)
                .WithRequired(e => e.molecular_entities)
                .HasForeignKey(e => e.entity_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<molecular_entities>()
                .HasMany(e => e.process_entities)
                .WithRequired(e => e.molecular_entities)
                .HasForeignKey(e => e.entity_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<molecular_entity_names>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<molecular_entity_names>()
                .HasMany(e => e.ec_number_name_lookups)
                .WithRequired(e => e.molecular_entity_names)
                .HasForeignKey(e => e.name_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<molecular_entity_names>()
                .HasMany(e => e.entity_name_lookups)
                .WithRequired(e => e.molecular_entity_names)
                .HasForeignKey(e => e.name_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<molecular_entity_types>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<name_types>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<OLD_external_database_links>()
                .Property(e => e.id_in_external_database)
                .IsUnicode(false);

            modelBuilder.Entity<OLD_external_databases>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<OLD_external_databases>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<OLD_external_databases>()
                .HasMany(e => e.OLD_external_database_links)
                .WithRequired(e => e.OLD_external_databases)
                .HasForeignKey(e => e.external_database_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<organism_groups>()
                .Property(e => e.scientific_name)
                .IsUnicode(false);

            modelBuilder.Entity<organism_groups>()
                .Property(e => e.common_name)
                .IsUnicode(false);

            modelBuilder.Entity<organism_groups>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<organism_groups>()
                .Property(e => e.nodeLabel)
                .IsUnicode(false);

            modelBuilder.Entity<organism_groups>()
                .HasMany(e => e.genes)
                .WithRequired(e => e.organism_groups)
                .HasForeignKey(e => e.organism_group_id);

            modelBuilder.Entity<organism_groups>()
                .HasMany(e => e.MapModelsPathways)
                .WithOptional(e => e.organism_groups)
                .HasForeignKey(e => e.organismGroupId);

            modelBuilder.Entity<organism_groups>()
                .HasMany(e => e.ModelOrganism)
                .WithOptional(e => e.organism_groups)
                .HasForeignKey(e => e.organismGroupId);

            modelBuilder.Entity<organism_groups>()
                .HasMany(e => e.catalyzes)
                .WithOptional(e => e.organism_groups)
                .HasForeignKey(e => e.organism_group_id);

            modelBuilder.Entity<organism_groups>()
                .HasMany(e => e.organism_groups1)
                .WithOptional(e => e.organism_groups2)
                .HasForeignKey(e => e.parent_id);

            modelBuilder.Entity<organisms>()
                .Property(e => e.taxonomy_id)
                .IsUnicode(false);

            modelBuilder.Entity<Parameter>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<Parameter>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<pathway_groups>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<pathway_groups>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<pathway_groups>()
                .HasMany(e => e.pathways)
                .WithMany(e => e.pathway_groups)
                .Map(m => m.ToTable("pathway_to_pathway_groups").MapLeftKey("group_id").MapRightKey("pathway_id"));

            modelBuilder.Entity<pathway_links>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<pathway_processes>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<pathway_types>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<pathways>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<pathways>()
                .Property(e => e.status)
                .IsUnicode(false);

            modelBuilder.Entity<pathways>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<pathways>()
                .Property(e => e.layout)
                .IsUnicode(false);

            modelBuilder.Entity<pathways>()
                .HasMany(e => e.MapModelsPathways)
                .WithRequired(e => e.pathways)
                .HasForeignKey(e => e.pathwayId);

            modelBuilder.Entity<pathways>()
                .HasMany(e => e.pathway_links)
                .WithRequired(e => e.pathways)
                .HasForeignKey(e => e.pathway_id_1)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<pathways>()
                .HasMany(e => e.pathway_links1)
                .WithRequired(e => e.pathways1)
                .HasForeignKey(e => e.pathway_id_2)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<pathways>()
                .HasMany(e => e.pathway_processes)
                .WithRequired(e => e.pathways)
                .HasForeignKey(e => e.pathway_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<process_entities>()
                .Property(e => e.quantity)
                .IsUnicode(false);

            modelBuilder.Entity<process_entities>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<process_entity_roles>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<processes>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<processes>()
                .Property(e => e.location)
                .IsUnicode(false);

            modelBuilder.Entity<processes>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<processes>()
                .HasMany(e => e.MapReactionsProcessEntities)
                .WithRequired(e => e.processes)
                .HasForeignKey(e => e.processId);

            modelBuilder.Entity<processes>()
                .HasMany(e => e.pathway_processes)
                .WithRequired(e => e.processes)
                .HasForeignKey(e => e.process_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<processes>()
                .HasMany(e => e.process_entities)
                .WithRequired(e => e.processes)
                .HasForeignKey(e => e.process_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<processes>()
                .HasOptional(e => e.catalyzes)
                .WithRequired(e => e.processes)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Reaction>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<Reaction>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Reaction>()
                .HasMany(e => e.MapReactionECNumber)
                .WithRequired(e => e.Reaction)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reaction>()
                .HasMany(e => e.MapReactionsProcessEntities)
                .WithRequired(e => e.Reaction)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reaction>()
                .HasMany(e => e.ReactionBound)
                .WithRequired(e => e.Reaction)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reaction>()
                .HasOptional(e => e.ReactionBoundFix)
                .WithRequired(e => e.Reaction);

            modelBuilder.Entity<ReactionSpecies>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<ReactionSpecies>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<ReactionSpeciesRole>()
                .Property(e => e.role)
                .IsUnicode(false);

            modelBuilder.Entity<ReactionSpeciesRole>()
                .HasMany(e => e.ReactionSpecies)
                .WithRequired(e => e.ReactionSpeciesRole)
                .HasForeignKey(e => e.roleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<rna_types>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Rule>()
                .Property(e => e.variable)
                .IsUnicode(false);

            modelBuilder.Entity<Rule>()
                .Property(e => e.math)
                .IsUnicode(false);

            modelBuilder.Entity<RuleType>()
                .Property(e => e.type)
                .IsUnicode(false);

            modelBuilder.Entity<RuleType>()
                .HasMany(e => e.Rule)
                .WithRequired(e => e.RuleType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Sbase>()
                .Property(e => e.metaId)
                .IsUnicode(false);

            modelBuilder.Entity<Sbase>()
                .Property(e => e.sboTerm)
                .IsUnicode(false);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.Compartment)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.CompartmentType)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.Constraint)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.Event)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.EventAssignment)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.EventDelay)
                .WithRequired(e => e.Sbase)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.EventTrigger)
                .WithRequired(e => e.Sbase)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.FunctionDefinition)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.InitialAssignment)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.KineticLaw)
                .WithRequired(e => e.Sbase)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.Model)
                .WithRequired(e => e.Sbase)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.Parameter)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.Reaction)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.ReactionSpecies)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.Rule)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.Species)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.SpeciesType)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.StoichiometryMath)
                .WithRequired(e => e.Sbase)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.Unit)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Sbase>()
                .HasOptional(e => e.UnitDefinition)
                .WithRequired(e => e.Sbase);

            modelBuilder.Entity<Species>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<Species>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Species>()
                .HasMany(e => e.Formula)
                .WithRequired(e => e.Species)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Species>()
                .HasMany(e => e.CycleConnection)
                .WithRequired(e => e.Species)
                .HasForeignKey(e => e.metaboliteId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SpeciesType>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<SpeciesType>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<StoichiometryMath>()
                .Property(e => e.math)
                .IsUnicode(false);

            modelBuilder.Entity<Unit>()
                .HasMany(e => e.UnitDefinition1)
                .WithMany(e => e.Unit1)
                .Map(m => m.ToTable("UnitComposition").MapLeftKey("unitId").MapRightKey("unitDefinitionId"));

            modelBuilder.Entity<UnitDefinition>()
                .Property(e => e.sbmlId)
                .IsUnicode(false);

            modelBuilder.Entity<UnitDefinition>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<UnitDefinition>()
                .HasMany(e => e.Compartment)
                .WithOptional(e => e.UnitDefinition)
                .HasForeignKey(e => e.unitsId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<UnitDefinition>()
                .HasMany(e => e.Parameter)
                .WithOptional(e => e.UnitDefinition)
                .HasForeignKey(e => e.unitsId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<UnitDefinition>()
                .HasMany(e => e.Species)
                .WithOptional(e => e.UnitDefinition)
                .HasForeignKey(e => e.substanceUnitsId);

            modelBuilder.Entity<UnitDefinition>()
                .HasMany(e => e.Unit)
                .WithRequired(e => e.UnitDefinition)
                .HasForeignKey(e => e.kind);

            modelBuilder.Entity<viewState>()
                .Property(e => e.openSection)
                .IsUnicode(false);

            modelBuilder.Entity<viewState>()
                .Property(e => e.organism)
                .IsUnicode(false);

            modelBuilder.Entity<viewState>()
                .Property(e => e.openNode1Type)
                .IsUnicode(false);

            modelBuilder.Entity<viewState>()
                .Property(e => e.openNode2Type)
                .IsUnicode(false);

            modelBuilder.Entity<viewState>()
                .Property(e => e.openNode3Type)
                .IsUnicode(false);

            modelBuilder.Entity<viewState>()
                .Property(e => e.displayItemType)
                .IsUnicode(false);

            modelBuilder.Entity<catalyzes>()
                .Property(e => e.ec_number)
                .IsUnicode(false);

            modelBuilder.Entity<chromosome_bands>()
                .Property(e => e.chromosome_name)
                .IsUnicode(false);

            modelBuilder.Entity<chromosome_bands>()
                .Property(e => e.arm)
                .IsUnicode(false);

            modelBuilder.Entity<chromosome_bands>()
                .Property(e => e.band)
                .IsUnicode(false);

            modelBuilder.Entity<chromosome_bands>()
                .Property(e => e.stain)
                .IsUnicode(false);

            modelBuilder.Entity<chromosomes_pathcase>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<chromosomes_pathcase>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<CompartmentClass>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<ec_go>()
                .Property(e => e.ec_number)
                .IsUnicode(false);

            modelBuilder.Entity<ec_go>()
                .Property(e => e.go_id)
                .IsUnicode(false);

            modelBuilder.Entity<ec_go_orig>()
                .Property(e => e.ec_number)
                .IsUnicode(false);

            modelBuilder.Entity<ec_go_orig>()
                .Property(e => e.go_id)
                .IsUnicode(false);

            modelBuilder.Entity<external_database_links>()
                .Property(e => e.id_in_external_database)
                .IsUnicode(false);

            modelBuilder.Entity<external_database_links>()
                .Property(e => e.name_in_external_database)
                .IsUnicode(false);

            modelBuilder.Entity<external_database_urls>()
                .Property(e => e.type)
                .IsUnicode(false);

            modelBuilder.Entity<external_database_urls>()
                .Property(e => e.url_template)
                .IsUnicode(false);

            modelBuilder.Entity<external_databases>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<external_databases>()
                .Property(e => e.fullname)
                .IsUnicode(false);

            modelBuilder.Entity<external_databases>()
                .Property(e => e.url)
                .IsUnicode(false);

            modelBuilder.Entity<go_pathway_annotation_counts>()
                .Property(e => e.go_id)
                .IsUnicode(false);

            modelBuilder.Entity<go_pathway_group_annotation_counts>()
                .Property(e => e.go_id)
                .IsUnicode(false);

            modelBuilder.Entity<go_terms_hierarchy>()
                .Property(e => e.ParentID)
                .IsUnicode(false);

            modelBuilder.Entity<go_terms_hierarchy>()
                .Property(e => e.ChildID)
                .IsUnicode(false);

            modelBuilder.Entity<go_terms_hierarchy>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<MapSbaseGO>()
                .Property(e => e.goId)
                .IsUnicode(false);
        }
    }
}
