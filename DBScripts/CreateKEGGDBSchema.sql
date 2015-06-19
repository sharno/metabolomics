USE [master]
GO
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'Pathcase_KEGG_NEW')
BEGIN
CREATE DATABASE [Pathcase_KEGG_NEW] ON  PRIMARY 
( NAME = N'KEGG_Data', FILENAME = N'C:\PathCase Databases\KEGG_NEW\KEGG_Data.MDF' , SIZE = 3390848KB , MAXSIZE = UNLIMITED, FILEGROWTH = 10%)
 LOG ON 
( NAME = N'KEGG_Log', FILENAME = N'C:\PathCase Databases\KEGG_NEW\KEGG_Log.LDF' , SIZE = 2614848KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
END

GO
EXEC dbo.sp_dbcmptlevel @dbname=N'Pathcase_KEGG_NEW', @new_cmptlevel=80
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Pathcase_KEGG_NEW].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET ARITHABORT OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET  READ_WRITE 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET RECOVERY FULL 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET  MULTI_USER 
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET PAGE_VERIFY TORN_PAGE_DETECTION  
GO
ALTER DATABASE [Pathcase_KEGG_NEW] SET DB_CHAINING OFF 
USE [Pathcase_KEGG_NEW]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'NETWORK SERVICE')
CREATE USER [NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE] WITH DEFAULT_SCHEMA=[dbo]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'gyavas')
CREATE USER [gyavas] FOR LOGIN [gyavas] WITH DEFAULT_SCHEMA=[dbo]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'cakmak')
CREATE USER [cakmak] FOR LOGIN [cakmak] WITH DEFAULT_SCHEMA=[dbo]
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'pathcase')
EXEC sys.sp_executesql N'CREATE SCHEMA [pathcase] AUTHORIZATION [dbo]'

GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'NETWORK SERVICE')
EXEC sys.sp_executesql N'CREATE SCHEMA [NETWORK SERVICE] AUTHORIZATION [NETWORK SERVICE]'

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Beginning of the modification by En,    Script Date: 04/30/2008  ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[entity_graph_nodes]') AND type in (N'U'))
BEGIN
/****** Object:  Table [dbo].[entity_graph_nodes]    Script Date: 04/30/2008 09:38:16 ******/
CREATE TABLE [dbo].[entity_graph_nodes](
	[pathwayId] [uniqueidentifier] NULL,
	[entityId] [uniqueidentifier] NOT NULL,
	[graphNodeId] [uniqueidentifier] NULL
) ON [PRIMARY]

END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[entity_graph_nodes]') AND name = N'IX_entity_graph_nodes')

