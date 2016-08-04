namespace Metabol.DbModels.Migrations.MetabolicNetworkContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remigrate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnnotationQualifier",
                c => new
                    {
                        id = c.Short(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.MapModelsPathways",
                c => new
                    {
                        modelId = c.Guid(nullable: false),
                        pathwayId = c.Guid(nullable: false),
                        qualifierId = c.Short(nullable: false),
                        organismGroupId = c.Guid(),
                    })
                .PrimaryKey(t => new { t.modelId, t.pathwayId, t.qualifierId })
                .ForeignKey("dbo.pathways", t => t.pathwayId, cascadeDelete: true)
                .ForeignKey("dbo.organism_groups", t => t.organismGroupId)
                .ForeignKey("dbo.Model", t => t.modelId, cascadeDelete: true)
                .ForeignKey("dbo.AnnotationQualifier", t => t.qualifierId)
                .Index(t => t.modelId)
                .Index(t => t.pathwayId)
                .Index(t => t.qualifierId)
                .Index(t => t.organismGroupId);
            
            CreateTable(
                "dbo.Model",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        sbmlId = c.String(maxLength: 100, unicode: false),
                        name = c.String(maxLength: 100, unicode: false),
                        sbmlLevel = c.Byte(),
                        sbmlVersion = c.Byte(),
                        dataSourceId = c.Short(),
                        sbmlFile = c.String(nullable: false, storeType: "ntext"),
                        sbmlFileName = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Sbase", t => t.id, cascadeDelete: true)
                .ForeignKey("dbo.DataSource", t => t.dataSourceId, cascadeDelete: true)
                .Index(t => t.id)
                .Index(t => t.dataSourceId);
            
            CreateTable(
                "dbo.Compartment",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(nullable: false),
                        sbmlId = c.String(maxLength: 100, unicode: false),
                        name = c.String(maxLength: 100, unicode: false),
                        compartmentTypeId = c.Guid(),
                        spatialDimensions = c.Byte(nullable: false),
                        size = c.Double(),
                        unitsId = c.Guid(),
                        compartmentClassId = c.Guid(),
                        outside = c.Guid(),
                        constant = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Compartment", t => t.outside)
                .ForeignKey("dbo.Model", t => t.modelId, cascadeDelete: true)
                .ForeignKey("dbo.Sbase", t => t.id)
                .ForeignKey("dbo.UnitDefinition", t => t.unitsId, cascadeDelete: true)
                .Index(t => t.id)
                .Index(t => t.modelId)
                .Index(t => t.unitsId)
                .Index(t => t.outside);
            
            CreateTable(
                "dbo.Sbase",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        metaId = c.String(maxLength: 100, unicode: false),
                        sboTerm = c.String(maxLength: 50, unicode: false),
                        notes = c.String(storeType: "ntext"),
                        annotation = c.String(storeType: "ntext"),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.CompartmentType",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(nullable: false),
                        sbmlId = c.String(maxLength: 100, unicode: false),
                        name = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Model", t => t.modelId, cascadeDelete: true)
                .ForeignKey("dbo.Sbase", t => t.id)
                .Index(t => t.id)
                .Index(t => t.modelId);
            
            CreateTable(
                "dbo.Constraint",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(nullable: false),
                        math = c.String(nullable: false, storeType: "xml"),
                        message = c.String(storeType: "xml"),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Model", t => t.modelId, cascadeDelete: true)
                .ForeignKey("dbo.Sbase", t => t.id)
                .Index(t => t.id)
                .Index(t => t.modelId);
            
            CreateTable(
                "dbo.Event",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(nullable: false),
                        sbmlId = c.String(maxLength: 100, unicode: false),
                        name = c.String(maxLength: 100, unicode: false),
                        eventTriggerId = c.Guid(nullable: false),
                        eventDelayId = c.Guid(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.EventDelay", t => t.eventDelayId)
                .ForeignKey("dbo.EventTrigger", t => t.eventTriggerId)
                .ForeignKey("dbo.Sbase", t => t.id)
                .ForeignKey("dbo.Model", t => t.modelId)
                .Index(t => t.id)
                .Index(t => t.modelId)
                .Index(t => t.eventTriggerId)
                .Index(t => t.eventDelayId);
            
            CreateTable(
                "dbo.EventAssignment",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        eventId = c.Guid(nullable: false),
                        variable = c.String(nullable: false, maxLength: 100, unicode: false),
                        math = c.String(nullable: false, storeType: "xml"),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Event", t => t.eventId, cascadeDelete: true)
                .ForeignKey("dbo.Sbase", t => t.id)
                .Index(t => t.id)
                .Index(t => t.eventId);
            
            CreateTable(
                "dbo.EventDelay",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        math = c.String(nullable: false, storeType: "xml"),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Sbase", t => t.id, cascadeDelete: true)
                .Index(t => t.id);
            
            CreateTable(
                "dbo.EventTrigger",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        math = c.String(nullable: false, storeType: "xml"),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Sbase", t => t.id, cascadeDelete: true)
                .Index(t => t.id);
            
            CreateTable(
                "dbo.FunctionDefinition",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(nullable: false),
                        sbmlId = c.String(nullable: false, maxLength: 100, unicode: false),
                        name = c.String(unicode: false),
                        lambda = c.String(nullable: false, storeType: "xml"),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Sbase", t => t.id)
                .ForeignKey("dbo.Model", t => t.modelId)
                .Index(t => t.id)
                .Index(t => t.modelId);
            
            CreateTable(
                "dbo.InitialAssignment",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(nullable: false),
                        symbol = c.String(nullable: false, maxLength: 100, unicode: false),
                        math = c.String(nullable: false, storeType: "xml"),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Model", t => t.modelId, cascadeDelete: true)
                .ForeignKey("dbo.Sbase", t => t.id)
                .Index(t => t.id)
                .Index(t => t.modelId);
            
            CreateTable(
                "dbo.KineticLaw",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        math = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Sbase", t => t.id, cascadeDelete: true)
                .Index(t => t.id);
            
            CreateTable(
                "dbo.Reaction",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(nullable: false),
                        sbmlId = c.String(unicode: false),
                        name = c.String(unicode: false),
                        reversible = c.Boolean(nullable: false),
                        fast = c.Boolean(nullable: false),
                        kineticLawId = c.Guid(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.KineticLaw", t => t.kineticLawId)
                .ForeignKey("dbo.Model", t => t.modelId, cascadeDelete: true)
                .ForeignKey("dbo.Sbase", t => t.id)
                .Index(t => t.id)
                .Index(t => t.modelId)
                .Index(t => t.kineticLawId);
            
            CreateTable(
                "dbo.FixedReactions",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        reactionId = c.Guid(nullable: false),
                        lowerBound = c.Single(nullable: false),
                        upperBound = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.id, t.reactionId, t.lowerBound, t.upperBound })
                .ForeignKey("dbo.Reaction", t => t.reactionId)
                .Index(t => t.reactionId);
            
            CreateTable(
                "dbo.MapReactionECNumber",
                c => new
                    {
                        reactionId = c.Guid(nullable: false),
                        ecNumber = c.String(nullable: false, maxLength: 20, unicode: false),
                        qualifierId = c.Short(nullable: false),
                    })
                .PrimaryKey(t => new { t.reactionId, t.ecNumber, t.qualifierId })
                .ForeignKey("dbo.ec_numbers", t => t.ecNumber)
                .ForeignKey("dbo.Reaction", t => t.reactionId)
                .ForeignKey("dbo.AnnotationQualifier", t => t.qualifierId)
                .Index(t => t.reactionId)
                .Index(t => t.ecNumber)
                .Index(t => t.qualifierId);
            
            CreateTable(
                "dbo.ec_numbers",
                c => new
                    {
                        ec_number = c.String(nullable: false, maxLength: 20, unicode: false),
                        name = c.String(nullable: false, maxLength: 255, unicode: false),
                        nodeCode = c.String(maxLength: 20, unicode: false),
                    })
                .PrimaryKey(t => t.ec_number);
            
            CreateTable(
                "dbo.catalyzes",
                c => new
                    {
                        process_id = c.Guid(nullable: false),
                        organism_group_id = c.Guid(),
                        gene_product_id = c.Guid(),
                        ec_number = c.String(maxLength: 20, unicode: false),
                    })
                .PrimaryKey(t => t.process_id)
                .ForeignKey("dbo.ec_numbers", t => t.ec_number)
                .ForeignKey("dbo.gene_products", t => t.gene_product_id)
                .ForeignKey("dbo.processes", t => t.process_id, cascadeDelete: true)
                .ForeignKey("dbo.organism_groups", t => t.organism_group_id)
                .Index(t => t.process_id)
                .Index(t => t.organism_group_id)
                .Index(t => t.gene_product_id)
                .Index(t => t.ec_number);
            
            CreateTable(
                "dbo.gene_products",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        notes = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.molecular_entities", t => t.id)
                .Index(t => t.id);
            
            CreateTable(
                "dbo.gene_encodings",
                c => new
                    {
                        gene_id = c.Guid(nullable: false),
                        gene_product_id = c.Guid(nullable: false),
                        notes = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => new { t.gene_id, t.gene_product_id })
                .ForeignKey("dbo.gene_products", t => t.gene_product_id)
                .Index(t => t.gene_product_id);
            
            CreateTable(
                "dbo.molecular_entities",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        type_id = c.Byte(nullable: false),
                        name = c.String(maxLength: 255, unicode: false),
                        notes = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.basic_molecules",
                c => new
                    {
                        id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.molecular_entities", t => t.id)
                .Index(t => t.id);
            
            CreateTable(
                "dbo.common_molecules",
                c => new
                    {
                        id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.basic_molecules", t => t.id)
                .Index(t => t.id);
            
            CreateTable(
                "dbo.entity_name_lookups",
                c => new
                    {
                        entity_id = c.Guid(nullable: false),
                        name_id = c.Guid(nullable: false),
                        name_type_id = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => new { t.entity_id, t.name_id })
                .ForeignKey("dbo.molecular_entity_names", t => t.name_id)
                .ForeignKey("dbo.molecular_entities", t => t.entity_id, cascadeDelete: true)
                .Index(t => t.entity_id)
                .Index(t => t.name_id);
            
            CreateTable(
                "dbo.molecular_entity_names",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        name = c.String(nullable: false, maxLength: 255, unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.ec_number_name_lookups",
                c => new
                    {
                        ec_number = c.String(nullable: false, maxLength: 20, unicode: false),
                        name_id = c.Guid(nullable: false),
                        name_type_id = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => new { t.ec_number, t.name_id })
                .ForeignKey("dbo.molecular_entity_names", t => t.name_id)
                .ForeignKey("dbo.ec_numbers", t => t.ec_number)
                .Index(t => t.ec_number)
                .Index(t => t.name_id);
            
            CreateTable(
                "dbo.MapSpeciesMolecularEntities",
                c => new
                    {
                        speciesId = c.Guid(nullable: false),
                        qualifierId = c.Short(nullable: false),
                        molecularEntityId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.speciesId, t.qualifierId, t.molecularEntityId })
                .ForeignKey("dbo.Species", t => t.speciesId, cascadeDelete: true)
                .ForeignKey("dbo.molecular_entities", t => t.molecularEntityId)
                .ForeignKey("dbo.AnnotationQualifier", t => t.qualifierId)
                .Index(t => t.speciesId)
                .Index(t => t.qualifierId)
                .Index(t => t.molecularEntityId);
            
            CreateTable(
                "dbo.Species",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(nullable: false),
                        sbmlId = c.String(unicode: false),
                        name = c.String(unicode: false),
                        speciesTypeId = c.Guid(),
                        compartmentId = c.Guid(nullable: false),
                        initialAmount = c.Double(),
                        initialConcentration = c.Double(),
                        substanceUnitsId = c.Guid(),
                        hasOnlySubstanceUnits = c.Boolean(nullable: false),
                        boundaryCondition = c.Boolean(nullable: false),
                        charge = c.Int(),
                        constant = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.SpeciesType", t => t.speciesTypeId)
                .ForeignKey("dbo.UnitDefinition", t => t.substanceUnitsId)
                .ForeignKey("dbo.Sbase", t => t.id)
                .ForeignKey("dbo.Compartment", t => t.compartmentId)
                .Index(t => t.id)
                .Index(t => t.speciesTypeId)
                .Index(t => t.compartmentId)
                .Index(t => t.substanceUnitsId);
            
            CreateTable(
                "dbo.CycleConnection",
                c => new
                    {
                        cycleId = c.Guid(nullable: false),
                        metaboliteId = c.Guid(nullable: false),
                        roleId = c.Byte(nullable: false),
                        stoichiometry = c.Double(nullable: false),
                        isReversible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.cycleId, t.metaboliteId, t.roleId, t.stoichiometry, t.isReversible })
                .ForeignKey("dbo.Cycle", t => t.cycleId)
                .ForeignKey("dbo.Species", t => t.metaboliteId)
                .Index(t => t.cycleId)
                .Index(t => t.metaboliteId);
            
            CreateTable(
                "dbo.Cycle",
                c => new
                    {
                        id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.CycleReaction",
                c => new
                    {
                        cycleId = c.Guid(nullable: false),
                        otherId = c.Guid(nullable: false),
                        isReaction = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.cycleId, t.otherId, t.isReaction })
                .ForeignKey("dbo.Cycle", t => t.cycleId)
                .Index(t => t.cycleId);
            
            CreateTable(
                "dbo.MetaboliteReactionCount",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        speciesId = c.Guid(nullable: false),
                        consumerCount = c.Int(nullable: false),
                        producerCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Species", t => t.speciesId, cascadeDelete: true)
                .Index(t => t.speciesId);
            
            CreateTable(
                "dbo.MetaboliteReactionStoichiometry",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        speciesId = c.Guid(nullable: false),
                        consumerStoch = c.Double(nullable: false),
                        producerStoch = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Species", t => t.speciesId, cascadeDelete: true)
                .Index(t => t.speciesId);
            
            CreateTable(
                "dbo.ReactionSpecies",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        reactionId = c.Guid(nullable: false),
                        speciesId = c.Guid(nullable: false),
                        roleId = c.Byte(nullable: false),
                        stoichiometry = c.Double(nullable: false),
                        stoichiometryMathId = c.Guid(),
                        sbmlId = c.String(maxLength: 500, unicode: false),
                        name = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Reaction", t => t.reactionId, cascadeDelete: true)
                .ForeignKey("dbo.ReactionSpeciesRole", t => t.roleId)
                .ForeignKey("dbo.Species", t => t.speciesId, cascadeDelete: true)
                .ForeignKey("dbo.StoichiometryMath", t => t.stoichiometryMathId)
                .ForeignKey("dbo.Sbase", t => t.id)
                .Index(t => t.id)
                .Index(t => t.reactionId)
                .Index(t => t.speciesId)
                .Index(t => t.roleId)
                .Index(t => t.stoichiometryMathId);
            
            CreateTable(
                "dbo.ReactionSpeciesRole",
                c => new
                    {
                        id = c.Byte(nullable: false),
                        role = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.StoichiometryMath",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        math = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Sbase", t => t.id, cascadeDelete: true)
                .Index(t => t.id);
            
            CreateTable(
                "dbo.SpeciesType",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(nullable: false),
                        sbmlId = c.String(nullable: false, maxLength: 100, unicode: false),
                        name = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Model", t => t.modelId, cascadeDelete: true)
                .ForeignKey("dbo.Sbase", t => t.id)
                .Index(t => t.id)
                .Index(t => t.modelId);
            
            CreateTable(
                "dbo.UnitDefinition",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(),
                        sbmlId = c.String(nullable: false, maxLength: 100, unicode: false),
                        name = c.String(maxLength: 100, unicode: false),
                        isBaseUnit = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Model", t => t.modelId)
                .ForeignKey("dbo.Sbase", t => t.id)
                .Index(t => t.id)
                .Index(t => t.modelId);
            
            CreateTable(
                "dbo.Parameter",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(),
                        reactionId = c.Guid(),
                        sbmlId = c.String(nullable: false, maxLength: 100, unicode: false),
                        name = c.String(maxLength: 100, unicode: false),
                        value = c.Double(),
                        unitsId = c.Guid(),
                        constant = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.UnitDefinition", t => t.unitsId, cascadeDelete: true)
                .ForeignKey("dbo.Sbase", t => t.id)
                .ForeignKey("dbo.Model", t => t.modelId, cascadeDelete: true)
                .Index(t => t.id)
                .Index(t => t.modelId)
                .Index(t => t.unitsId);
            
            CreateTable(
                "dbo.Unit",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(),
                        kind = c.Guid(nullable: false),
                        exponent = c.Int(nullable: false),
                        scale = c.Int(nullable: false),
                        multiplier = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.UnitDefinition", t => t.kind, cascadeDelete: true)
                .ForeignKey("dbo.Sbase", t => t.id)
                .ForeignKey("dbo.Model", t => t.modelId, cascadeDelete: true)
                .Index(t => t.id)
                .Index(t => t.modelId)
                .Index(t => t.kind);
            
            CreateTable(
                "dbo.pathway_links",
                c => new
                    {
                        pathway_id_1 = c.Guid(nullable: false),
                        pathway_id_2 = c.Guid(nullable: false),
                        entity_id = c.Guid(nullable: false),
                        notes = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => new { t.pathway_id_1, t.pathway_id_2, t.entity_id })
                .ForeignKey("dbo.pathways", t => t.pathway_id_1)
                .ForeignKey("dbo.pathways", t => t.pathway_id_2)
                .ForeignKey("dbo.molecular_entities", t => t.entity_id)
                .Index(t => t.pathway_id_1)
                .Index(t => t.pathway_id_2)
                .Index(t => t.entity_id);
            
            CreateTable(
                "dbo.pathways",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        name = c.String(nullable: false, maxLength: 100, unicode: false),
                        pathway_type_id = c.Byte(nullable: false),
                        status = c.String(maxLength: 255, unicode: false),
                        notes = c.String(unicode: false, storeType: "text"),
                        layout = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.pathway_groups",
                c => new
                    {
                        group_id = c.Guid(nullable: false),
                        name = c.String(nullable: false, maxLength: 100, unicode: false),
                        notes = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => t.group_id);
            
            CreateTable(
                "dbo.pathway_processes",
                c => new
                    {
                        pathway_id = c.Guid(nullable: false),
                        process_id = c.Guid(nullable: false),
                        notes = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => new { t.pathway_id, t.process_id })
                .ForeignKey("dbo.processes", t => t.process_id)
                .ForeignKey("dbo.pathways", t => t.pathway_id)
                .Index(t => t.pathway_id)
                .Index(t => t.process_id);
            
            CreateTable(
                "dbo.processes",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        name = c.String(nullable: false, maxLength: 800, unicode: false),
                        reversible = c.Boolean(),
                        location = c.String(maxLength: 100, unicode: false),
                        notes = c.String(unicode: false, storeType: "text"),
                        generic_process_id = c.Guid(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.MapReactionsProcessEntities",
                c => new
                    {
                        reactionId = c.Guid(nullable: false),
                        processId = c.Guid(nullable: false),
                        qualifierId = c.Short(nullable: false),
                    })
                .PrimaryKey(t => new { t.reactionId, t.processId, t.qualifierId })
                .ForeignKey("dbo.processes", t => t.processId, cascadeDelete: true)
                .ForeignKey("dbo.Reaction", t => t.reactionId)
                .ForeignKey("dbo.AnnotationQualifier", t => t.qualifierId)
                .Index(t => t.reactionId)
                .Index(t => t.processId)
                .Index(t => t.qualifierId);
            
            CreateTable(
                "dbo.process_entities",
                c => new
                    {
                        process_id = c.Guid(nullable: false),
                        entity_id = c.Guid(nullable: false),
                        role_id = c.Byte(nullable: false),
                        quantity = c.String(nullable: false, maxLength: 10, unicode: false),
                        notes = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => new { t.process_id, t.entity_id, t.role_id })
                .ForeignKey("dbo.processes", t => t.process_id)
                .ForeignKey("dbo.molecular_entities", t => t.entity_id)
                .Index(t => t.process_id)
                .Index(t => t.entity_id);
            
            CreateTable(
                "dbo.proteins",
                c => new
                    {
                        id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.gene_products", t => t.id)
                .Index(t => t.id);
            
            CreateTable(
                "dbo.rnas",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        rna_type_id = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.gene_products", t => t.id)
                .Index(t => t.id);
            
            CreateTable(
                "dbo.organism_groups",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        scientific_name = c.String(maxLength: 100, unicode: false),
                        common_name = c.String(maxLength: 100, unicode: false),
                        parent_id = c.Guid(),
                        notes = c.String(unicode: false, storeType: "text"),
                        is_organism = c.Boolean(nullable: false),
                        nodeLabel = c.String(maxLength: 500, unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.organism_groups", t => t.parent_id)
                .Index(t => t.parent_id);
            
            CreateTable(
                "dbo.genes",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        organism_group_id = c.Guid(nullable: false),
                        chromosome_id = c.Guid(),
                        homologue_group_id = c.Guid(nullable: false),
                        raw_address = c.String(maxLength: 8000, unicode: false),
                        cytogenic_address = c.String(maxLength: 100, unicode: false),
                        genetic_address = c.Long(),
                        relative_address = c.Long(),
                        notes = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.chromosomes", t => t.chromosome_id)
                .ForeignKey("dbo.organism_groups", t => t.organism_group_id, cascadeDelete: true)
                .Index(t => t.organism_group_id)
                .Index(t => t.chromosome_id);
            
            CreateTable(
                "dbo.chromosomes",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        organism_group_id = c.Guid(),
                        name = c.String(nullable: false, maxLength: 100, unicode: false),
                        length = c.Long(),
                        centromere_location = c.Int(nullable: false),
                        notes = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.ModelOrganism",
                c => new
                    {
                        modelId = c.Guid(nullable: false),
                        NCBITaxonomyId = c.Int(nullable: false),
                        qualifierId = c.Short(nullable: false),
                        organismGroupId = c.Guid(),
                    })
                .PrimaryKey(t => new { t.modelId, t.NCBITaxonomyId, t.qualifierId })
                .ForeignKey("dbo.organism_groups", t => t.organismGroupId)
                .ForeignKey("dbo.Model", t => t.modelId)
                .ForeignKey("dbo.AnnotationQualifier", t => t.qualifierId)
                .Index(t => t.modelId)
                .Index(t => t.qualifierId)
                .Index(t => t.organismGroupId);
            
            CreateTable(
                "dbo.ReactionBound",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        reactionId = c.Guid(nullable: false),
                        lowerBound = c.Int(nullable: false),
                        upperBound = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Reaction", t => t.reactionId)
                .Index(t => t.reactionId);
            
            CreateTable(
                "dbo.Rule",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        modelId = c.Guid(nullable: false),
                        variable = c.String(maxLength: 100, unicode: false),
                        math = c.String(nullable: false, unicode: false),
                        ruleTypeId = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Model", t => t.modelId, cascadeDelete: true)
                .ForeignKey("dbo.RuleType", t => t.ruleTypeId)
                .ForeignKey("dbo.Sbase", t => t.id)
                .Index(t => t.id)
                .Index(t => t.modelId)
                .Index(t => t.ruleTypeId);
            
            CreateTable(
                "dbo.RuleType",
                c => new
                    {
                        id = c.Byte(nullable: false),
                        type = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.DataSource",
                c => new
                    {
                        id = c.Short(nullable: false),
                        name = c.String(nullable: false, maxLength: 100, unicode: false),
                        url = c.String(maxLength: 200, unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.ModelLayout",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        layout = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Model", t => t.id)
                .Index(t => t.id);
            
            CreateTable(
                "dbo.attribute_names",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 100, unicode: false),
                        attributeId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.attribute_values",
                c => new
                    {
                        attributeId = c.Int(nullable: false),
                        itemId = c.Guid(nullable: false),
                        value = c.String(maxLength: 800, unicode: false),
                        textValue = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => new { t.attributeId, t.itemId });
            
            CreateTable(
                "dbo.Author",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(maxLength: 200),
                        Surname = c.String(maxLength: 200),
                        EMail = c.String(maxLength: 200),
                        OrgName = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DesignedBy",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ModelMetadataId = c.Guid(nullable: false),
                        AuthorId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ModelMetadata", t => t.ModelMetadataId)
                .ForeignKey("dbo.Author", t => t.AuthorId)
                .Index(t => t.ModelMetadataId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.ModelMetadata",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ModelName = c.String(maxLength: 500),
                        PublicationId = c.Int(),
                        CreationDate = c.DateTime(),
                        ModificationDate = c.DateTime(),
                        Notes = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.chromosome_bands",
                c => new
                    {
                        chromosome_id = c.Guid(nullable: false),
                        chromosome_name = c.String(maxLength: 50, unicode: false),
                        arm = c.String(maxLength: 10, unicode: false),
                        band = c.String(maxLength: 20, unicode: false),
                        iscn_start = c.Int(),
                        iscn_stop = c.Int(),
                        bp_start = c.Int(),
                        bp_stop = c.Int(),
                        stain = c.String(maxLength: 20, unicode: false),
                        density = c.Double(),
                        bases = c.Long(),
                    })
                .PrimaryKey(t => t.chromosome_id);
            
            CreateTable(
                "dbo.chromosomes_pathcase",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        name = c.String(nullable: false, maxLength: 10, unicode: false),
                        centromere_location = c.Int(nullable: false),
                        organism_group_id = c.Guid(),
                        length = c.Long(),
                        notes = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => new { t.id, t.name, t.centromere_location });
            
            CreateTable(
                "dbo.common_species",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.CompartmentClass",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        name = c.String(unicode: false),
                        parentid = c.Guid(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.ec_go",
                c => new
                    {
                        ec_number = c.String(nullable: false, maxLength: 50, unicode: false),
                        go_id = c.String(maxLength: 10, unicode: false),
                    })
                .PrimaryKey(t => t.ec_number);
            
            CreateTable(
                "dbo.ec_go_orig",
                c => new
                    {
                        ec_number = c.String(nullable: false, maxLength: 10, unicode: false),
                        go_id = c.String(nullable: false, maxLength: 7, unicode: false),
                    })
                .PrimaryKey(t => new { t.ec_number, t.go_id });
            
            CreateTable(
                "dbo.entity_graph_nodes",
                c => new
                    {
                        entityId = c.Guid(nullable: false),
                        pathwayId = c.Guid(),
                        graphNodeId = c.Guid(),
                    })
                .PrimaryKey(t => t.entityId);
            
            CreateTable(
                "dbo.external_database_links",
                c => new
                    {
                        local_id = c.Guid(nullable: false),
                        external_database_id = c.Int(nullable: false),
                        id_in_external_database = c.String(nullable: false, maxLength: 100, unicode: false),
                        name_in_external_database = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => new { t.local_id, t.external_database_id, t.id_in_external_database });
            
            CreateTable(
                "dbo.external_database_urls",
                c => new
                    {
                        external_database_id = c.Int(nullable: false),
                        type = c.String(nullable: false, maxLength: 16, unicode: false),
                        url_template = c.String(nullable: false, maxLength: 256, unicode: false),
                    })
                .PrimaryKey(t => new { t.external_database_id, t.type, t.url_template });
            
            CreateTable(
                "dbo.external_databases",
                c => new
                    {
                        id = c.Int(nullable: false),
                        name = c.String(maxLength: 100, unicode: false),
                        fullname = c.String(maxLength: 256, unicode: false),
                        url = c.String(maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.go_annotation_pathways",
                c => new
                    {
                        pathway_id = c.Guid(nullable: false),
                        go_level = c.Int(nullable: false),
                        serialized_image = c.Binary(nullable: false, storeType: "image"),
                        serialized_image_map = c.String(nullable: false, storeType: "ntext"),
                        date_generated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.pathway_id, t.go_level, t.serialized_image, t.serialized_image_map, t.date_generated });
            
            CreateTable(
                "dbo.go_pathway_annotation_counts",
                c => new
                    {
                        go_id = c.String(nullable: false, maxLength: 7, unicode: false),
                        hierarchy_level = c.Int(nullable: false),
                        number_annotations = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.go_id, t.hierarchy_level, t.number_annotations });
            
            CreateTable(
                "dbo.go_pathway_group_annotation_counts",
                c => new
                    {
                        pathway_group_id = c.Guid(nullable: false),
                        go_id = c.String(nullable: false, maxLength: 7, unicode: false),
                        hierarchy_level = c.Int(nullable: false),
                        number_annotations = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.pathway_group_id, t.go_id, t.hierarchy_level, t.number_annotations });
            
            CreateTable(
                "dbo.go_terms",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 7, unicode: false),
                        Name = c.String(nullable: false, maxLength: 200, unicode: false),
                        SubtreeHeight = c.Int(nullable: false),
                        TotalDescendants = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.GONodeCodes",
                c => new
                    {
                        goid = c.String(nullable: false, maxLength: 7, unicode: false),
                        nodeCode = c.String(nullable: false, maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => new { t.goid, t.nodeCode })
                .ForeignKey("dbo.go_terms", t => t.goid)
                .Index(t => t.goid);
            
            CreateTable(
                "dbo.go_terms_hierarchy",
                c => new
                    {
                        ParentID = c.String(nullable: false, maxLength: 7, unicode: false),
                        ChildID = c.String(nullable: false, maxLength: 7, unicode: false),
                        Type = c.String(nullable: false, maxLength: 10, unicode: false),
                        TermLevel = c.Int(),
                        OnPathUnderCatalyticActivity = c.Boolean(),
                    })
                .PrimaryKey(t => new { t.ParentID, t.ChildID, t.Type });
            
            CreateTable(
                "dbo.MapSbaseGO",
                c => new
                    {
                        sbaseId = c.Guid(nullable: false),
                        goId = c.String(nullable: false, maxLength: 7, unicode: false),
                        qualifierId = c.Short(nullable: false),
                    })
                .PrimaryKey(t => new { t.sbaseId, t.goId, t.qualifierId });
            
            CreateTable(
                "dbo.molecular_entity_types",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 50, unicode: false),
                        type_id = c.Byte(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.name_types",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 50, unicode: false),
                        name_type_id = c.Byte(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.OLD_external_database_links",
                c => new
                    {
                        local_id = c.Guid(nullable: false),
                        external_database_id = c.Guid(nullable: false),
                        id_in_external_database = c.String(nullable: false, maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => new { t.local_id, t.external_database_id, t.id_in_external_database })
                .ForeignKey("dbo.OLD_external_databases", t => t.external_database_id)
                .Index(t => t.external_database_id);
            
            CreateTable(
                "dbo.OLD_external_databases",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        name = c.String(nullable: false, maxLength: 100, unicode: false),
                        notes = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.organisms",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        taxonomy_id = c.String(maxLength: 20, unicode: false),
                        cM_unit_length = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.pathway_types",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 100, unicode: false),
                        pathway_type_id = c.Byte(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.process_entity_roles",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 18, unicode: false),
                        role_id = c.Byte(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.process_graph_nodes",
                c => new
                    {
                        genericProcessId = c.Guid(nullable: false),
                        pathwayId = c.Guid(),
                        graphNodeId = c.Guid(),
                    })
                .PrimaryKey(t => t.genericProcessId);
            
            CreateTable(
                "dbo.rna_types",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 4, unicode: false),
                        rna_type_id = c.Byte(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.sysdiagrams",
                c => new
                    {
                        diagram_id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 128),
                        principal_id = c.Int(nullable: false),
                        version = c.Int(),
                        definition = c.Binary(),
                    })
                .PrimaryKey(t => t.diagram_id);
            
            CreateTable(
                "dbo.viewState",
                c => new
                    {
                        viewID = c.Guid(nullable: false),
                        openSection = c.String(maxLength: 32, unicode: false),
                        organism = c.String(maxLength: 32, unicode: false),
                        openNode1ID = c.Guid(),
                        openNode1Type = c.String(maxLength: 32, unicode: false),
                        openNode2ID = c.Guid(),
                        openNode2Type = c.String(maxLength: 32, unicode: false),
                        openNode3ID = c.Guid(),
                        openNode3Type = c.String(maxLength: 32, unicode: false),
                        displayItemID = c.Guid(),
                        displayItemType = c.String(maxLength: 32, unicode: false),
                        viewGraph = c.Byte(),
                        timeStamp = c.DateTime(),
                    })
                .PrimaryKey(t => t.viewID);
            
            CreateTable(
                "dbo.UnitComposition",
                c => new
                    {
                        unitId = c.Guid(nullable: false),
                        unitDefinitionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.unitId, t.unitDefinitionId })
                .ForeignKey("dbo.Unit", t => t.unitId, cascadeDelete: true)
                .ForeignKey("dbo.UnitDefinition", t => t.unitDefinitionId, cascadeDelete: true)
                .Index(t => t.unitId)
                .Index(t => t.unitDefinitionId);
            
            CreateTable(
                "dbo.pathway_to_pathway_groups",
                c => new
                    {
                        group_id = c.Guid(nullable: false),
                        pathway_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.group_id, t.pathway_id })
                .ForeignKey("dbo.pathway_groups", t => t.group_id, cascadeDelete: true)
                .ForeignKey("dbo.pathways", t => t.pathway_id, cascadeDelete: true)
                .Index(t => t.group_id)
                .Index(t => t.pathway_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OLD_external_database_links", "external_database_id", "dbo.OLD_external_databases");
            DropForeignKey("dbo.GONodeCodes", "goid", "dbo.go_terms");
            DropForeignKey("dbo.DesignedBy", "AuthorId", "dbo.Author");
            DropForeignKey("dbo.DesignedBy", "ModelMetadataId", "dbo.ModelMetadata");
            DropForeignKey("dbo.ModelOrganism", "qualifierId", "dbo.AnnotationQualifier");
            DropForeignKey("dbo.MapSpeciesMolecularEntities", "qualifierId", "dbo.AnnotationQualifier");
            DropForeignKey("dbo.MapReactionsProcessEntities", "qualifierId", "dbo.AnnotationQualifier");
            DropForeignKey("dbo.MapReactionECNumber", "qualifierId", "dbo.AnnotationQualifier");
            DropForeignKey("dbo.MapModelsPathways", "qualifierId", "dbo.AnnotationQualifier");
            DropForeignKey("dbo.Unit", "modelId", "dbo.Model");
            DropForeignKey("dbo.Parameter", "modelId", "dbo.Model");
            DropForeignKey("dbo.ModelOrganism", "modelId", "dbo.Model");
            DropForeignKey("dbo.ModelLayout", "id", "dbo.Model");
            DropForeignKey("dbo.MapModelsPathways", "modelId", "dbo.Model");
            DropForeignKey("dbo.FunctionDefinition", "modelId", "dbo.Model");
            DropForeignKey("dbo.Event", "modelId", "dbo.Model");
            DropForeignKey("dbo.Model", "dataSourceId", "dbo.DataSource");
            DropForeignKey("dbo.Species", "compartmentId", "dbo.Compartment");
            DropForeignKey("dbo.UnitDefinition", "id", "dbo.Sbase");
            DropForeignKey("dbo.Unit", "id", "dbo.Sbase");
            DropForeignKey("dbo.StoichiometryMath", "id", "dbo.Sbase");
            DropForeignKey("dbo.SpeciesType", "id", "dbo.Sbase");
            DropForeignKey("dbo.Species", "id", "dbo.Sbase");
            DropForeignKey("dbo.Rule", "id", "dbo.Sbase");
            DropForeignKey("dbo.Rule", "ruleTypeId", "dbo.RuleType");
            DropForeignKey("dbo.Rule", "modelId", "dbo.Model");
            DropForeignKey("dbo.ReactionSpecies", "id", "dbo.Sbase");
            DropForeignKey("dbo.Reaction", "id", "dbo.Sbase");
            DropForeignKey("dbo.Parameter", "id", "dbo.Sbase");
            DropForeignKey("dbo.Model", "id", "dbo.Sbase");
            DropForeignKey("dbo.KineticLaw", "id", "dbo.Sbase");
            DropForeignKey("dbo.ReactionBound", "reactionId", "dbo.Reaction");
            DropForeignKey("dbo.Reaction", "modelId", "dbo.Model");
            DropForeignKey("dbo.MapReactionsProcessEntities", "reactionId", "dbo.Reaction");
            DropForeignKey("dbo.MapReactionECNumber", "reactionId", "dbo.Reaction");
            DropForeignKey("dbo.MapReactionECNumber", "ecNumber", "dbo.ec_numbers");
            DropForeignKey("dbo.ec_number_name_lookups", "ec_number", "dbo.ec_numbers");
            DropForeignKey("dbo.organism_groups", "parent_id", "dbo.organism_groups");
            DropForeignKey("dbo.ModelOrganism", "organismGroupId", "dbo.organism_groups");
            DropForeignKey("dbo.MapModelsPathways", "organismGroupId", "dbo.organism_groups");
            DropForeignKey("dbo.genes", "organism_group_id", "dbo.organism_groups");
            DropForeignKey("dbo.genes", "chromosome_id", "dbo.chromosomes");
            DropForeignKey("dbo.catalyzes", "organism_group_id", "dbo.organism_groups");
            DropForeignKey("dbo.rnas", "id", "dbo.gene_products");
            DropForeignKey("dbo.proteins", "id", "dbo.gene_products");
            DropForeignKey("dbo.process_entities", "entity_id", "dbo.molecular_entities");
            DropForeignKey("dbo.pathway_links", "entity_id", "dbo.molecular_entities");
            DropForeignKey("dbo.pathway_processes", "pathway_id", "dbo.pathways");
            DropForeignKey("dbo.process_entities", "process_id", "dbo.processes");
            DropForeignKey("dbo.pathway_processes", "process_id", "dbo.processes");
            DropForeignKey("dbo.MapReactionsProcessEntities", "processId", "dbo.processes");
            DropForeignKey("dbo.catalyzes", "process_id", "dbo.processes");
            DropForeignKey("dbo.pathway_links", "pathway_id_2", "dbo.pathways");
            DropForeignKey("dbo.pathway_links", "pathway_id_1", "dbo.pathways");
            DropForeignKey("dbo.pathway_to_pathway_groups", "pathway_id", "dbo.pathways");
            DropForeignKey("dbo.pathway_to_pathway_groups", "group_id", "dbo.pathway_groups");
            DropForeignKey("dbo.MapModelsPathways", "pathwayId", "dbo.pathways");
            DropForeignKey("dbo.MapSpeciesMolecularEntities", "molecularEntityId", "dbo.molecular_entities");
            DropForeignKey("dbo.Unit", "kind", "dbo.UnitDefinition");
            DropForeignKey("dbo.UnitComposition", "unitDefinitionId", "dbo.UnitDefinition");
            DropForeignKey("dbo.UnitComposition", "unitId", "dbo.Unit");
            DropForeignKey("dbo.Species", "substanceUnitsId", "dbo.UnitDefinition");
            DropForeignKey("dbo.Parameter", "unitsId", "dbo.UnitDefinition");
            DropForeignKey("dbo.UnitDefinition", "modelId", "dbo.Model");
            DropForeignKey("dbo.Compartment", "unitsId", "dbo.UnitDefinition");
            DropForeignKey("dbo.Species", "speciesTypeId", "dbo.SpeciesType");
            DropForeignKey("dbo.SpeciesType", "modelId", "dbo.Model");
            DropForeignKey("dbo.ReactionSpecies", "stoichiometryMathId", "dbo.StoichiometryMath");
            DropForeignKey("dbo.ReactionSpecies", "speciesId", "dbo.Species");
            DropForeignKey("dbo.ReactionSpecies", "roleId", "dbo.ReactionSpeciesRole");
            DropForeignKey("dbo.ReactionSpecies", "reactionId", "dbo.Reaction");
            DropForeignKey("dbo.MetaboliteReactionStoichiometry", "speciesId", "dbo.Species");
            DropForeignKey("dbo.MetaboliteReactionCount", "speciesId", "dbo.Species");
            DropForeignKey("dbo.MapSpeciesMolecularEntities", "speciesId", "dbo.Species");
            DropForeignKey("dbo.CycleConnection", "metaboliteId", "dbo.Species");
            DropForeignKey("dbo.CycleReaction", "cycleId", "dbo.Cycle");
            DropForeignKey("dbo.CycleConnection", "cycleId", "dbo.Cycle");
            DropForeignKey("dbo.gene_products", "id", "dbo.molecular_entities");
            DropForeignKey("dbo.entity_name_lookups", "entity_id", "dbo.molecular_entities");
            DropForeignKey("dbo.entity_name_lookups", "name_id", "dbo.molecular_entity_names");
            DropForeignKey("dbo.ec_number_name_lookups", "name_id", "dbo.molecular_entity_names");
            DropForeignKey("dbo.basic_molecules", "id", "dbo.molecular_entities");
            DropForeignKey("dbo.common_molecules", "id", "dbo.basic_molecules");
            DropForeignKey("dbo.gene_encodings", "gene_product_id", "dbo.gene_products");
            DropForeignKey("dbo.catalyzes", "gene_product_id", "dbo.gene_products");
            DropForeignKey("dbo.catalyzes", "ec_number", "dbo.ec_numbers");
            DropForeignKey("dbo.Reaction", "kineticLawId", "dbo.KineticLaw");
            DropForeignKey("dbo.FixedReactions", "reactionId", "dbo.Reaction");
            DropForeignKey("dbo.InitialAssignment", "id", "dbo.Sbase");
            DropForeignKey("dbo.InitialAssignment", "modelId", "dbo.Model");
            DropForeignKey("dbo.FunctionDefinition", "id", "dbo.Sbase");
            DropForeignKey("dbo.EventTrigger", "id", "dbo.Sbase");
            DropForeignKey("dbo.EventDelay", "id", "dbo.Sbase");
            DropForeignKey("dbo.EventAssignment", "id", "dbo.Sbase");
            DropForeignKey("dbo.Event", "id", "dbo.Sbase");
            DropForeignKey("dbo.Event", "eventTriggerId", "dbo.EventTrigger");
            DropForeignKey("dbo.Event", "eventDelayId", "dbo.EventDelay");
            DropForeignKey("dbo.EventAssignment", "eventId", "dbo.Event");
            DropForeignKey("dbo.Constraint", "id", "dbo.Sbase");
            DropForeignKey("dbo.Constraint", "modelId", "dbo.Model");
            DropForeignKey("dbo.CompartmentType", "id", "dbo.Sbase");
            DropForeignKey("dbo.CompartmentType", "modelId", "dbo.Model");
            DropForeignKey("dbo.Compartment", "id", "dbo.Sbase");
            DropForeignKey("dbo.Compartment", "modelId", "dbo.Model");
            DropForeignKey("dbo.Compartment", "outside", "dbo.Compartment");
            DropIndex("dbo.pathway_to_pathway_groups", new[] { "pathway_id" });
            DropIndex("dbo.pathway_to_pathway_groups", new[] { "group_id" });
            DropIndex("dbo.UnitComposition", new[] { "unitDefinitionId" });
            DropIndex("dbo.UnitComposition", new[] { "unitId" });
            DropIndex("dbo.OLD_external_database_links", new[] { "external_database_id" });
            DropIndex("dbo.GONodeCodes", new[] { "goid" });
            DropIndex("dbo.DesignedBy", new[] { "AuthorId" });
            DropIndex("dbo.DesignedBy", new[] { "ModelMetadataId" });
            DropIndex("dbo.ModelLayout", new[] { "id" });
            DropIndex("dbo.Rule", new[] { "ruleTypeId" });
            DropIndex("dbo.Rule", new[] { "modelId" });
            DropIndex("dbo.Rule", new[] { "id" });
            DropIndex("dbo.ReactionBound", new[] { "reactionId" });
            DropIndex("dbo.ModelOrganism", new[] { "organismGroupId" });
            DropIndex("dbo.ModelOrganism", new[] { "qualifierId" });
            DropIndex("dbo.ModelOrganism", new[] { "modelId" });
            DropIndex("dbo.genes", new[] { "chromosome_id" });
            DropIndex("dbo.genes", new[] { "organism_group_id" });
            DropIndex("dbo.organism_groups", new[] { "parent_id" });
            DropIndex("dbo.rnas", new[] { "id" });
            DropIndex("dbo.proteins", new[] { "id" });
            DropIndex("dbo.process_entities", new[] { "entity_id" });
            DropIndex("dbo.process_entities", new[] { "process_id" });
            DropIndex("dbo.MapReactionsProcessEntities", new[] { "qualifierId" });
            DropIndex("dbo.MapReactionsProcessEntities", new[] { "processId" });
            DropIndex("dbo.MapReactionsProcessEntities", new[] { "reactionId" });
            DropIndex("dbo.pathway_processes", new[] { "process_id" });
            DropIndex("dbo.pathway_processes", new[] { "pathway_id" });
            DropIndex("dbo.pathway_links", new[] { "entity_id" });
            DropIndex("dbo.pathway_links", new[] { "pathway_id_2" });
            DropIndex("dbo.pathway_links", new[] { "pathway_id_1" });
            DropIndex("dbo.Unit", new[] { "kind" });
            DropIndex("dbo.Unit", new[] { "modelId" });
            DropIndex("dbo.Unit", new[] { "id" });
            DropIndex("dbo.Parameter", new[] { "unitsId" });
            DropIndex("dbo.Parameter", new[] { "modelId" });
            DropIndex("dbo.Parameter", new[] { "id" });
            DropIndex("dbo.UnitDefinition", new[] { "modelId" });
            DropIndex("dbo.UnitDefinition", new[] { "id" });
            DropIndex("dbo.SpeciesType", new[] { "modelId" });
            DropIndex("dbo.SpeciesType", new[] { "id" });
            DropIndex("dbo.StoichiometryMath", new[] { "id" });
            DropIndex("dbo.ReactionSpecies", new[] { "stoichiometryMathId" });
            DropIndex("dbo.ReactionSpecies", new[] { "roleId" });
            DropIndex("dbo.ReactionSpecies", new[] { "speciesId" });
            DropIndex("dbo.ReactionSpecies", new[] { "reactionId" });
            DropIndex("dbo.ReactionSpecies", new[] { "id" });
            DropIndex("dbo.MetaboliteReactionStoichiometry", new[] { "speciesId" });
            DropIndex("dbo.MetaboliteReactionCount", new[] { "speciesId" });
            DropIndex("dbo.CycleReaction", new[] { "cycleId" });
            DropIndex("dbo.CycleConnection", new[] { "metaboliteId" });
            DropIndex("dbo.CycleConnection", new[] { "cycleId" });
            DropIndex("dbo.Species", new[] { "substanceUnitsId" });
            DropIndex("dbo.Species", new[] { "compartmentId" });
            DropIndex("dbo.Species", new[] { "speciesTypeId" });
            DropIndex("dbo.Species", new[] { "id" });
            DropIndex("dbo.MapSpeciesMolecularEntities", new[] { "molecularEntityId" });
            DropIndex("dbo.MapSpeciesMolecularEntities", new[] { "qualifierId" });
            DropIndex("dbo.MapSpeciesMolecularEntities", new[] { "speciesId" });
            DropIndex("dbo.ec_number_name_lookups", new[] { "name_id" });
            DropIndex("dbo.ec_number_name_lookups", new[] { "ec_number" });
            DropIndex("dbo.entity_name_lookups", new[] { "name_id" });
            DropIndex("dbo.entity_name_lookups", new[] { "entity_id" });
            DropIndex("dbo.common_molecules", new[] { "id" });
            DropIndex("dbo.basic_molecules", new[] { "id" });
            DropIndex("dbo.gene_encodings", new[] { "gene_product_id" });
            DropIndex("dbo.gene_products", new[] { "id" });
            DropIndex("dbo.catalyzes", new[] { "ec_number" });
            DropIndex("dbo.catalyzes", new[] { "gene_product_id" });
            DropIndex("dbo.catalyzes", new[] { "organism_group_id" });
            DropIndex("dbo.catalyzes", new[] { "process_id" });
            DropIndex("dbo.MapReactionECNumber", new[] { "qualifierId" });
            DropIndex("dbo.MapReactionECNumber", new[] { "ecNumber" });
            DropIndex("dbo.MapReactionECNumber", new[] { "reactionId" });
            DropIndex("dbo.FixedReactions", new[] { "reactionId" });
            DropIndex("dbo.Reaction", new[] { "kineticLawId" });
            DropIndex("dbo.Reaction", new[] { "modelId" });
            DropIndex("dbo.Reaction", new[] { "id" });
            DropIndex("dbo.KineticLaw", new[] { "id" });
            DropIndex("dbo.InitialAssignment", new[] { "modelId" });
            DropIndex("dbo.InitialAssignment", new[] { "id" });
            DropIndex("dbo.FunctionDefinition", new[] { "modelId" });
            DropIndex("dbo.FunctionDefinition", new[] { "id" });
            DropIndex("dbo.EventTrigger", new[] { "id" });
            DropIndex("dbo.EventDelay", new[] { "id" });
            DropIndex("dbo.EventAssignment", new[] { "eventId" });
            DropIndex("dbo.EventAssignment", new[] { "id" });
            DropIndex("dbo.Event", new[] { "eventDelayId" });
            DropIndex("dbo.Event", new[] { "eventTriggerId" });
            DropIndex("dbo.Event", new[] { "modelId" });
            DropIndex("dbo.Event", new[] { "id" });
            DropIndex("dbo.Constraint", new[] { "modelId" });
            DropIndex("dbo.Constraint", new[] { "id" });
            DropIndex("dbo.CompartmentType", new[] { "modelId" });
            DropIndex("dbo.CompartmentType", new[] { "id" });
            DropIndex("dbo.Compartment", new[] { "outside" });
            DropIndex("dbo.Compartment", new[] { "unitsId" });
            DropIndex("dbo.Compartment", new[] { "modelId" });
            DropIndex("dbo.Compartment", new[] { "id" });
            DropIndex("dbo.Model", new[] { "dataSourceId" });
            DropIndex("dbo.Model", new[] { "id" });
            DropIndex("dbo.MapModelsPathways", new[] { "organismGroupId" });
            DropIndex("dbo.MapModelsPathways", new[] { "qualifierId" });
            DropIndex("dbo.MapModelsPathways", new[] { "pathwayId" });
            DropIndex("dbo.MapModelsPathways", new[] { "modelId" });
            DropTable("dbo.pathway_to_pathway_groups");
            DropTable("dbo.UnitComposition");
            DropTable("dbo.viewState");
            DropTable("dbo.sysdiagrams");
            DropTable("dbo.rna_types");
            DropTable("dbo.process_graph_nodes");
            DropTable("dbo.process_entity_roles");
            DropTable("dbo.pathway_types");
            DropTable("dbo.organisms");
            DropTable("dbo.OLD_external_databases");
            DropTable("dbo.OLD_external_database_links");
            DropTable("dbo.name_types");
            DropTable("dbo.molecular_entity_types");
            DropTable("dbo.MapSbaseGO");
            DropTable("dbo.go_terms_hierarchy");
            DropTable("dbo.GONodeCodes");
            DropTable("dbo.go_terms");
            DropTable("dbo.go_pathway_group_annotation_counts");
            DropTable("dbo.go_pathway_annotation_counts");
            DropTable("dbo.go_annotation_pathways");
            DropTable("dbo.external_databases");
            DropTable("dbo.external_database_urls");
            DropTable("dbo.external_database_links");
            DropTable("dbo.entity_graph_nodes");
            DropTable("dbo.ec_go_orig");
            DropTable("dbo.ec_go");
            DropTable("dbo.CompartmentClass");
            DropTable("dbo.common_species");
            DropTable("dbo.chromosomes_pathcase");
            DropTable("dbo.chromosome_bands");
            DropTable("dbo.ModelMetadata");
            DropTable("dbo.DesignedBy");
            DropTable("dbo.Author");
            DropTable("dbo.attribute_values");
            DropTable("dbo.attribute_names");
            DropTable("dbo.ModelLayout");
            DropTable("dbo.DataSource");
            DropTable("dbo.RuleType");
            DropTable("dbo.Rule");
            DropTable("dbo.ReactionBound");
            DropTable("dbo.ModelOrganism");
            DropTable("dbo.chromosomes");
            DropTable("dbo.genes");
            DropTable("dbo.organism_groups");
            DropTable("dbo.rnas");
            DropTable("dbo.proteins");
            DropTable("dbo.process_entities");
            DropTable("dbo.MapReactionsProcessEntities");
            DropTable("dbo.processes");
            DropTable("dbo.pathway_processes");
            DropTable("dbo.pathway_groups");
            DropTable("dbo.pathways");
            DropTable("dbo.pathway_links");
            DropTable("dbo.Unit");
            DropTable("dbo.Parameter");
            DropTable("dbo.UnitDefinition");
            DropTable("dbo.SpeciesType");
            DropTable("dbo.StoichiometryMath");
            DropTable("dbo.ReactionSpeciesRole");
            DropTable("dbo.ReactionSpecies");
            DropTable("dbo.MetaboliteReactionStoichiometry");
            DropTable("dbo.MetaboliteReactionCount");
            DropTable("dbo.CycleReaction");
            DropTable("dbo.Cycle");
            DropTable("dbo.CycleConnection");
            DropTable("dbo.Species");
            DropTable("dbo.MapSpeciesMolecularEntities");
            DropTable("dbo.ec_number_name_lookups");
            DropTable("dbo.molecular_entity_names");
            DropTable("dbo.entity_name_lookups");
            DropTable("dbo.common_molecules");
            DropTable("dbo.basic_molecules");
            DropTable("dbo.molecular_entities");
            DropTable("dbo.gene_encodings");
            DropTable("dbo.gene_products");
            DropTable("dbo.catalyzes");
            DropTable("dbo.ec_numbers");
            DropTable("dbo.MapReactionECNumber");
            DropTable("dbo.FixedReactions");
            DropTable("dbo.Reaction");
            DropTable("dbo.KineticLaw");
            DropTable("dbo.InitialAssignment");
            DropTable("dbo.FunctionDefinition");
            DropTable("dbo.EventTrigger");
            DropTable("dbo.EventDelay");
            DropTable("dbo.EventAssignment");
            DropTable("dbo.Event");
            DropTable("dbo.Constraint");
            DropTable("dbo.CompartmentType");
            DropTable("dbo.Sbase");
            DropTable("dbo.Compartment");
            DropTable("dbo.Model");
            DropTable("dbo.MapModelsPathways");
            DropTable("dbo.AnnotationQualifier");
        }
    }
}
