/* To avoid disclosure of passwords, the password is generated in script. */
declare @idx as int
declare @randomPwd as nvarchar(64)
declare @rnd as float
select @idx = 0
select @randomPwd = N''
select @rnd = rand((@@CPU_BUSY % 100) + ((@@IDLE % 100) * 100) + 
       (DATEPART(ss, GETDATE()) * 10000) + ((cast(DATEPART(ms, GETDATE()) as int) % 100) * 1000000))
while @idx < 64
begin
   select @randomPwd = @randomPwd + char((cast((@rnd * 83) as int) + 43))
   select @idx = @idx + 1
select @rnd = rand()
end
declare @statement nvarchar(4000)
select @statement = N'IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N''WWWUser'' AND type = ''A'')
CREATE APPLICATION ROLE [WWWUser] WITH DEFAULT_SCHEMA = [WWWUser], ' + N'PASSWORD = N' + QUOTENAME(@randomPwd,'''')
EXEC dbo.sp_executesql @statement

GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'pathcase')
EXEC sys.sp_executesql N'CREATE SCHEMA [pathcase] AUTHORIZATION [pathcase]'

GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'NETWORK SERVICE')
EXEC sys.sp_executesql N'CREATE SCHEMA [NETWORK SERVICE] AUTHORIZATION [NETWORK SERVICE]'

GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'WWWUser')
EXEC sys.sp_executesql N'CREATE SCHEMA [WWWUser] AUTHORIZATION [WWWUser]'

GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'NETWORK SERVICE')
CREATE USER [NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'pathways')
CREATE USER [pathways] FOR LOGIN [pathways] WITH DEFAULT_SCHEMA=[dbo]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'pathcase')
CREATE USER [pathcase] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[pathcase]
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[pathcase].[Get_molecular_entity_type]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'CREATE FUNCTION [pathcase].[Get_molecular_entity_type] (@entity_id uniqueidentifier)  
RETURNS varchar(15) AS  
BEGIN 
	DECLARE @type varchar(15)
	SELECT @type = met.name
	FROM molecular_entities me, molecular_enity_types met 
 	WHERE id = @entity_id 
	AND me.type_id = met.type_id
	
	RETURN @type
END
' 
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[external_database_urls]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[external_database_urls](
	[external_database_id] [int] NOT NULL,
	[type] [varchar](16) NOT NULL,
	[url_template] [varchar](256) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[external_database_links]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[external_database_links](
	[local_id] [uniqueidentifier] NOT NULL,
	[external_database_id] [int] NOT NULL,
	[id_in_external_database] [varchar](100) NOT NULL,
	[name_in_external_database] [varchar](100) NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[external_databases]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[external_databases](
	[id] [int] NOT NULL,
	[name] [varchar](100) NULL,
	[fullname] [varchar](256) NULL,
	[url] [varchar](50) NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[chromosome_bands]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[chromosome_bands](
	[chromosome_id] [uniqueidentifier] NOT NULL,
	[chromosome_name] [varchar](50) NULL,
	[arm] [varchar](10) NULL,
	[band] [varchar](20) NULL,
	[iscn_start] [int] NULL,
	[iscn_stop] [int] NULL,
	[bp_start] [int] NULL,
	[bp_stop] [int] NULL,
	[stain] [varchar](20) NULL,
	[density] [float] NULL,
	[bases] [bigint] NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[chromosomes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[chromosomes](
	[id] [uniqueidentifier] NOT NULL,
	[organism_group_id] [uniqueidentifier] NULL,
	[name] [varchar](10) NOT NULL,
	[length] [bigint] NULL,
	[centromere_location] [int] NOT NULL CONSTRAINT [DF_chromosomes_centromere_location]  DEFAULT (0),
	[notes] [text] NULL,
 CONSTRAINT [PK_chromosomes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[organism_groups]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[organism_groups](
	[id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_organism_groups_id]  DEFAULT (newid()),
	[scientific_name] [varchar](100) NULL,
	[common_name] [varchar](100) NULL,
	[parent_id] [uniqueidentifier] NULL,
	[notes] [text] NULL,
	[is_organism] [bit] NOT NULL CONSTRAINT [DF_organism_groups_is_organism]  DEFAULT (0),
	[nodeLabel] [varchar](500) NULL,
 CONSTRAINT [PK_organism_groups] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The scientific name' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'organism_groups', @level2type=N'COLUMN', @level2name=N'scientific_name'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'the immediate parent organism' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'organism_groups', @level2type=N'COLUMN', @level2name=N'parent_id'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Designates if this is an organism or group' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'organism_groups', @level2type=N'COLUMN', @level2name=N'is_organism'

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[organisms]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[organisms](
	[id] [uniqueidentifier] NOT NULL,
	[taxonomy_id] [varchar](20) NULL,
	[cM_unit_length] [int] NOT NULL CONSTRAINT [DF_organisms_cM_unit_length]  DEFAULT (1000000),
 CONSTRAINT [PK_organisms] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ec_go]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ec_go](
	[ec_number] [varchar](10) NOT NULL,
	[go_id] [varchar](7) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[go_pathway_annotation_counts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[go_pathway_annotation_counts](
	[go_id] [varchar](7) NOT NULL,
	[hierarchy_level] [int] NOT NULL,
	[number_annotations] [int] NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rule_Type]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Rule_Type](
	[id] [tinyint] NOT NULL,
	[name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Rule_Type_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[go_pathway_group_annotation_counts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[go_pathway_group_annotation_counts](
	[pathway_group_id] [uniqueidentifier] NOT NULL,
	[go_id] [varchar](7) NOT NULL,
	[hierarchy_level] [int] NOT NULL,
	[number_annotations] [int] NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[go_terms]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[go_terms](
	[ID] [varchar](7) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[SubtreeHeight] [int] NOT NULL,
	[TotalDescendants] [int] NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[go_terms_hierarchy]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[go_terms_hierarchy](
	[ParentID] [varchar](7) NOT NULL,
	[ChildID] [varchar](7) NOT NULL,
	[Type] [varchar](10) NOT NULL,
	[TermLevel] [int] NULL,
	[OnPathUnderCatalyticActivity] [bit] NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entity_types]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[molecular_entity_types](
	[type_id] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_molecular_entity_types] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[process_entity_roles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[process_entity_roles](
	[role_id] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](18) NOT NULL,
 CONSTRAINT [PK_process_entity_roles] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Model]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Model](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[SBMLData] [xml] NULL,
	[creation_date] [datetime] NULL,
	[last_modified_date] [datetime] NULL,
 CONSTRAINT [PK_Model] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ec_numbers]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ec_numbers](
	[ec_number] [varchar](10) NOT NULL,
	[name] [varchar](255) NOT NULL,
	[notes] [text] NULL,
 CONSTRAINT [PK_ec_numbers] PRIMARY KEY CLUSTERED 
(
	[ec_number] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'the default name or official name of the EC#' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ec_numbers', @level2type=N'COLUMN', @level2name=N'name'

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OLD_external_databases]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OLD_external_databases](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](100) NOT NULL,
	[notes] [text] NULL,
 CONSTRAINT [PK_external_databases] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entity_names]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[molecular_entity_names](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](255) NOT NULL,
 CONSTRAINT [PK_molecular_entity_names] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[pathway_groups]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[pathway_groups](
	[group_id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_groups_group_id]  DEFAULT (newid()),
	[name] [varchar](100) NOT NULL,
	[notes] [text] NULL,
 CONSTRAINT [PK_groups] PRIMARY KEY CLUSTERED 
(
	[group_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Delay]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Delay](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[math_expression] [xml] NOT NULL,
 CONSTRAINT [PK_Delay] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Unit]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Unit](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[is_basic] [bit] NULL,
 CONSTRAINT [PK_Unit] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[name_types]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[name_types](
	[name_type_id] [tinyint] NOT NULL,
	[name] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[processes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[processes](
	[id] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_processes_id]  DEFAULT (newid()),
	[name] [varchar](100) NOT NULL,
	[reversible] [bit] NULL CONSTRAINT [DF_processes_reversible]  DEFAULT (0),
	[location] [varchar](100) NULL,
	[notes] [text] NULL,
	[generic_process_id] [uniqueidentifier] NULL,
	[fast] [bit] NULL,
 CONSTRAINT [PK_processes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[viewState]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[viewState](
	[viewID] [uniqueidentifier] NOT NULL,
	[openSection] [varchar](32) NULL,
	[organism] [varchar](32) NULL,
	[openNode1ID] [uniqueidentifier] NULL,
	[openNode1Type] [varchar](32) NULL,
	[openNode2ID] [uniqueidentifier] NULL,
	[openNode2Type] [varchar](32) NULL,
	[openNode3ID] [uniqueidentifier] NULL,
	[openNode3Type] [varchar](32) NULL,
	[displayItemID] [uniqueidentifier] NULL,
	[displayItemType] [varchar](32) NULL,
	[viewGraph] [tinyint] NULL,
	[timeStamp] [datetime] NULL,
 CONSTRAINT [PK_viewState] PRIMARY KEY CLUSTERED 
(
	[viewID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Trigger]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Trigger](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[math_expression] [xml] NOT NULL,
	[notes] [varchar](200) NULL,
 CONSTRAINT [PK_Trigger] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Function]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Function](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[formula] [xml] NOT NULL,
	[notes] [varchar](200) NULL,
 CONSTRAINT [PK_Function] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Parameter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Parameter](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[constant] [bit] NOT NULL,
	[notes] [varchar](50) NULL,
 CONSTRAINT [PK_Parameter] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Compartment_Type]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Compartment_Type](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Compartment_Type] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Kinetic_Law]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Kinetic_Law](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NULL,
	[math_expression] [xml] NOT NULL,
	[notes] [varchar](200) NULL,
 CONSTRAINT [PK_Kinetic_law] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rna_types]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[rna_types](
	[rna_type_id] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](4) NOT NULL,
 CONSTRAINT [PK_rna_types] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[pathway_types]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[pathway_types](
	[pathway_type_id] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_pathway_types] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[attribute_names]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[attribute_names](
	[attributeId] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_attribute_names] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[go_annotation_pathways]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[go_annotation_pathways](
	[pathway_id] [uniqueidentifier] NOT NULL,
	[go_level] [int] NOT NULL,
	[serialized_image] [image] NOT NULL,
	[serialized_image_map] [ntext] NOT NULL,
	[date_generated] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Variation_Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Variation_Argument](
	[var_id] [uniqueidentifier] NOT NULL,
	[arg_id] [uniqueidentifier] NOT NULL,
	[value] [float] NOT NULL,
 CONSTRAINT [PK_Variation_Argument] PRIMARY KEY CLUSTERED 
(
	[var_id] ASC,
	[arg_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[genes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[genes](
	[id] [uniqueidentifier] NOT NULL,
	[organism_group_id] [uniqueidentifier] NOT NULL,
	[chromosome_id] [uniqueidentifier] NULL,
	[homologue_group_id] [uniqueidentifier] NOT NULL,
	[raw_address] [varchar](100) NULL,
	[cytogenic_address] [varchar](100) NULL,
	[genetic_address] [bigint] NULL,
	[relative_address] [bigint] NULL,
	[notes] [text] NULL,
 CONSTRAINT [PK_genes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_genes] UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'the homolog gene group. it''s also used to identify the homolog proteins and processes.' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'genes', @level2type=N'COLUMN', @level2name=N'homologue_group_id'

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rule]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Rule](
	[id] [uniqueidentifier] NOT NULL,
	[rule_type_id] [tinyint] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[math_expression] [xml] NOT NULL,
	[model_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Rule] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rule_Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Rule_Argument](
	[rule_id] [uniqueidentifier] NOT NULL,
	[arg_id] [uniqueidentifier] NOT NULL,
	[RightHandSide] [bit] NOT NULL,
 CONSTRAINT [PK_Rule_Argument] PRIMARY KEY CLUSTERED 
(
	[rule_id] ASC,
	[arg_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Assignment_Rule]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Assignment_Rule](
	[id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Assignment_Rule] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entities]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[molecular_entities](
	[id] [uniqueidentifier] NOT NULL,
	[type_id] [tinyint] NOT NULL,
	[name] [varchar](255) NULL,
	[notes] [text] NULL,
	[compartment_id] [uniqueidentifier] NULL,
	[substanceUnits] [varchar](20) NULL,
	[hasOnlySubstanceUnits] [bit] NULL,
 CONSTRAINT [PK_molecular_entities] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'the default name or offical name of the molecular entity' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'molecular_entities', @level2type=N'COLUMN', @level2name=N'name'

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[process_entities]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[process_entities](
	[process_id] [uniqueidentifier] NOT NULL,
	[entity_id] [uniqueidentifier] NOT NULL,
	[role_id] [tinyint] NOT NULL,
	[quantity] [varchar](10) NOT NULL CONSTRAINT [DF_process_entities_quantity]  DEFAULT (1),
	[notes] [text] NULL,
 CONSTRAINT [PK_process_entities] PRIMARY KEY CLUSTERED 
(
	[process_id] ASC,
	[entity_id] ASC,
	[role_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[entity_name_lookups](
	[entity_id] [uniqueidentifier] NOT NULL,
	[name_id] [uniqueidentifier] NOT NULL,
	[name_type_id] [tinyint] NOT NULL,
 CONSTRAINT [PK_entity_name_lookups] PRIMARY KEY CLUSTERED 
(
	[entity_id] ASC,
	[name_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[gene_products]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[gene_products](
	[id] [uniqueidentifier] NOT NULL,
	[notes] [text] NULL,
 CONSTRAINT [PK_gene_products] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_gene_products] UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[pathway_links]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[pathway_links](
	[pathway_id_1] [uniqueidentifier] NOT NULL,
	[pathway_id_2] [uniqueidentifier] NOT NULL,
	[entity_id] [uniqueidentifier] NOT NULL,
	[notes] [text] NULL,
 CONSTRAINT [PK_pathway_links] PRIMARY KEY CLUSTERED 
(
	[pathway_id_1] ASC,
	[pathway_id_2] ASC,
	[entity_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[basic_molecules]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[basic_molecules](
	[id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_basic_molecules] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Argument](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[molecule_id] [uniqueidentifier] NULL,
	[param_id] [uniqueidentifier] NULL,
	[compartment_id] [uniqueidentifier] NULL,
	[unit_id] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Argument] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Composed_Units]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Composed_Units](
	[generated_unit_id] [uniqueidentifier] NOT NULL,
	[used_unit_id] [uniqueidentifier] NOT NULL,
	[scale] [float] NULL,
	[exponent] [float] NULL,
	[multiplier] [float] NULL,
 CONSTRAINT [PK_Composed_Units] PRIMARY KEY CLUSTERED 
(
	[generated_unit_id] ASC,
	[used_unit_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Model_Parameter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Model_Parameter](
	[model_id] [uniqueidentifier] NOT NULL,
	[param_id] [uniqueidentifier] NOT NULL,
	[notes] [varchar](50) NULL,
 CONSTRAINT [PK_Model_Parameter] PRIMARY KEY CLUSTERED 
(
	[model_id] ASC,
	[param_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pathway_Model]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Pathway_Model](
	[pathway_id] [uniqueidentifier] NOT NULL,
	[model_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Pathway_Model] PRIMARY KEY CLUSTERED 
(
	[pathway_id] ASC,
	[model_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Model_Function]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Model_Function](
	[model_id] [uniqueidentifier] NOT NULL,
	[function_id] [uniqueidentifier] NOT NULL,
	[notes] [varchar](50) NULL,
 CONSTRAINT [PK_Has_Function] PRIMARY KEY CLUSTERED 
(
	[model_id] ASC,
	[function_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Model_Process]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Model_Process](
	[model_id] [uniqueidentifier] NOT NULL,
	[process_id] [uniqueidentifier] NOT NULL,
	[kinetic_id] [uniqueidentifier] NOT NULL,
	[notes] [varchar](50) NULL,
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[model_id] ASC,
	[process_id] ASC,
	[kinetic_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Variation]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Variation](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[notes] [varchar](50) NULL,
	[model_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Variation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Model_Hierarchy]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Model_Hierarchy](
	[parent_model_id] [uniqueidentifier] NOT NULL,
	[child_model_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Model_Hierarchy] PRIMARY KEY CLUSTERED 
(
	[parent_model_id] ASC,
	[child_model_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Model_Event]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Model_Event](
	[model_id] [uniqueidentifier] NOT NULL,
	[event_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Model_Event] PRIMARY KEY CLUSTERED 
(
	[model_id] ASC,
	[event_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ec_number_name_lookups](
	[ec_number] [varchar](10) NOT NULL,
	[name_id] [uniqueidentifier] NOT NULL,
	[name_type_id] [tinyint] NOT NULL,
 CONSTRAINT [PK_ec_number_name_lookups] PRIMARY KEY CLUSTERED 
(
	[ec_number] ASC,
	[name_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[catalyzes](
	[process_id] [uniqueidentifier] NOT NULL,
	[organism_group_id] [uniqueidentifier] NOT NULL,
	[gene_product_id] [uniqueidentifier] NULL,
	[ec_number] [varchar](10) NULL,
 CONSTRAINT [IX_catalyzes_all] UNIQUE NONCLUSTERED 
(
	[gene_product_id] ASC,
	[process_id] ASC,
	[ec_number] ASC,
	[organism_group_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OLD_external_database_links]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OLD_external_database_links](
	[local_id] [uniqueidentifier] NOT NULL,
	[external_database_id] [uniqueidentifier] NOT NULL,
	[id_in_external_database] [varchar](100) NOT NULL,
 CONSTRAINT [PK_external_database_links] PRIMARY KEY CLUSTERED 
(
	[local_id] ASC,
	[external_database_id] ASC,
	[id_in_external_database] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[pathway_to_pathway_groups](
	[pathway_id] [uniqueidentifier] NOT NULL,
	[group_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_pathways_groups] PRIMARY KEY CLUSTERED 
(
	[pathway_id] ASC,
	[group_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Event_Trigger_Rule_Delay]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Event_Trigger_Rule_Delay](
	[event_id] [uniqueidentifier] NOT NULL,
	[assigment_rule_id] [uniqueidentifier] NOT NULL,
	[delay_id] [uniqueidentifier] NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Delay_Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Delay_Argument](
	[delay_id] [uniqueidentifier] NOT NULL,
	[arg_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Delay_Argument] PRIMARY KEY CLUSTERED 
(
	[delay_id] ASC,
	[arg_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[pathway_processes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[pathway_processes](
	[pathway_id] [uniqueidentifier] NOT NULL,
	[process_id] [uniqueidentifier] NOT NULL,
	[notes] [text] NULL,
 CONSTRAINT [PK_pathway_processes] PRIMARY KEY CLUSTERED 
(
	[pathway_id] ASC,
	[process_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Event]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Event](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[notes] [varchar](50) NULL,
	[trigger_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Trigger_Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Trigger_Argument](
	[trigger_id] [uniqueidentifier] NOT NULL,
	[arg_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Trigger_Argument] PRIMARY KEY CLUSTERED 
(
	[trigger_id] ASC,
	[arg_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Function_Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Function_Argument](
	[func_id] [uniqueidentifier] NOT NULL,
	[arg_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Function_Argument] PRIMARY KEY CLUSTERED 
(
	[func_id] ASC,
	[arg_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Compartment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Compartment](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[comp_type_id] [uniqueidentifier] NULL,
	[spatialDimensions] [int] NOT NULL,
	[size] [float] NOT NULL,
	[constant] [bit] NULL,
	[parent_comp_id] [uniqueidentifier] NULL,
	[notes] [varchar](200) NULL,
 CONSTRAINT [PK_Compartment] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[common_molecules]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[common_molecules](
	[id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_common_molecules] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KineticLaw_Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[KineticLaw_Argument](
	[law_id] [uniqueidentifier] NOT NULL,
	[arg_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_KineticLaw_Argument] PRIMARY KEY CLUSTERED 
(
	[law_id] ASC,
	[arg_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rnas]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[rnas](
	[id] [uniqueidentifier] NOT NULL,
	[rna_type_id] [tinyint] NOT NULL,
 CONSTRAINT [PK_rnas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[gene_encodings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[gene_encodings](
	[gene_id] [uniqueidentifier] NOT NULL,
	[gene_product_id] [uniqueidentifier] NOT NULL,
	[notes] [text] NULL,
 CONSTRAINT [PK_gene_and_gene_products] PRIMARY KEY CLUSTERED 
(
	[gene_id] ASC,
	[gene_product_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[proteins]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[proteins](
	[id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_proteins] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[pathways]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[pathways](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](100) NOT NULL,
	[pathway_type_id] [tinyint] NOT NULL,
	[status] [varchar](255) NULL,
	[notes] [text] NULL,
	[layout] [text] NULL,
 CONSTRAINT [PK_pathways] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[attribute_values]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[attribute_values](
	[attributeId] [int] NOT NULL,
	[itemId] [uniqueidentifier] NOT NULL,
	[value] [varchar](800) NULL,
	[textValue] [text] NULL,
 CONSTRAINT [PK_attribute_values] PRIMARY KEY NONCLUSTERED 
(
	[attributeId] ASC,
	[itemId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[pathcase].[ComputePathwayLinks]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROC [pathcase].[ComputePathwayLinks]
AS

delete from pathway_links

insert into pathway_links (pathway_id_1, pathway_id_2, entity_id)
select distinct t1.pathway_id pathway_id_1,t2.pathway_id pathway_id_2,t1.entity_id
from 
	(
	select distinct entity_id,pathway_id
	from process_entities pe, pathway_processes pp, process_entity_roles per
	where pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in (''substrate'', ''product'')
  		and entity_id not in (select id from common_molecules)
	) t1,
	(
	select distinct entity_id,pathway_id
	from process_entities pe, pathway_processes pp, process_entity_roles per
	where pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in (''substrate'', ''product'')
  		and entity_id not in (select id from common_molecules)
	) t2
where t1.entity_id = t2.entity_id and t1.pathway_id != t2.pathway_id' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[View_Process_Entities]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[View_Process_Entities]
AS
SELECT     pe.process_id, pe.entity_id, per.name AS role
FROM         dbo.process_entities AS pe INNER JOIN
                      dbo.process_entity_roles AS per ON pe.role_id = per.role_id
UNION
SELECT     process_id, gene_product_id AS entity_id, ''catalyzing molecule'' AS Expr1
FROM         dbo.catalyzes
' 
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4[30] 2[40] 3) )"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 3
   End
   Begin DiagramPane = 
      PaneHidden = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 5
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'View_Process_Entities'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'View_Process_Entities'

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_organism_groups_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[organism_groups]'))
ALTER TABLE [dbo].[organism_groups]  WITH CHECK ADD  CONSTRAINT [FK_organism_groups_organism_groups] FOREIGN KEY([parent_id])
REFERENCES [dbo].[organism_groups] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[organism_groups]'))
ALTER TABLE [dbo].[organism_groups]  WITH CHECK ADD  CONSTRAINT [CK_organism_groups] CHECK  (([common_name] is not null or [scientific_name] is not null))
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_numbers]'))
ALTER TABLE [dbo].[ec_numbers]  WITH NOCHECK ADD  CONSTRAINT [CK_ec_numbers] CHECK  (([ec_number] like '[1-6].%.%.%'))
GO
ALTER TABLE [dbo].[ec_numbers] CHECK CONSTRAINT [CK_ec_numbers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Variation_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[dbo].[Variation_Argument]'))
ALTER TABLE [dbo].[Variation_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Variation_Argument_Argument] FOREIGN KEY([arg_id])
REFERENCES [dbo].[Argument] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Variation_Argument_Variation]') AND parent_object_id = OBJECT_ID(N'[dbo].[Variation_Argument]'))
ALTER TABLE [dbo].[Variation_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Variation_Argument_Variation] FOREIGN KEY([var_id])
REFERENCES [dbo].[Variation] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_genes_chromosomes]') AND parent_object_id = OBJECT_ID(N'[dbo].[genes]'))
ALTER TABLE [dbo].[genes]  WITH CHECK ADD  CONSTRAINT [FK_genes_chromosomes] FOREIGN KEY([chromosome_id])
REFERENCES [dbo].[chromosomes] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_genes_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[genes]'))
ALTER TABLE [dbo].[genes]  WITH CHECK ADD  CONSTRAINT [FK_genes_organism_groups] FOREIGN KEY([organism_group_id])
REFERENCES [dbo].[organism_groups] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Rule_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Rule]'))
ALTER TABLE [dbo].[Rule]  WITH CHECK ADD  CONSTRAINT [FK_Rule_Model] FOREIGN KEY([model_id])
REFERENCES [dbo].[Model] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Rule_Rule_Type]') AND parent_object_id = OBJECT_ID(N'[dbo].[Rule]'))
ALTER TABLE [dbo].[Rule]  WITH CHECK ADD  CONSTRAINT [FK_Rule_Rule_Type] FOREIGN KEY([rule_type_id])
REFERENCES [dbo].[Rule_Type] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Rule_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[dbo].[Rule_Argument]'))
ALTER TABLE [dbo].[Rule_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Rule_Argument_Argument] FOREIGN KEY([arg_id])
REFERENCES [dbo].[Argument] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Rule_Argument_Rule]') AND parent_object_id = OBJECT_ID(N'[dbo].[Rule_Argument]'))
ALTER TABLE [dbo].[Rule_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Rule_Argument_Rule] FOREIGN KEY([rule_id])
REFERENCES [dbo].[Rule] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Assignment_Rule_Rule]') AND parent_object_id = OBJECT_ID(N'[dbo].[Assignment_Rule]'))
ALTER TABLE [dbo].[Assignment_Rule]  WITH CHECK ADD  CONSTRAINT [FK_Assignment_Rule_Rule] FOREIGN KEY([id])
REFERENCES [dbo].[Rule] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_molecular_entities_Compartment]') AND parent_object_id = OBJECT_ID(N'[dbo].[molecular_entities]'))
ALTER TABLE [dbo].[molecular_entities]  WITH CHECK ADD  CONSTRAINT [FK_molecular_entities_Compartment] FOREIGN KEY([compartment_id])
REFERENCES [dbo].[Compartment] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_molecular_entities_molecular_entity_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[molecular_entities]'))
ALTER TABLE [dbo].[molecular_entities]  WITH NOCHECK ADD  CONSTRAINT [FK_molecular_entities_molecular_entity_names] FOREIGN KEY([name])
REFERENCES [dbo].[molecular_entity_names] ([name])
GO
ALTER TABLE [dbo].[molecular_entities] CHECK CONSTRAINT [FK_molecular_entities_molecular_entity_names]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_molecular_entities_molecular_entity_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[molecular_entities]'))
ALTER TABLE [dbo].[molecular_entities]  WITH NOCHECK ADD  CONSTRAINT [FK_molecular_entities_molecular_entity_types] FOREIGN KEY([type_id])
REFERENCES [dbo].[molecular_entity_types] ([type_id])
GO
ALTER TABLE [dbo].[molecular_entities] CHECK CONSTRAINT [FK_molecular_entities_molecular_entity_types]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH NOCHECK ADD  CONSTRAINT [FK_process_entities_molecular_entities] FOREIGN KEY([entity_id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
ALTER TABLE [dbo].[process_entities] CHECK CONSTRAINT [FK_process_entities_molecular_entities]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_Process]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH CHECK ADD  CONSTRAINT [FK_process_entities_Process] FOREIGN KEY([process_id])
REFERENCES [dbo].[processes] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_process_entity_roles]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH CHECK ADD  CONSTRAINT [FK_process_entities_process_entity_roles] FOREIGN KEY([role_id])
REFERENCES [dbo].[process_entity_roles] ([role_id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH NOCHECK ADD  CONSTRAINT [FK_process_entities_processes] FOREIGN KEY([process_id])
REFERENCES [dbo].[processes] ([id])
GO
ALTER TABLE [dbo].[process_entities] CHECK CONSTRAINT [FK_process_entities_processes]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_process_entities_quantity]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH NOCHECK ADD  CONSTRAINT [CK_process_entities_quantity] CHECK  (([quantity] >= 1))
GO
ALTER TABLE [dbo].[process_entities] CHECK CONSTRAINT [CK_process_entities_quantity]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_entity_name_lookups_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]'))
ALTER TABLE [dbo].[entity_name_lookups]  WITH NOCHECK ADD  CONSTRAINT [FK_entity_name_lookups_molecular_entities] FOREIGN KEY([entity_id])
REFERENCES [dbo].[molecular_entities] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[entity_name_lookups] CHECK CONSTRAINT [FK_entity_name_lookups_molecular_entities]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_entity_name_lookups_molecular_entity_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]'))
ALTER TABLE [dbo].[entity_name_lookups]  WITH NOCHECK ADD  CONSTRAINT [FK_entity_name_lookups_molecular_entity_names] FOREIGN KEY([name_id])
REFERENCES [dbo].[molecular_entity_names] ([id])
GO
ALTER TABLE [dbo].[entity_name_lookups] CHECK CONSTRAINT [FK_entity_name_lookups_molecular_entity_names]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_gene_products_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[gene_products]'))
ALTER TABLE [dbo].[gene_products]  WITH NOCHECK ADD  CONSTRAINT [FK_gene_products_molecular_entities] FOREIGN KEY([id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
ALTER TABLE [dbo].[gene_products] CHECK CONSTRAINT [FK_gene_products_molecular_entities]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_links_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_links]'))
ALTER TABLE [dbo].[pathway_links]  WITH NOCHECK ADD  CONSTRAINT [FK_pathway_links_molecular_entities] FOREIGN KEY([entity_id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
ALTER TABLE [dbo].[pathway_links] CHECK CONSTRAINT [FK_pathway_links_molecular_entities]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_links_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_links]'))
ALTER TABLE [dbo].[pathway_links]  WITH NOCHECK ADD  CONSTRAINT [FK_pathway_links_pathways] FOREIGN KEY([pathway_id_1])
REFERENCES [dbo].[pathways] ([id])
GO
ALTER TABLE [dbo].[pathway_links] CHECK CONSTRAINT [FK_pathway_links_pathways]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_links_pathways1]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_links]'))
ALTER TABLE [dbo].[pathway_links]  WITH NOCHECK ADD  CONSTRAINT [FK_pathway_links_pathways1] FOREIGN KEY([pathway_id_2])
REFERENCES [dbo].[pathways] ([id])
GO
ALTER TABLE [dbo].[pathway_links] CHECK CONSTRAINT [FK_pathway_links_pathways1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_basic_molecules_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[basic_molecules]'))
ALTER TABLE [dbo].[basic_molecules]  WITH NOCHECK ADD  CONSTRAINT [FK_basic_molecules_molecular_entities] FOREIGN KEY([id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
ALTER TABLE [dbo].[basic_molecules] CHECK CONSTRAINT [FK_basic_molecules_molecular_entities]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[dbo].[Argument]'))
ALTER TABLE [dbo].[Argument]  WITH CHECK ADD  CONSTRAINT [FK_Argument_Argument] FOREIGN KEY([molecule_id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Argument_Compartment]') AND parent_object_id = OBJECT_ID(N'[dbo].[Argument]'))
ALTER TABLE [dbo].[Argument]  WITH CHECK ADD  CONSTRAINT [FK_Argument_Compartment] FOREIGN KEY([compartment_id])
REFERENCES [dbo].[Compartment] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Argument_Parameter]') AND parent_object_id = OBJECT_ID(N'[dbo].[Argument]'))
ALTER TABLE [dbo].[Argument]  WITH CHECK ADD  CONSTRAINT [FK_Argument_Parameter] FOREIGN KEY([param_id])
REFERENCES [dbo].[Parameter] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Argument_Unit]') AND parent_object_id = OBJECT_ID(N'[dbo].[Argument]'))
ALTER TABLE [dbo].[Argument]  WITH CHECK ADD  CONSTRAINT [FK_Argument_Unit] FOREIGN KEY([unit_id])
REFERENCES [dbo].[Unit] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Composed_Units_Unit]') AND parent_object_id = OBJECT_ID(N'[dbo].[Composed_Units]'))
ALTER TABLE [dbo].[Composed_Units]  WITH CHECK ADD  CONSTRAINT [FK_Composed_Units_Unit] FOREIGN KEY([generated_unit_id])
REFERENCES [dbo].[Unit] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Composed_Units_Unit1]') AND parent_object_id = OBJECT_ID(N'[dbo].[Composed_Units]'))
ALTER TABLE [dbo].[Composed_Units]  WITH CHECK ADD  CONSTRAINT [FK_Composed_Units_Unit1] FOREIGN KEY([used_unit_id])
REFERENCES [dbo].[Unit] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_Parameter_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model_Parameter]'))
ALTER TABLE [dbo].[Model_Parameter]  WITH CHECK ADD  CONSTRAINT [FK_Model_Parameter_Model] FOREIGN KEY([model_id])
REFERENCES [dbo].[Model] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_Parameter_Parameter]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model_Parameter]'))
ALTER TABLE [dbo].[Model_Parameter]  WITH CHECK ADD  CONSTRAINT [FK_Model_Parameter_Parameter] FOREIGN KEY([param_id])
REFERENCES [dbo].[Parameter] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Pathway_Model_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Pathway_Model]'))
ALTER TABLE [dbo].[Pathway_Model]  WITH CHECK ADD  CONSTRAINT [FK_Pathway_Model_Model] FOREIGN KEY([model_id])
REFERENCES [dbo].[Model] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Pathway_Model_Pathway]') AND parent_object_id = OBJECT_ID(N'[dbo].[Pathway_Model]'))
ALTER TABLE [dbo].[Pathway_Model]  WITH CHECK ADD  CONSTRAINT [FK_Pathway_Model_Pathway] FOREIGN KEY([pathway_id])
REFERENCES [dbo].[pathways] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Has_Function_Function]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model_Function]'))
ALTER TABLE [dbo].[Model_Function]  WITH CHECK ADD  CONSTRAINT [FK_Has_Function_Function] FOREIGN KEY([function_id])
REFERENCES [dbo].[Function] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Has_Function_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model_Function]'))
ALTER TABLE [dbo].[Model_Function]  WITH CHECK ADD  CONSTRAINT [FK_Has_Function_Model] FOREIGN KEY([model_id])
REFERENCES [dbo].[Model] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_Process_Kinetic_Law]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model_Process]'))
ALTER TABLE [dbo].[Model_Process]  WITH CHECK ADD  CONSTRAINT [FK_Model_Process_Kinetic_Law] FOREIGN KEY([kinetic_id])
REFERENCES [dbo].[Kinetic_Law] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_Process_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model_Process]'))
ALTER TABLE [dbo].[Model_Process]  WITH CHECK ADD  CONSTRAINT [FK_Model_Process_Model] FOREIGN KEY([model_id])
REFERENCES [dbo].[Model] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_Process_Process]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model_Process]'))
ALTER TABLE [dbo].[Model_Process]  WITH CHECK ADD  CONSTRAINT [FK_Model_Process_Process] FOREIGN KEY([process_id])
REFERENCES [dbo].[processes] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Variation_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Variation]'))
ALTER TABLE [dbo].[Variation]  WITH CHECK ADD  CONSTRAINT [FK_Variation_Model] FOREIGN KEY([model_id])
REFERENCES [dbo].[Model] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_Hierarchy_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model_Hierarchy]'))
ALTER TABLE [dbo].[Model_Hierarchy]  WITH CHECK ADD  CONSTRAINT [FK_Model_Hierarchy_Model] FOREIGN KEY([child_model_id])
REFERENCES [dbo].[Model] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_Hierarchy_Model_Hierarchy]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model_Hierarchy]'))
ALTER TABLE [dbo].[Model_Hierarchy]  WITH CHECK ADD  CONSTRAINT [FK_Model_Hierarchy_Model_Hierarchy] FOREIGN KEY([parent_model_id])
REFERENCES [dbo].[Model] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_Event_Event]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model_Event]'))
ALTER TABLE [dbo].[Model_Event]  WITH CHECK ADD  CONSTRAINT [FK_Model_Event_Event] FOREIGN KEY([event_id])
REFERENCES [dbo].[Event] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_Event_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model_Event]'))
ALTER TABLE [dbo].[Model_Event]  WITH CHECK ADD  CONSTRAINT [FK_Model_Event_Model] FOREIGN KEY([model_id])
REFERENCES [dbo].[Model] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ec_number_name_lookups_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]'))
ALTER TABLE [dbo].[ec_number_name_lookups]  WITH NOCHECK ADD  CONSTRAINT [FK_ec_number_name_lookups_ec_numbers] FOREIGN KEY([ec_number])
REFERENCES [dbo].[ec_numbers] ([ec_number])
GO
ALTER TABLE [dbo].[ec_number_name_lookups] CHECK CONSTRAINT [FK_ec_number_name_lookups_ec_numbers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ec_number_name_lookups_molecular_entity_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]'))
ALTER TABLE [dbo].[ec_number_name_lookups]  WITH NOCHECK ADD  CONSTRAINT [FK_ec_number_name_lookups_molecular_entity_names] FOREIGN KEY([name_id])
REFERENCES [dbo].[molecular_entity_names] ([id])
GO
ALTER TABLE [dbo].[ec_number_name_lookups] CHECK CONSTRAINT [FK_ec_number_name_lookups_molecular_entity_names]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes]  WITH NOCHECK ADD  CONSTRAINT [FK_catalyzes_ec_numbers] FOREIGN KEY([ec_number])
REFERENCES [dbo].[ec_numbers] ([ec_number])
GO
ALTER TABLE [dbo].[catalyzes] CHECK CONSTRAINT [FK_catalyzes_ec_numbers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes]  WITH NOCHECK ADD  CONSTRAINT [FK_catalyzes_gene_products] FOREIGN KEY([gene_product_id])
REFERENCES [dbo].[gene_products] ([id])
GO
ALTER TABLE [dbo].[catalyzes] CHECK CONSTRAINT [FK_catalyzes_gene_products]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes]  WITH NOCHECK ADD  CONSTRAINT [FK_catalyzes_processes] FOREIGN KEY([process_id])
REFERENCES [dbo].[processes] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[catalyzes] CHECK CONSTRAINT [FK_catalyzes_processes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_external_database_links_external_databases]') AND parent_object_id = OBJECT_ID(N'[dbo].[OLD_external_database_links]'))
ALTER TABLE [dbo].[OLD_external_database_links]  WITH NOCHECK ADD  CONSTRAINT [FK_external_database_links_external_databases] FOREIGN KEY([external_database_id])
REFERENCES [dbo].[OLD_external_databases] ([id])
GO
ALTER TABLE [dbo].[OLD_external_database_links] CHECK CONSTRAINT [FK_external_database_links_external_databases]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_to_pathway_groups_pathway_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]'))
ALTER TABLE [dbo].[pathway_to_pathway_groups]  WITH CHECK ADD  CONSTRAINT [FK_pathway_to_pathway_groups_pathway_groups] FOREIGN KEY([group_id])
REFERENCES [dbo].[pathway_groups] ([group_id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_to_pathway_groups_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]'))
ALTER TABLE [dbo].[pathway_to_pathway_groups]  WITH NOCHECK ADD  CONSTRAINT [FK_pathway_to_pathway_groups_pathways] FOREIGN KEY([pathway_id])
REFERENCES [dbo].[pathways] ([id])
GO
ALTER TABLE [dbo].[pathway_to_pathway_groups] CHECK CONSTRAINT [FK_pathway_to_pathway_groups_pathways]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_Trigger_Rule_Delay_Assignment_Rule]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event_Trigger_Rule_Delay]'))
ALTER TABLE [dbo].[Event_Trigger_Rule_Delay]  WITH CHECK ADD  CONSTRAINT [FK_Event_Trigger_Rule_Delay_Assignment_Rule] FOREIGN KEY([assigment_rule_id])
REFERENCES [dbo].[Assignment_Rule] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_Trigger_Rule_Delay_Delay]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event_Trigger_Rule_Delay]'))
ALTER TABLE [dbo].[Event_Trigger_Rule_Delay]  WITH CHECK ADD  CONSTRAINT [FK_Event_Trigger_Rule_Delay_Delay] FOREIGN KEY([delay_id])
REFERENCES [dbo].[Delay] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_Trigger_Rule_Delay_Event]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event_Trigger_Rule_Delay]'))
ALTER TABLE [dbo].[Event_Trigger_Rule_Delay]  WITH CHECK ADD  CONSTRAINT [FK_Event_Trigger_Rule_Delay_Event] FOREIGN KEY([event_id])
REFERENCES [dbo].[Event] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Delay_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[dbo].[Delay_Argument]'))
ALTER TABLE [dbo].[Delay_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Delay_Argument_Argument] FOREIGN KEY([arg_id])
REFERENCES [dbo].[Argument] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Delay_Argument_Delay]') AND parent_object_id = OBJECT_ID(N'[dbo].[Delay_Argument]'))
ALTER TABLE [dbo].[Delay_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Delay_Argument_Delay] FOREIGN KEY([delay_id])
REFERENCES [dbo].[Delay] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_processes_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_processes]'))
ALTER TABLE [dbo].[pathway_processes]  WITH NOCHECK ADD  CONSTRAINT [FK_pathway_processes_pathways] FOREIGN KEY([pathway_id])
REFERENCES [dbo].[pathways] ([id])
GO
ALTER TABLE [dbo].[pathway_processes] CHECK CONSTRAINT [FK_pathway_processes_pathways]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_processes_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_processes]'))
ALTER TABLE [dbo].[pathway_processes]  WITH CHECK ADD  CONSTRAINT [FK_pathway_processes_processes] FOREIGN KEY([process_id])
REFERENCES [dbo].[processes] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_Trigger]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event]'))
ALTER TABLE [dbo].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_Trigger] FOREIGN KEY([trigger_id])
REFERENCES [dbo].[Trigger] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Trigger_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[dbo].[Trigger_Argument]'))
ALTER TABLE [dbo].[Trigger_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Trigger_Argument_Argument] FOREIGN KEY([arg_id])
REFERENCES [dbo].[Argument] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Trigger_Argument_Trigger]') AND parent_object_id = OBJECT_ID(N'[dbo].[Trigger_Argument]'))
ALTER TABLE [dbo].[Trigger_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Trigger_Argument_Trigger] FOREIGN KEY([trigger_id])
REFERENCES [dbo].[Trigger] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Function_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[dbo].[Function_Argument]'))
ALTER TABLE [dbo].[Function_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Function_Argument_Argument] FOREIGN KEY([arg_id])
REFERENCES [dbo].[Argument] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Function_Argument_Function]') AND parent_object_id = OBJECT_ID(N'[dbo].[Function_Argument]'))
ALTER TABLE [dbo].[Function_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Function_Argument_Function] FOREIGN KEY([func_id])
REFERENCES [dbo].[Function] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compartment_Compartment]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compartment]'))
ALTER TABLE [dbo].[Compartment]  WITH CHECK ADD  CONSTRAINT [FK_Compartment_Compartment] FOREIGN KEY([parent_comp_id])
REFERENCES [dbo].[Compartment] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compartment_Compartment_Type]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compartment]'))
ALTER TABLE [dbo].[Compartment]  WITH CHECK ADD  CONSTRAINT [FK_Compartment_Compartment_Type] FOREIGN KEY([comp_type_id])
REFERENCES [dbo].[Compartment_Type] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_common_molecules_basic_molecules]') AND parent_object_id = OBJECT_ID(N'[dbo].[common_molecules]'))
ALTER TABLE [dbo].[common_molecules]  WITH NOCHECK ADD  CONSTRAINT [FK_common_molecules_basic_molecules] FOREIGN KEY([id])
REFERENCES [dbo].[basic_molecules] ([id])
GO
ALTER TABLE [dbo].[common_molecules] CHECK CONSTRAINT [FK_common_molecules_basic_molecules]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KineticLaw_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[dbo].[KineticLaw_Argument]'))
ALTER TABLE [dbo].[KineticLaw_Argument]  WITH CHECK ADD  CONSTRAINT [FK_KineticLaw_Argument_Argument] FOREIGN KEY([arg_id])
REFERENCES [dbo].[Argument] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KineticLaw_Argument_Kinetic_law]') AND parent_object_id = OBJECT_ID(N'[dbo].[KineticLaw_Argument]'))
ALTER TABLE [dbo].[KineticLaw_Argument]  WITH CHECK ADD  CONSTRAINT [FK_KineticLaw_Argument_Kinetic_law] FOREIGN KEY([law_id])
REFERENCES [dbo].[Kinetic_Law] ([id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_rnas_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[rnas]'))
ALTER TABLE [dbo].[rnas]  WITH NOCHECK ADD  CONSTRAINT [FK_rnas_gene_products] FOREIGN KEY([id])
REFERENCES [dbo].[gene_products] ([id])
GO
ALTER TABLE [dbo].[rnas] CHECK CONSTRAINT [FK_rnas_gene_products]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_rnas_rna_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[rnas]'))
ALTER TABLE [dbo].[rnas]  WITH NOCHECK ADD  CONSTRAINT [FK_rnas_rna_types] FOREIGN KEY([rna_type_id])
REFERENCES [dbo].[rna_types] ([rna_type_id])
GO
ALTER TABLE [dbo].[rnas] CHECK CONSTRAINT [FK_rnas_rna_types]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_gene_and_gene_products_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[gene_encodings]'))
ALTER TABLE [dbo].[gene_encodings]  WITH NOCHECK ADD  CONSTRAINT [FK_gene_and_gene_products_gene_products] FOREIGN KEY([gene_product_id])
REFERENCES [dbo].[gene_products] ([id])
GO
ALTER TABLE [dbo].[gene_encodings] CHECK CONSTRAINT [FK_gene_and_gene_products_gene_products]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_proteins_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[proteins]'))
ALTER TABLE [dbo].[proteins]  WITH NOCHECK ADD  CONSTRAINT [FK_proteins_gene_products] FOREIGN KEY([id])
REFERENCES [dbo].[gene_products] ([id])
GO
ALTER TABLE [dbo].[proteins] CHECK CONSTRAINT [FK_proteins_gene_products]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathways_pathway_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathways]'))
ALTER TABLE [dbo].[pathways]  WITH CHECK ADD  CONSTRAINT [FK_pathways_pathway_types] FOREIGN KEY([pathway_type_id])
REFERENCES [dbo].[pathway_types] ([pathway_type_id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_attribute_values_attribute_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[attribute_values]'))
ALTER TABLE [dbo].[attribute_values]  WITH CHECK ADD  CONSTRAINT [FK_attribute_values_attribute_names] FOREIGN KEY([attributeId])
REFERENCES [dbo].[attribute_names] ([attributeId])
ON UPDATE CASCADE
ON DELETE CASCADE