/****** Object:  Index [IX_entity_graph_nodes]    Script Date: 04/30/2008 09:46:12 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_entity_graph_nodes] ON [dbo].[entity_graph_nodes] 
(
	[pathwayId] ASC,
	[entityId] ASC
)WITH (PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[entity_graph_nodes]') AND name = N'IX_pathway_entities')

/****** Object:  Index [IX_pathway_entities]    Script Date: 04/30/2008 09:46:30 ******/
CREATE NONCLUSTERED INDEX [IX_pathway_entities] ON [dbo].[entity_graph_nodes] 
(
	[graphNodeId] ASC
)WITH (PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[process_graph_nodes]') AND type in (N'U'))
BEGIN

/****** Object:  Table [dbo].[process_graph_nodes]    Script Date: 04/30/2008 10:13:39 ******/

CREATE TABLE [dbo].[process_graph_nodes](
	[pathwayId] [uniqueidentifier] NULL,
	[genericProcessId] [uniqueidentifier] NOT NULL,
	[graphNodeId] [uniqueidentifier] NULL
) ON [PRIMARY]

END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_graph_nodes]') AND name = N'IX_process_graph_nodes')
/****** Object:  Index [IX_process_graph_nodes]    Script Date: 04/30/2008 10:14:05 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_process_graph_nodes] ON [dbo].[process_graph_nodes] 
(
	[pathwayId] ASC,
	[genericProcessId] ASC
)WITH (PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_graph_nodes]') AND name = N'IX_process_graph_nodes_1')
/****** Object:  Index [IX_process_graph_nodes_1]    Script Date: 04/30/2008 10:14:11 ******/
CREATE NONCLUSTERED INDEX [IX_process_graph_nodes_1] ON [dbo].[process_graph_nodes] 
(
	[graphNodeId] ASC
)WITH (PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** End of the modification by En,    Script Date: 04/30/2008 ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[name_types]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[name_types](
	[name_type_id] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_name_types] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[name_types]') AND name = N'IX_name_types')
CREATE UNIQUE NONCLUSTERED INDEX [IX_name_types] ON [dbo].[name_types] 
(
	[name_type_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
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
	[is_organism] [bit] NOT NULL CONSTRAINT [DF_organism_groups_is_organism]  DEFAULT ((0)),
	[nodeLabel] [varchar](500) NULL,
 CONSTRAINT [PK_organism_groups] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[organism_groups]') AND name = N'IX_organism_groups')
CREATE NONCLUSTERED INDEX [IX_organism_groups] ON [dbo].[organism_groups] 
(
	[common_name] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[organism_groups]') AND name = N'IX_organism_groups_1')
CREATE NONCLUSTERED INDEX [IX_organism_groups_1] ON [dbo].[organism_groups] 
(
	[parent_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[organism_groups]') AND name = N'IX_organism_groups_2')
CREATE NONCLUSTERED INDEX [IX_organism_groups_2] ON [dbo].[organism_groups] 
(
	[nodeLabel] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'organism_groups', N'COLUMN',N'scientific_name'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The scientific name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'organism_groups', @level2type=N'COLUMN',@level2name=N'scientific_name'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'organism_groups', N'COLUMN',N'parent_id'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'the immediate parent organism' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'organism_groups', @level2type=N'COLUMN',@level2name=N'parent_id'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'organism_groups', N'COLUMN',N'is_organism'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Designates if this is an organism or group' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'organism_groups', @level2type=N'COLUMN',@level2name=N'is_organism'
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
	[cM_unit_length] [int] NOT NULL CONSTRAINT [DF_organisms_cM_unit_length]  DEFAULT ((1000000)),
 CONSTRAINT [PK_organisms] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_types]') AND name = N'IX_pathway_types')
CREATE UNIQUE NONCLUSTERED INDEX [IX_pathway_types] ON [dbo].[pathway_types] 
(
	[pathway_type_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_entity_roles]') AND name = N'IX_process_entity_roles')
CREATE UNIQUE NONCLUSTERED INDEX [IX_process_entity_roles] ON [dbo].[process_entity_roles] 
(
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[rna_types]') AND name = N'IX_rna_types')
CREATE UNIQUE NONCLUSTERED INDEX [IX_rna_types] ON [dbo].[rna_types] 
(
	[rna_type_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[viewState]') AND name = N'IX_All')
CREATE NONCLUSTERED INDEX [IX_All] ON [dbo].[viewState] 
(
	[openSection] ASC,
	[openNode1ID] ASC,
	[openNode1Type] ASC,
	[openNode2ID] ASC,
	[openNode2Type] ASC,
	[openNode3ID] ASC,
	[openNode3Type] ASC,
	[displayItemID] ASC,
	[displayItemType] ASC,
	[viewGraph] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[processes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[processes](
	[id] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_processes_id]  DEFAULT (newid()),
	[name] [varchar](800) NOT NULL,
	[reversible] [bit] NULL CONSTRAINT [DF_processes_reversible]  DEFAULT ((0)),
	[location] [varchar](100) NULL,
	[notes] [text] NULL,
	[generic_process_id] [uniqueidentifier] NULL,
 CONSTRAINT [PK_processes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[processes]') AND name = N'IX_processes')
CREATE NONCLUSTERED INDEX [IX_processes] ON [dbo].[processes] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[processes]') AND name = N'IX_processes_2')
CREATE NONCLUSTERED INDEX [IX_processes_2] ON [dbo].[processes] 
(
	[generic_process_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
	[name] [varchar](100) NOT NULL,
	[length] [bigint] NULL,
	[centromere_location] [int] NOT NULL CONSTRAINT [DF_chromosomes_centromere_location]  DEFAULT ((0)),
	[notes] [text] NULL,
 CONSTRAINT [PK_chromosomes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[chromosomes]') AND name = N'IX_chromosomes')
CREATE NONCLUSTERED INDEX [IX_chromosomes] ON [dbo].[chromosomes] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[chromosomes]') AND name = N'IX_chromosomes_1')
CREATE NONCLUSTERED INDEX [IX_chromosomes_1] ON [dbo].[chromosomes] 
(
	[organism_group_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ec_numbers]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ec_numbers](
	[ec_number] [varchar](20) NOT NULL,
	[name] [varchar](255) NOT NULL,
	[notes] [text] NULL,
 CONSTRAINT [PK_ec_numbers] PRIMARY KEY CLUSTERED 
(
	[ec_number] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_numbers]') AND name = N'IX_ec_numbers')
CREATE NONCLUSTERED INDEX [IX_ec_numbers] ON [dbo].[ec_numbers] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'ec_numbers', N'COLUMN',N'name'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'the default name or official name of the EC#' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ec_numbers', @level2type=N'COLUMN',@level2name=N'name'
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ec_go]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ec_go](
	[ec_number] [varchar](50) NOT NULL,
	[go_id] [varchar](10) NULL
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_go]') AND name = N'IX_Ec2gomap1')
CREATE UNIQUE CLUSTERED INDEX [IX_Ec2gomap1] ON [dbo].[ec_go] 
(
	[ec_number] ASC,
	[go_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_go]') AND name = N'IX_Ec2gomap2')
CREATE NONCLUSTERED INDEX [IX_Ec2gomap2] ON [dbo].[ec_go] 
(
	[ec_number] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_go]') AND name = N'IX_Ec2gomap3')
CREATE NONCLUSTERED INDEX [IX_Ec2gomap3] ON [dbo].[ec_go] 
(
	[go_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[attribute_names]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[attribute_names](
	[attributeId] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_attribute_names] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[attribute_names]') AND name = N'IX_attribute_names')
CREATE UNIQUE NONCLUSTERED INDEX [IX_attribute_names] ON [dbo].[attribute_names] 
(
	[attributeId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[chromosome_bands]') AND name = N'IX_chromosome_bands')
CREATE NONCLUSTERED INDEX [IX_chromosome_bands] ON [dbo].[chromosome_bands] 
(
	[chromosome_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[chromosome_bands]') AND name = N'IX_chromosome_bands_1')
CREATE NONCLUSTERED INDEX [IX_chromosome_bands_1] ON [dbo].[chromosome_bands] 
(
	[chromosome_name] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ec_go_orig]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ec_go_orig](
	[ec_number] [varchar](10) NOT NULL,
	[go_id] [varchar](7) NOT NULL
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
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[chromosomes_pathcase]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[chromosomes_pathcase](
	[id] [uniqueidentifier] NOT NULL,
	[organism_group_id] [uniqueidentifier] NULL,
	[name] [varchar](10) NOT NULL,
	[length] [bigint] NULL,
	[centromere_location] [int] NOT NULL,
	[notes] [text] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entity_names]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[molecular_entity_names](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](255) NOT NULL,
 CONSTRAINT [PK_molecular_entity_names] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entity_names]') AND name = N'IX_molecular_entity_names')
CREATE UNIQUE NONCLUSTERED INDEX [IX_molecular_entity_names] ON [dbo].[molecular_entity_names] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entity_types]') AND name = N'IX_molecular_entity_types')
CREATE UNIQUE NONCLUSTERED INDEX [IX_molecular_entity_types] ON [dbo].[molecular_entity_types] 
(
	[type_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ec_number_name_lookups](
	[ec_number] [varchar](20) NOT NULL,
	[name_id] [uniqueidentifier] NOT NULL,
	[name_type_id] [tinyint] NOT NULL,
 CONSTRAINT [PK_ec_number_name_lookups] PRIMARY KEY CLUSTERED 
(
	[ec_number] ASC,
	[name_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]') AND name = N'IX_ec_number_name_lookups')
CREATE NONCLUSTERED INDEX [IX_ec_number_name_lookups] ON [dbo].[ec_number_name_lookups] 
(
	[ec_number] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]') AND name = N'IX_ec_number_name_lookups_1')
CREATE NONCLUSTERED INDEX [IX_ec_number_name_lookups_1] ON [dbo].[ec_number_name_lookups] 
(
	[name_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]') AND name = N'IX_ec_number_name_lookups_2')
CREATE NONCLUSTERED INDEX [IX_ec_number_name_lookups_2] ON [dbo].[ec_number_name_lookups] 
(
	[ec_number] ASC,
	[name_type_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]') AND name = N'IX_entity_name_lookups')
CREATE NONCLUSTERED INDEX [IX_entity_name_lookups] ON [dbo].[entity_name_lookups] 
(
	[entity_id] ASC,
	[name_type_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]') AND name = N'IX_entity_name_lookups_1')
CREATE NONCLUSTERED INDEX [IX_entity_name_lookups_1] ON [dbo].[entity_name_lookups] 
(
	[name_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]') AND name = N'IX_entity_name_lookups_2')
CREATE NONCLUSTERED INDEX [IX_entity_name_lookups_2] ON [dbo].[entity_name_lookups] 
(
	[entity_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
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
	[organism_group_id] [uniqueidentifier] NULL,
	[gene_product_id] [uniqueidentifier] NULL,
	[ec_number] [varchar](20) NULL,
 CONSTRAINT [IX_catalyzes_all] UNIQUE NONCLUSTERED 
(
	[gene_product_id] ASC,
	[process_id] ASC,
	[ec_number] ASC,
	[organism_group_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_ec')
CREATE NONCLUSTERED INDEX [IX_catalyzes_ec] ON [dbo].[catalyzes] 
(
	[ec_number] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_except_ec')
CREATE NONCLUSTERED INDEX [IX_catalyzes_except_ec] ON [dbo].[catalyzes] 
(
	[process_id] ASC,
	[organism_group_id] ASC,
	[gene_product_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_gene')
CREATE NONCLUSTERED INDEX [IX_catalyzes_gene] ON [dbo].[catalyzes] 
(
	[gene_product_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_gene_organism_group')
CREATE NONCLUSTERED INDEX [IX_catalyzes_gene_organism_group] ON [dbo].[catalyzes] 
(
	[organism_group_id] ASC,
	[gene_product_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_gene_process')
CREATE NONCLUSTERED INDEX [IX_catalyzes_gene_process] ON [dbo].[catalyzes] 
(
	[process_id] ASC,
	[gene_product_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_organism_group')
CREATE NONCLUSTERED INDEX [IX_catalyzes_organism_group] ON [dbo].[catalyzes] 
(
	[organism_group_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_process')
CREATE NONCLUSTERED INDEX [IX_catalyzes_process] ON [dbo].[catalyzes] 
(
	[process_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_process_organism_group')
CREATE NONCLUSTERED INDEX [IX_catalyzes_process_organism_group] ON [dbo].[catalyzes] 
(
	[process_id] ASC,
	[organism_group_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
	[raw_address] [varchar](8000) NULL,
	[cytogenic_address] [varchar](100) NULL,
	[genetic_address] [bigint] NULL,
	[relative_address] [bigint] NULL,
	[notes] [text] NULL,
 CONSTRAINT [PK_genes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY],
 CONSTRAINT [IX_genes] UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[genes]') AND name = N'IX_genes_1')
CREATE NONCLUSTERED INDEX [IX_genes_1] ON [dbo].[genes] 
(
	[organism_group_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[genes]') AND name = N'IX_genes_2')
CREATE NONCLUSTERED INDEX [IX_genes_2] ON [dbo].[genes] 
(
	[homologue_group_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[genes]') AND name = N'IX_genes_3')
CREATE NONCLUSTERED INDEX [IX_genes_3] ON [dbo].[genes] 
(
	[organism_group_id] ASC,
	[chromosome_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[genes]') AND name = N'IX_genes_4')
CREATE NONCLUSTERED INDEX [IX_genes_4] ON [dbo].[genes] 
(
	[chromosome_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'genes', N'COLUMN',N'homologue_group_id'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'the homolog gene group. it''s also used to identify the homolog proteins and processes.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'genes', @level2type=N'COLUMN',@level2name=N'homologue_group_id'
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]') AND name = N'IX_pathway_id')
CREATE NONCLUSTERED INDEX [IX_pathway_id] ON [dbo].[pathway_to_pathway_groups] 
(
	[pathway_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]') AND name = N'IX_pathway_to_pathway_groups')
CREATE NONCLUSTERED INDEX [IX_pathway_to_pathway_groups] ON [dbo].[pathway_to_pathway_groups] 
(
	[group_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathways]') AND name = N'IX_pathways')
CREATE NONCLUSTERED INDEX [IX_pathways] ON [dbo].[pathways] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathways]') AND name = N'IX_pathways_1')
CREATE NONCLUSTERED INDEX [IX_pathways_1] ON [dbo].[pathways] 
(
	[pathway_type_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
	[quantity] [varchar](10) NOT NULL CONSTRAINT [DF_process_entities_quantity]  DEFAULT ((1)),
	[notes] [text] NULL,
 CONSTRAINT [PK_process_entities] PRIMARY KEY CLUSTERED 
(
	[process_id] ASC,
	[entity_id] ASC,
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_entities]') AND name = N'IX_process_entities')
CREATE NONCLUSTERED INDEX [IX_process_entities] ON [dbo].[process_entities] 
(
	[entity_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_entities]') AND name = N'IX_process_entities_1')
CREATE NONCLUSTERED INDEX [IX_process_entities_1] ON [dbo].[process_entities] 
(
	[entity_id] ASC,
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_entities]') AND name = N'IX_process_entities_2')
CREATE NONCLUSTERED INDEX [IX_process_entities_2] ON [dbo].[process_entities] 
(
	[process_id] ASC,
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_entities]') AND name = N'IX_process_entities_3')
CREATE NONCLUSTERED INDEX [IX_process_entities_3] ON [dbo].[process_entities] 
(
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_entities]') AND name = N'IX_process_id')
CREATE NONCLUSTERED INDEX [IX_process_id] ON [dbo].[process_entities] 
(
	[process_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_processes]') AND name = N'IX_pathway_id')
CREATE NONCLUSTERED INDEX [IX_pathway_id] ON [dbo].[pathway_processes] 
(
	[pathway_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_processes]') AND name = N'IX_pathway_processes')
CREATE NONCLUSTERED INDEX [IX_pathway_processes] ON [dbo].[pathway_processes] 
(
	[process_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[attribute_values]') AND name = N'IX_attribute_values')
CREATE CLUSTERED INDEX [IX_attribute_values] ON [dbo].[attribute_values] 
(
	[attributeId] ASC,
	[itemId] ASC,
	[value] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[attribute_values]') AND name = N'attribute_value_lookup')
CREATE NONCLUSTERED INDEX [attribute_value_lookup] ON [dbo].[attribute_values] 
(
	[attributeId] ASC,
	[value] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[attribute_values]') AND name = N'IX_attribute_values_1')
CREATE UNIQUE NONCLUSTERED INDEX [IX_attribute_values_1] ON [dbo].[attribute_values] 
(
	[attributeId] ASC,
	[itemId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY],
 CONSTRAINT [IX_gene_products] UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_links]') AND name = N'IX_pathway_links')
CREATE NONCLUSTERED INDEX [IX_pathway_links] ON [dbo].[pathway_links] 
(
	[pathway_id_1] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_links]') AND name = N'IX_pathway_links_1')
CREATE NONCLUSTERED INDEX [IX_pathway_links_1] ON [dbo].[pathway_links] 
(
	[pathway_id_2] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_links]') AND name = N'IX_pathway_links_2')
CREATE NONCLUSTERED INDEX [IX_pathway_links_2] ON [dbo].[pathway_links] 
(
	[entity_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[gene_encodings]') AND name = N'IX_gene_encodings')
CREATE NONCLUSTERED INDEX [IX_gene_encodings] ON [dbo].[gene_encodings] 
(
	[gene_product_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[gene_encodings]') AND name = N'IX_gene_encodings_1')
CREATE NONCLUSTERED INDEX [IX_gene_encodings_1] ON [dbo].[gene_encodings] 
(
	[gene_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
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
 CONSTRAINT [PK_molecular_entities] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entities]') AND name = N'IX_molecular_entities')
CREATE NONCLUSTERED INDEX [IX_molecular_entities] ON [dbo].[molecular_entities] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entities]') AND name = N'IX_molecular_entities_1')
CREATE NONCLUSTERED INDEX [IX_molecular_entities_1] ON [dbo].[molecular_entities] 
(
	[type_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entities]') AND name = N'IX_molecular_entities_2')
CREATE NONCLUSTERED INDEX [IX_molecular_entities_2] ON [dbo].[molecular_entities] 
(
	[name] ASC,
	[type_id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'molecular_entities', N'COLUMN',N'name'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'the default name or offical name of the molecular entity' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'molecular_entities', @level2type=N'COLUMN',@level2name=N'name'
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
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPane1' , N'SCHEMA',N'dbo', N'VIEW',N'View_Process_Entities', NULL,NULL))
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_Process_Entities'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPaneCount' , N'SCHEMA',N'dbo', N'VIEW',N'View_Process_Entities', NULL,NULL))
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_Process_Entities'
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
where t1.entity_id = t2.entity_id and t1.pathway_id != t2.pathway_id
' 
END

CREATE PROCEDURE [dbo].[PopulatePathwayEntities]
AS
BEGIN

delete from entity_graph_nodes

insert into entity_graph_nodes
select distinct pp.pathway_id, pe.entity_id, null
from pathway_processes pp, process_entities pe, process_entity_roles per
where pp.process_id = pe.process_id
and pe.role_id = per.role_id
and (per.name = 'substrate' OR per.name='product')

declare @sourcePathway uniqueidentifier
declare @destinationPathway uniqueidentifier
declare @linkingEntity uniqueidentifier
declare @nodeId uniqueidentifier

declare crs cursor for
select pathway_id_1, pathway_id_2, entity_id 
from pathway_links

open crs

fetch crs into @sourcePathway, @destinationPathway, @linkingEntity

WHILE @@FETCH_STATUS = 0
begin

select @nodeId=graphNodeId
from entity_graph_nodes
where pathwayId=@sourcePathway
and entityId=@linkingEntity

if(@nodeId is null)
begin
	select @nodeId=newId()

	update entity_graph_nodes
	set graphNodeId=@nodeId
	where pathwayId=@sourcePathway
	and entityId=@linkingEntity
end

update entity_graph_nodes
set graphNodeId=@nodeId
where pathwayId=@destinationPathway	
and entityId=@linkingEntity

fetch crs into @sourcePathway, @destinationPathway, @linkingEntity

end

-- handles processes that are not members of any pathway --- 
---- each such process is considered to be a single process pathway
---- disconnected from the metabolic network
insert into entity_graph_nodes
select distinct pe.process_id, pe.entity_id, null
from process_entities pe, process_entity_roles per
where pe.role_id = per.role_id
and (per.name = 'substrate' OR per.name='product')
and pe.process_id not in
(
select process_id from pathway_processes
)

update entity_graph_nodes
set graphNodeId=newId()
where graphNodeId is null
	
END

CREATE PROCEDURE [dbo].[PopulateProcessGraphNodes]
AS
BEGIN

delete from process_graph_nodes

insert into process_graph_nodes
select distinct pp.pathway_id, pr.generic_process_id, null
from pathway_processes pp, processes pr
where pp.process_id = pr.id


declare @sourcePathway uniqueidentifier
declare @destinationPathway uniqueidentifier
declare @processId uniqueidentifier
declare @nodeId uniqueidentifier

declare @prCrs cursor

declare crs cursor for
select pathway_id_1, pathway_id_2 --, entity_id 
from pathway_links

open crs

fetch crs into @sourcePathway, @destinationPathway--, @linkingEntity

WHILE @@FETCH_STATUS = 0
begin
	set @prCrs=cursor for
	select distinct pr.generic_process_id
	from processes pr, pathway_processes pp1, pathway_processes pp2
	where pp1.pathway_id = @sourcePathway
	and pp2.pathway_id = @destinationPathway
	and pp1.process_id = pp2.process_id
	and pp1.process_id = pr.id
	and not exists
	(
		select pe.entity_id
		from process_entities pe
		where pe.process_id = pr.id
		and pe.entity_id not in 
		(
			select entity_id
			from pathway_links
			where pathway_id_1 = @sourcePathway
			and pathway_id_2 = @destinationPathway
		)
	)

	open @prCrs
	fetch @prCrs into @processId
	
	WHILE @@FETCH_STATUS = 0
	begin
		select @nodeId=newId()

		update process_graph_nodes
		set graphNodeId=@nodeId
		where (pathwayId=@sourcePathway or pathwayId=@destinationPathway)	
		and genericProcessId=@processId

		fetch @prCrs into @processId
	end
	close @prCrs
	fetch crs into @sourcePathway, @destinationPathway--, @linkingEntity
end
close crs
-- handles processes that are not members of any pathway --- 
---- each such process is considered to be a single process pathway
---- disconnected from the metabolic network
insert into process_graph_nodes
select distinct pr.id, pr.generic_process_id, null
from processes pr
where pr.id not in
(
	select process_id from pathway_processes
)

update process_graph_nodes
set graphNodeId=newId()
where graphNodeId is null

	
END
CREATE PROCEDURE [dbo].[SetLinkingEntitiesInPathwayLinks] 
AS
BEGIN

declare @sourcePathway uniqueidentifier
declare @destinationPathway uniqueidentifier
declare @linkingEntity uniqueidentifier
declare @crs2 cursor

declare crs cursor for
select distinct pathway_id_1, pathway_id_2 
from pathway_links

open crs

fetch crs into @sourcePathway, @destinationPathway

WHILE @@FETCH_STATUS = 0
begin

set @crs2=cursor for
select distinct t1.entity_id
from 
	(
	select distinct entity_id
	from process_entities pe, pathway_processes pp, process_entity_roles per, processes p
	where ((pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in ('product') and p.reversible=0)
        OR (pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in ('product', 'substrate') and p.reversible=1))
  		and entity_id not in (select id from common_molecules)
		and p.id = pe.process_id
		and pp.pathway_id = @sourcePathway 
	) t1,
	(
	select distinct entity_id
	from process_entities pe, pathway_processes pp, process_entity_roles per, processes p
	where ((pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in ('substrate') and p.reversible=0)
        OR (pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in ('product', 'substrate') and p.reversible=1))
  		and entity_id not in (select id from common_molecules)
		and p.id = pe.process_id
		and pp.pathway_id = @destinationPathway
	) t2
where t1.entity_id = t2.entity_id

open @crs2

/*
fetch @crs2 into @linkingEntity

update pathway_links
set entity_id = @linkingEntity
where pathway_id_1 = @sourcePathway
and pathway_id_2 = @destinationPathway
*/
delete from pathway_links
where pathway_id_1 = @sourcePathway
and pathway_id_2 = @destinationPathway


fetch @crs2 into @linkingEntity
WHILE @@FETCH_STATUS = 0
begin
	insert into pathway_links (pathway_id_1, pathway_id_2, entity_id) values(@sourcePathway, @destinationPathway, @linkingEntity)
	fetch @crs2 into @linkingEntity
end
close @crs2

fetch crs into @sourcePathway, @destinationPathway

end
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_organism_groups_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[organism_groups]'))
ALTER TABLE [dbo].[organism_groups]  WITH CHECK ADD  CONSTRAINT [FK_organism_groups_organism_groups] FOREIGN KEY([parent_id])
REFERENCES [dbo].[organism_groups] ([id])
GO
ALTER TABLE [dbo].[organism_groups] CHECK CONSTRAINT [FK_organism_groups_organism_groups]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[organism_groups]'))
ALTER TABLE [dbo].[organism_groups]  WITH CHECK ADD  CONSTRAINT [CK_organism_groups] CHECK  (([common_name] IS NOT NULL OR [scientific_name] IS NOT NULL))
GO
ALTER TABLE [dbo].[organism_groups] CHECK CONSTRAINT [CK_organism_groups]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_numbers]'))
ALTER TABLE [dbo].[ec_numbers]  WITH CHECK ADD  CONSTRAINT [CK_ec_numbers] CHECK  (([ec_number] like '[1-6].%.%.%'))
GO
ALTER TABLE [dbo].[ec_numbers] CHECK CONSTRAINT [CK_ec_numbers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ec_number_name_lookups_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]'))
ALTER TABLE [dbo].[ec_number_name_lookups]  WITH CHECK ADD  CONSTRAINT [FK_ec_number_name_lookups_ec_numbers] FOREIGN KEY([ec_number])
REFERENCES [dbo].[ec_numbers] ([ec_number])
GO
ALTER TABLE [dbo].[ec_number_name_lookups] CHECK CONSTRAINT [FK_ec_number_name_lookups_ec_numbers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ec_number_name_lookups_molecular_entity_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]'))
ALTER TABLE [dbo].[ec_number_name_lookups]  WITH CHECK ADD  CONSTRAINT [FK_ec_number_name_lookups_molecular_entity_names] FOREIGN KEY([name_id])
REFERENCES [dbo].[molecular_entity_names] ([id])
GO
ALTER TABLE [dbo].[ec_number_name_lookups] CHECK CONSTRAINT [FK_ec_number_name_lookups_molecular_entity_names]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ec_number_name_lookups_name_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]'))
ALTER TABLE [dbo].[ec_number_name_lookups]  WITH CHECK ADD  CONSTRAINT [FK_ec_number_name_lookups_name_types] FOREIGN KEY([name_type_id])
REFERENCES [dbo].[name_types] ([name_type_id])
GO
ALTER TABLE [dbo].[ec_number_name_lookups] CHECK CONSTRAINT [FK_ec_number_name_lookups_name_types]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_entity_name_lookups_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]'))
ALTER TABLE [dbo].[entity_name_lookups]  WITH CHECK ADD  CONSTRAINT [FK_entity_name_lookups_molecular_entities] FOREIGN KEY([entity_id])
REFERENCES [dbo].[molecular_entities] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[entity_name_lookups] CHECK CONSTRAINT [FK_entity_name_lookups_molecular_entities]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_entity_name_lookups_molecular_entity_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]'))
ALTER TABLE [dbo].[entity_name_lookups]  WITH CHECK ADD  CONSTRAINT [FK_entity_name_lookups_molecular_entity_names] FOREIGN KEY([name_id])
REFERENCES [dbo].[molecular_entity_names] ([id])
GO
ALTER TABLE [dbo].[entity_name_lookups] CHECK CONSTRAINT [FK_entity_name_lookups_molecular_entity_names]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_entity_name_lookups_name_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]'))
ALTER TABLE [dbo].[entity_name_lookups]  WITH CHECK ADD  CONSTRAINT [FK_entity_name_lookups_name_types] FOREIGN KEY([name_type_id])
REFERENCES [dbo].[name_types] ([name_type_id])
GO
ALTER TABLE [dbo].[entity_name_lookups] CHECK CONSTRAINT [FK_entity_name_lookups_name_types]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_external_database_links_external_databases]') AND parent_object_id = OBJECT_ID(N'[dbo].[OLD_external_database_links]'))
ALTER TABLE [dbo].[OLD_external_database_links]  WITH CHECK ADD  CONSTRAINT [FK_external_database_links_external_databases] FOREIGN KEY([external_database_id])
REFERENCES [dbo].[OLD_external_databases] ([id])
GO
ALTER TABLE [dbo].[OLD_external_database_links] CHECK CONSTRAINT [FK_external_database_links_external_databases]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes]  WITH CHECK ADD  CONSTRAINT [FK_catalyzes_ec_numbers] FOREIGN KEY([ec_number])
REFERENCES [dbo].[ec_numbers] ([ec_number])
GO
ALTER TABLE [dbo].[catalyzes] CHECK CONSTRAINT [FK_catalyzes_ec_numbers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes]  WITH CHECK ADD  CONSTRAINT [FK_catalyzes_gene_products] FOREIGN KEY([gene_product_id])
REFERENCES [dbo].[gene_products] ([id])
GO
ALTER TABLE [dbo].[catalyzes] CHECK CONSTRAINT [FK_catalyzes_gene_products]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes]  WITH CHECK ADD  CONSTRAINT [FK_catalyzes_organism_groups] FOREIGN KEY([organism_group_id])
REFERENCES [dbo].[organism_groups] ([id])
GO
ALTER TABLE [dbo].[catalyzes] CHECK CONSTRAINT [FK_catalyzes_organism_groups]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes]  WITH CHECK ADD  CONSTRAINT [FK_catalyzes_processes] FOREIGN KEY([process_id])
REFERENCES [dbo].[processes] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[catalyzes] CHECK CONSTRAINT [FK_catalyzes_processes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_genes_chromosomes]') AND parent_object_id = OBJECT_ID(N'[dbo].[genes]'))
ALTER TABLE [dbo].[genes]  WITH CHECK ADD  CONSTRAINT [FK_genes_chromosomes] FOREIGN KEY([chromosome_id])
REFERENCES [dbo].[chromosomes] ([id])
GO
ALTER TABLE [dbo].[genes] CHECK CONSTRAINT [FK_genes_chromosomes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_genes_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[genes]'))
ALTER TABLE [dbo].[genes]  WITH CHECK ADD  CONSTRAINT [FK_genes_organism_groups] FOREIGN KEY([organism_group_id])
REFERENCES [dbo].[organism_groups] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[genes] CHECK CONSTRAINT [FK_genes_organism_groups]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_to_pathway_groups_pathway_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]'))
ALTER TABLE [dbo].[pathway_to_pathway_groups]  WITH CHECK ADD  CONSTRAINT [FK_pathway_to_pathway_groups_pathway_groups] FOREIGN KEY([group_id])
REFERENCES [dbo].[pathway_groups] ([group_id])
GO
ALTER TABLE [dbo].[pathway_to_pathway_groups] CHECK CONSTRAINT [FK_pathway_to_pathway_groups_pathway_groups]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_to_pathway_groups_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]'))
ALTER TABLE [dbo].[pathway_to_pathway_groups]  WITH CHECK ADD  CONSTRAINT [FK_pathway_to_pathway_groups_pathways] FOREIGN KEY([pathway_id])
REFERENCES [dbo].[pathways] ([id])
GO
ALTER TABLE [dbo].[pathway_to_pathway_groups] CHECK CONSTRAINT [FK_pathway_to_pathway_groups_pathways]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathways_pathway_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathways]'))
ALTER TABLE [dbo].[pathways]  WITH CHECK ADD  CONSTRAINT [FK_pathways_pathway_types] FOREIGN KEY([pathway_type_id])
REFERENCES [dbo].[pathway_types] ([pathway_type_id])
GO
ALTER TABLE [dbo].[pathways] CHECK CONSTRAINT [FK_pathways_pathway_types]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH CHECK ADD  CONSTRAINT [FK_process_entities_molecular_entities] FOREIGN KEY([entity_id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
ALTER TABLE [dbo].[process_entities] CHECK CONSTRAINT [FK_process_entities_molecular_entities]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_process_entity_roles]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH CHECK ADD  CONSTRAINT [FK_process_entities_process_entity_roles] FOREIGN KEY([role_id])
REFERENCES [dbo].[process_entity_roles] ([role_id])
GO
ALTER TABLE [dbo].[process_entities] CHECK CONSTRAINT [FK_process_entities_process_entity_roles]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH CHECK ADD  CONSTRAINT [FK_process_entities_processes] FOREIGN KEY([process_id])
REFERENCES [dbo].[processes] ([id])
GO
ALTER TABLE [dbo].[process_entities] CHECK CONSTRAINT [FK_process_entities_processes]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_process_entities_quantity]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH CHECK ADD  CONSTRAINT [CK_process_entities_quantity] CHECK  (([quantity]>=(1)))
GO
ALTER TABLE [dbo].[process_entities] CHECK CONSTRAINT [CK_process_entities_quantity]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_rnas_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[rnas]'))
ALTER TABLE [dbo].[rnas]  WITH CHECK ADD  CONSTRAINT [FK_rnas_gene_products] FOREIGN KEY([id])
REFERENCES [dbo].[gene_products] ([id])
GO
ALTER TABLE [dbo].[rnas] CHECK CONSTRAINT [FK_rnas_gene_products]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_rnas_rna_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[rnas]'))
ALTER TABLE [dbo].[rnas]  WITH CHECK ADD  CONSTRAINT [FK_rnas_rna_types] FOREIGN KEY([rna_type_id])
REFERENCES [dbo].[rna_types] ([rna_type_id])
GO
ALTER TABLE [dbo].[rnas] CHECK CONSTRAINT [FK_rnas_rna_types]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_processes_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_processes]'))
ALTER TABLE [dbo].[pathway_processes]  WITH CHECK ADD  CONSTRAINT [FK_pathway_processes_pathways] FOREIGN KEY([pathway_id])
REFERENCES [dbo].[pathways] ([id])
GO
ALTER TABLE [dbo].[pathway_processes] CHECK CONSTRAINT [FK_pathway_processes_pathways]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_processes_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_processes]'))
ALTER TABLE [dbo].[pathway_processes]  WITH CHECK ADD  CONSTRAINT [FK_pathway_processes_processes] FOREIGN KEY([process_id])
REFERENCES [dbo].[processes] ([id])
GO
ALTER TABLE [dbo].[pathway_processes] CHECK CONSTRAINT [FK_pathway_processes_processes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_attribute_values_attribute_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[attribute_values]'))
ALTER TABLE [dbo].[attribute_values]  WITH NOCHECK ADD  CONSTRAINT [FK_attribute_values_attribute_names] FOREIGN KEY([attributeId])
REFERENCES [dbo].[attribute_names] ([attributeId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[attribute_values] CHECK CONSTRAINT [FK_attribute_values_attribute_names]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_basic_molecules_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[basic_molecules]'))
ALTER TABLE [dbo].[basic_molecules]  WITH CHECK ADD  CONSTRAINT [FK_basic_molecules_molecular_entities] FOREIGN KEY([id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
ALTER TABLE [dbo].[basic_molecules] CHECK CONSTRAINT [FK_basic_molecules_molecular_entities]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_gene_products_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[gene_products]'))
ALTER TABLE [dbo].[gene_products]  WITH CHECK ADD  CONSTRAINT [FK_gene_products_molecular_entities] FOREIGN KEY([id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
ALTER TABLE [dbo].[gene_products] CHECK CONSTRAINT [FK_gene_products_molecular_entities]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_links_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_links]'))
ALTER TABLE [dbo].[pathway_links]  WITH CHECK ADD  CONSTRAINT [FK_pathway_links_molecular_entities] FOREIGN KEY([entity_id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
ALTER TABLE [dbo].[pathway_links] CHECK CONSTRAINT [FK_pathway_links_molecular_entities]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_links_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_links]'))
ALTER TABLE [dbo].[pathway_links]  WITH CHECK ADD  CONSTRAINT [FK_pathway_links_pathways] FOREIGN KEY([pathway_id_1])
REFERENCES [dbo].[pathways] ([id])
GO
ALTER TABLE [dbo].[pathway_links] CHECK CONSTRAINT [FK_pathway_links_pathways]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_links_pathways1]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_links]'))
ALTER TABLE [dbo].[pathway_links]  WITH CHECK ADD  CONSTRAINT [FK_pathway_links_pathways1] FOREIGN KEY([pathway_id_2])
REFERENCES [dbo].[pathways] ([id])
GO
ALTER TABLE [dbo].[pathway_links] CHECK CONSTRAINT [FK_pathway_links_pathways1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_common_molecules_basic_molecules]') AND parent_object_id = OBJECT_ID(N'[dbo].[common_molecules]'))
ALTER TABLE [dbo].[common_molecules]  WITH CHECK ADD  CONSTRAINT [FK_common_molecules_basic_molecules] FOREIGN KEY([id])
REFERENCES [dbo].[basic_molecules] ([id])
GO
ALTER TABLE [dbo].[common_molecules] CHECK CONSTRAINT [FK_common_molecules_basic_molecules]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_gene_and_gene_products_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[gene_encodings]'))
ALTER TABLE [dbo].[gene_encodings]  WITH CHECK ADD  CONSTRAINT [FK_gene_and_gene_products_gene_products] FOREIGN KEY([gene_product_id])
REFERENCES [dbo].[gene_products] ([id])
GO
ALTER TABLE [dbo].[gene_encodings] CHECK CONSTRAINT [FK_gene_and_gene_products_gene_products]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_proteins_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[proteins]'))
ALTER TABLE [dbo].[proteins]  WITH CHECK ADD  CONSTRAINT [FK_proteins_gene_products] FOREIGN KEY([id])
REFERENCES [dbo].[gene_products] ([id])
GO
ALTER TABLE [dbo].[proteins] CHECK CONSTRAINT [FK_proteins_gene_products]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_molecular_entities_molecular_entity_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[molecular_entities]'))
ALTER TABLE [dbo].[molecular_entities]  WITH CHECK ADD  CONSTRAINT [FK_molecular_entities_molecular_entity_names] FOREIGN KEY([name])
REFERENCES [dbo].[molecular_entity_names] ([name])
GO
ALTER TABLE [dbo].[molecular_entities] CHECK CONSTRAINT [FK_molecular_entities_molecular_entity_names]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_molecular_entities_molecular_entity_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[molecular_entities]'))
ALTER TABLE [dbo].[molecular_entities]  WITH CHECK ADD  CONSTRAINT [FK_molecular_entities_molecular_entity_types] FOREIGN KEY([type_id])
REFERENCES [dbo].[molecular_entity_types] ([type_id])
GO
ALTER TABLE [dbo].[molecular_entities] CHECK CONSTRAINT [FK_molecular_entities_molecular_entity_types]
