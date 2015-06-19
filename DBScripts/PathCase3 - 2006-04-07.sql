if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_genes_chromosomes]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[genes] DROP CONSTRAINT FK_genes_chromosomes
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_catalyzes_ec_numbers]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[catalyzes] DROP CONSTRAINT FK_catalyzes_ec_numbers
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ec_number_name_lookups_ec_numbers]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ec_number_name_lookups] DROP CONSTRAINT FK_ec_number_name_lookups_ec_numbers
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_external_database_links_external_databases]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[external_database_links] DROP CONSTRAINT FK_external_database_links_external_databases
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ec_number_name_lookups_molecular_entity_names]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ec_number_name_lookups] DROP CONSTRAINT FK_ec_number_name_lookups_molecular_entity_names
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_entity_name_lookups_molecular_entity_names]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[entity_name_lookups] DROP CONSTRAINT FK_entity_name_lookups_molecular_entity_names
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_molecular_entities_molecular_entity_names]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[molecular_entities] DROP CONSTRAINT FK_molecular_entities_molecular_entity_names
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_catalyzes_organism_groups]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[catalyzes] DROP CONSTRAINT FK_catalyzes_organism_groups
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_genes_organism_groups]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[genes] DROP CONSTRAINT FK_genes_organism_groups
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_organism_groups_organism_groups]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[organism_groups] DROP CONSTRAINT FK_organism_groups_organism_groups
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_organisms_organism_groups]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[organisms] DROP CONSTRAINT FK_organisms_organism_groups
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_pathway_to_pathway_groups_pathway_groups]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[pathway_to_pathway_groups] DROP CONSTRAINT FK_pathway_to_pathway_groups_pathway_groups
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_pathway_links_pathways]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[pathway_links] DROP CONSTRAINT FK_pathway_links_pathways
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_pathway_links_pathways1]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[pathway_links] DROP CONSTRAINT FK_pathway_links_pathways1
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_pathway_processes_pathways]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[pathway_processes] DROP CONSTRAINT FK_pathway_processes_pathways
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_pathway_to_pathway_groups_pathways]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[pathway_to_pathway_groups] DROP CONSTRAINT FK_pathway_to_pathway_groups_pathways
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_catalyzes_processes]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[catalyzes] DROP CONSTRAINT FK_catalyzes_processes
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_pathway_processes_processes]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[pathway_processes] DROP CONSTRAINT FK_pathway_processes_processes
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_process_entities_processes]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[process_entities] DROP CONSTRAINT FK_process_entities_processes
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_gene_and_gene_products_genes]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[gene_encodings] DROP CONSTRAINT FK_gene_and_gene_products_genes
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_basic_molecules_molecular_entities]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[basic_molecules] DROP CONSTRAINT FK_basic_molecules_molecular_entities
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_entity_name_lookups_molecular_entities]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[entity_name_lookups] DROP CONSTRAINT FK_entity_name_lookups_molecular_entities
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_gene_products_molecular_entities]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[gene_products] DROP CONSTRAINT FK_gene_products_molecular_entities
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_pathway_links_molecular_entities]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[pathway_links] DROP CONSTRAINT FK_pathway_links_molecular_entities
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_process_entities_molecular_entities]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[process_entities] DROP CONSTRAINT FK_process_entities_molecular_entities
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_common_molecules_basic_molecules]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[common_molecules] DROP CONSTRAINT FK_common_molecules_basic_molecules
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_catalyzes_gene_products]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[catalyzes] DROP CONSTRAINT FK_catalyzes_gene_products
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_gene_and_gene_products_gene_products]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[gene_encodings] DROP CONSTRAINT FK_gene_and_gene_products_gene_products
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_proteins_gene_products]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[proteins] DROP CONSTRAINT FK_proteins_gene_products
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_rnas_gene_products]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[rnas] DROP CONSTRAINT FK_rnas_gene_products
GO

/****** Object:  Table [dbo].[catalyzes]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[catalyzes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[catalyzes]
GO

/****** Object:  Table [dbo].[common_molecules]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[common_molecules]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[common_molecules]
GO

/****** Object:  Table [dbo].[gene_encodings]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[gene_encodings]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[gene_encodings]
GO

/****** Object:  Table [dbo].[proteins]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[proteins]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[proteins]
GO

/****** Object:  Table [dbo].[rnas]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rnas]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[rnas]
GO

/****** Object:  Table [dbo].[basic_molecules]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[basic_molecules]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[basic_molecules]
GO

/****** Object:  Table [dbo].[entity_name_lookups]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[entity_name_lookups]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[entity_name_lookups]
GO

/****** Object:  Table [dbo].[gene_products]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[gene_products]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[gene_products]
GO

/****** Object:  Table [dbo].[pathway_links]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pathway_links]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[pathway_links]
GO

/****** Object:  Table [dbo].[process_entities]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[process_entities]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[process_entities]
GO

/****** Object:  Table [dbo].[ec_number_name_lookups]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ec_number_name_lookups]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ec_number_name_lookups]
GO

/****** Object:  Table [dbo].[external_database_links]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[external_database_links]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[external_database_links]
GO

/****** Object:  Table [dbo].[genes]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[genes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[genes]
GO

/****** Object:  Table [dbo].[molecular_entities]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[molecular_entities]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[molecular_entities]
GO

/****** Object:  Table [dbo].[organisms]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[organisms]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[organisms]
GO

/****** Object:  Table [dbo].[pathway_processes]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pathway_processes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[pathway_processes]
GO

/****** Object:  Table [dbo].[pathway_to_pathway_groups]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pathway_to_pathway_groups]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[pathway_to_pathway_groups]
GO

/****** Object:  Table [dbo].[chromosomes]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[chromosomes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[chromosomes]
GO

/****** Object:  Table [dbo].[ec_numbers]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ec_numbers]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ec_numbers]
GO

/****** Object:  Table [dbo].[external_databases]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[external_databases]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[external_databases]
GO

/****** Object:  Table [dbo].[molecular_entity_names]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[molecular_entity_names]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[molecular_entity_names]
GO

/****** Object:  Table [dbo].[organism_groups]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[organism_groups]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[organism_groups]
GO

/****** Object:  Table [dbo].[pathway_groups]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pathway_groups]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[pathway_groups]
GO

/****** Object:  Table [dbo].[pathways]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pathways]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[pathways]
GO

/****** Object:  Table [dbo].[processes]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[processes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[processes]
GO

/****** Object:  Table [dbo].[viewState]    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[viewState]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[viewState]
GO

/****** Object:  User Defined Function dbo.Get_molecular_entity_type    Script Date: 4/7/2006 4:22:44 PM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Get_molecular_entity_type]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[Get_molecular_entity_type]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  User Defined Function dbo.Get_molecular_entity_type    Script Date: 4/7/2006 4:22:52 PM ******/
CREATE FUNCTION Get_molecular_entity_type (@entity_id uniqueidentifier)  
RETURNS varchar(15) AS  
BEGIN 
	DECLARE @type varchar(15)
	SELECT @type = type FROM molecular_entities WHERE id = @entity_id
	RETURN @type
END


GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

/****** Object:  Table [dbo].[chromosomes]    Script Date: 4/7/2006 4:22:46 PM ******/
CREATE TABLE [dbo].[chromosomes] (
	[id] [uniqueidentifier] NOT NULL ,
	[name] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[length] [decimal](10, 2) NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ec_numbers]    Script Date: 4/7/2006 4:22:46 PM ******/
CREATE TABLE [dbo].[ec_numbers] (
	[ec_number] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[name] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[external_databases]    Script Date: 4/7/2006 4:22:47 PM ******/
CREATE TABLE [dbo].[external_databases] (
	[id] [uniqueidentifier] NOT NULL ,
	[name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[molecular_entity_names]    Script Date: 4/7/2006 4:22:47 PM ******/
CREATE TABLE [dbo].[molecular_entity_names] (
	[id] [uniqueidentifier] NOT NULL ,
	[name] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[organism_groups]    Script Date: 4/7/2006 4:22:47 PM ******/
CREATE TABLE [dbo].[organism_groups] (
	[id] [uniqueidentifier] NOT NULL ,
	[scientific_name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[common_name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[parent_id] [uniqueidentifier] NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[is_organism] [bit] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[pathway_groups]    Script Date: 4/7/2006 4:22:47 PM ******/
CREATE TABLE [dbo].[pathway_groups] (
	[group_id] [uniqueidentifier] NOT NULL ,
	[name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[pathways]    Script Date: 4/7/2006 4:22:47 PM ******/
CREATE TABLE [dbo].[pathways] (
	[id] [uniqueidentifier] NOT NULL ,
	[name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[type] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[status] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[processes]    Script Date: 4/7/2006 4:22:48 PM ******/
CREATE TABLE [dbo].[processes] (
	[id]  uniqueidentifier ROWGUIDCOL  NOT NULL ,
	[name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[reversible] [bit] NULL ,
	[location] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[type] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[generic_process_id] [uniqueidentifier] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[viewState]    Script Date: 4/7/2006 4:22:48 PM ******/
CREATE TABLE [dbo].[viewState] (
	[viewID] [uniqueidentifier] NOT NULL ,
	[openSection] [varchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[organism] [varchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[openNode1ID] [uniqueidentifier] NULL ,
	[openNode1Type] [varchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[openNode2ID] [uniqueidentifier] NULL ,
	[openNode2Type] [varchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[openNode3ID] [uniqueidentifier] NULL ,
	[openNode3Type] [varchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[displayItemID] [uniqueidentifier] NULL ,
	[displayItemType] [varchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[viewGraph] [tinyint] NULL ,
	[timeStamp] [datetime] NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ec_number_name_lookups]    Script Date: 4/7/2006 4:22:48 PM ******/
CREATE TABLE [dbo].[ec_number_name_lookups] (
	[ec_number] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[name_id] [uniqueidentifier] NOT NULL ,
	[type] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[external_database_links]    Script Date: 4/7/2006 4:22:48 PM ******/
CREATE TABLE [dbo].[external_database_links] (
	[local_id] [uniqueidentifier] NOT NULL ,
	[external_database_id] [uniqueidentifier] NOT NULL ,
	[id_in_external_database] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[genes]    Script Date: 4/7/2006 4:22:48 PM ******/
CREATE TABLE [dbo].[genes] (
	[id] [uniqueidentifier] NOT NULL ,
	[organism_group_id] [uniqueidentifier] NULL ,
	[chromosome_id] [uniqueidentifier] NULL ,
	[homologue_group_id] [uniqueidentifier] NOT NULL ,
	[raw_address] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[cytogenic_address] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[genetic_address] [bigint] NULL ,
	[relative_address] [decimal](10, 3) NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[molecular_entities]    Script Date: 4/7/2006 4:22:49 PM ******/
CREATE TABLE [dbo].[molecular_entities] (
	[id] [uniqueidentifier] NOT NULL ,
	[type] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[name] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[organisms]    Script Date: 4/7/2006 4:22:49 PM ******/
CREATE TABLE [dbo].[organisms] (
	[id] [uniqueidentifier] NOT NULL ,
	[taxonomy_id] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[pathway_processes]    Script Date: 4/7/2006 4:22:49 PM ******/
CREATE TABLE [dbo].[pathway_processes] (
	[pathway_id] [uniqueidentifier] NOT NULL ,
	[process_id] [uniqueidentifier] NOT NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[pathway_to_pathway_groups]    Script Date: 4/7/2006 4:22:49 PM ******/
CREATE TABLE [dbo].[pathway_to_pathway_groups] (
	[pathway_id] [uniqueidentifier] NOT NULL ,
	[group_id] [uniqueidentifier] NOT NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[basic_molecules]    Script Date: 4/7/2006 4:22:50 PM ******/
CREATE TABLE [dbo].[basic_molecules] (
	[id] [uniqueidentifier] NOT NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[entity_name_lookups]    Script Date: 4/7/2006 4:22:50 PM ******/
CREATE TABLE [dbo].[entity_name_lookups] (
	[entity_id] [uniqueidentifier] NOT NULL ,
	[name_id] [uniqueidentifier] NOT NULL ,
	[type] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[gene_products]    Script Date: 4/7/2006 4:22:50 PM ******/
CREATE TABLE [dbo].[gene_products] (
	[id] [uniqueidentifier] NOT NULL ,
	[type] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[pathway_links]    Script Date: 4/7/2006 4:22:50 PM ******/
CREATE TABLE [dbo].[pathway_links] (
	[pathway_id_1] [uniqueidentifier] NOT NULL ,
	[pathway_id_2] [uniqueidentifier] NOT NULL ,
	[entity_id] [uniqueidentifier] NOT NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[process_entities]    Script Date: 4/7/2006 4:22:50 PM ******/
CREATE TABLE [dbo].[process_entities] (
	[process_id] [uniqueidentifier] NOT NULL ,
	[entity_id] [uniqueidentifier] NOT NULL ,
	[role] [varchar] (18) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[quantity] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[catalyzes]    Script Date: 4/7/2006 4:22:51 PM ******/
CREATE TABLE [dbo].[catalyzes] (
	[process_id] [uniqueidentifier] NOT NULL ,
	[organism_group_id] [uniqueidentifier] NOT NULL ,
	[gene_product_id] [uniqueidentifier] NULL ,
	[ec_number] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[common_molecules]    Script Date: 4/7/2006 4:22:51 PM ******/
CREATE TABLE [dbo].[common_molecules] (
	[id] [uniqueidentifier] NOT NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[gene_encodings]    Script Date: 4/7/2006 4:22:51 PM ******/
CREATE TABLE [dbo].[gene_encodings] (
	[gene_id] [uniqueidentifier] NOT NULL ,
	[gene_product_id] [uniqueidentifier] NOT NULL ,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[proteins]    Script Date: 4/7/2006 4:22:51 PM ******/
CREATE TABLE [dbo].[proteins] (
	[id] [uniqueidentifier] NOT NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[rnas]    Script Date: 4/7/2006 4:22:52 PM ******/
CREATE TABLE [dbo].[rnas] (
	[id] [uniqueidentifier] NOT NULL ,
	[type] [varchar] (4) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[chromosomes] WITH NOCHECK ADD 
	CONSTRAINT [PK_chromosomes] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ec_numbers] WITH NOCHECK ADD 
	CONSTRAINT [PK_ec_numbers] PRIMARY KEY  CLUSTERED 
	(
		[ec_number]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[external_databases] WITH NOCHECK ADD 
	CONSTRAINT [PK_external_databases] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[molecular_entity_names] WITH NOCHECK ADD 
	CONSTRAINT [PK_molecular_entity_names] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[organism_groups] WITH NOCHECK ADD 
	CONSTRAINT [PK_organism_groups] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[pathway_groups] WITH NOCHECK ADD 
	CONSTRAINT [PK_groups] PRIMARY KEY  CLUSTERED 
	(
		[group_id]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[pathways] WITH NOCHECK ADD 
	CONSTRAINT [PK_pathways] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[processes] WITH NOCHECK ADD 
	CONSTRAINT [PK_processes] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ec_number_name_lookups] WITH NOCHECK ADD 
	CONSTRAINT [PK_ec_number_name_lookups] PRIMARY KEY  CLUSTERED 
	(
		[ec_number],
		[name_id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[external_database_links] WITH NOCHECK ADD 
	CONSTRAINT [PK_external_database_links] PRIMARY KEY  CLUSTERED 
	(
		[local_id],
		[external_database_id],
		[id_in_external_database]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[genes] WITH NOCHECK ADD 
	CONSTRAINT [PK_genes] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[molecular_entities] WITH NOCHECK ADD 
	CONSTRAINT [PK_molecular_entities] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[organisms] WITH NOCHECK ADD 
	CONSTRAINT [PK_organisms] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[pathway_processes] WITH NOCHECK ADD 
	CONSTRAINT [PK_pathway_processes] PRIMARY KEY  CLUSTERED 
	(
		[pathway_id],
		[process_id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[pathway_to_pathway_groups] WITH NOCHECK ADD 
	CONSTRAINT [PK_pathways_groups] PRIMARY KEY  CLUSTERED 
	(
		[pathway_id],
		[group_id]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[basic_molecules] WITH NOCHECK ADD 
	CONSTRAINT [PK_basic_molecules] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[entity_name_lookups] WITH NOCHECK ADD 
	CONSTRAINT [PK_entity_name_lookups] PRIMARY KEY  CLUSTERED 
	(
		[entity_id],
		[name_id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[gene_products] WITH NOCHECK ADD 
	CONSTRAINT [PK_gene_products] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[pathway_links] WITH NOCHECK ADD 
	CONSTRAINT [PK_pathway_links] PRIMARY KEY  CLUSTERED 
	(
		[pathway_id_1],
		[pathway_id_2],
		[entity_id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[process_entities] WITH NOCHECK ADD 
	CONSTRAINT [PK_process_entities] PRIMARY KEY  CLUSTERED 
	(
		[process_id],
		[entity_id],
		[role]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[common_molecules] WITH NOCHECK ADD 
	CONSTRAINT [PK_common_molecules] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[gene_encodings] WITH NOCHECK ADD 
	CONSTRAINT [PK_gene_and_gene_products] PRIMARY KEY  CLUSTERED 
	(
		[gene_id],
		[gene_product_id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[proteins] WITH NOCHECK ADD 
	CONSTRAINT [PK_proteins] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[rnas] WITH NOCHECK ADD 
	CONSTRAINT [PK_rnas] PRIMARY KEY  CLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ec_numbers] ADD 
	CONSTRAINT [CK_ec_numbers] CHECK ([ec_number] like '[1-6].%.%.%')
GO

ALTER TABLE [dbo].[molecular_entity_names] ADD 
	CONSTRAINT [IX_molecular_entity_names] UNIQUE  NONCLUSTERED 
	(
		[name]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[organism_groups] ADD 
	CONSTRAINT [DF_organism_groups_is_organism] DEFAULT (0) FOR [is_organism],
	CONSTRAINT [CK_organism_groups] CHECK ([common_name] is not null or [scientific_name] is not null)
GO

ALTER TABLE [dbo].[pathway_groups] ADD 
	CONSTRAINT [DF_groups_group_id] DEFAULT (newid()) FOR [group_id]
GO

ALTER TABLE [dbo].[processes] ADD 
	CONSTRAINT [DF_processes_id] DEFAULT (newid()) FOR [id],
	CONSTRAINT [DF_processes_reversible] DEFAULT (0) FOR [reversible]
GO

ALTER TABLE [dbo].[genes] ADD 
	CONSTRAINT [IX_genes] UNIQUE  NONCLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] ,
	CONSTRAINT [CK_genes_check_type] CHECK ([dbo].[Get_molecular_entity_type]([id]) = 'genes')
GO

ALTER TABLE [dbo].[molecular_entities] ADD 
	CONSTRAINT [CK_molecular_entities_types] CHECK ([type] = 'rnas' or ([type] = 'proteins' or ([type] = 'genes' or ([type] = 'basic_molecules' or [type] = 'amino_acids'))))
GO

ALTER TABLE [dbo].[basic_molecules] ADD 
	CONSTRAINT [CK_basic_molecules_check_type] CHECK ([dbo].[Get_molecular_entity_type]([id]) = 'basic_molecules')
GO

ALTER TABLE [dbo].[gene_products] ADD 
	CONSTRAINT [IX_gene_products] UNIQUE  NONCLUSTERED 
	(
		[id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] ,
	CONSTRAINT [CK_gene_products_check_type] CHECK ([dbo].[Get_molecular_entity_type]([id]) = 'proteins' or [dbo].[Get_molecular_entity_type]([id]) = 'rnas')
GO

ALTER TABLE [dbo].[process_entities] ADD 
	CONSTRAINT [DF_process_entities_quantity] DEFAULT (1) FOR [quantity],
	CONSTRAINT [CK_process_entities_quantity] CHECK ([quantity] >= 1),
	CONSTRAINT [CK_process_entities_role_types] CHECK ([role] = 'product' or [role] = 'substrate' or [role] = 'cofactor in' or [role] = 'cofactor out' or [role] = 'activator' or [role] = 'inhibitor' or [role] = 'regulator')
GO

ALTER TABLE [dbo].[catalyzes] ADD 
	CONSTRAINT [IX_catalyzes_all] UNIQUE  NONCLUSTERED 
	(
		[gene_product_id],
		[process_id],
		[ec_number],
		[organism_group_id]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

 CREATE  INDEX [IX_catalyzes_process] ON [dbo].[catalyzes]([process_id]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_catalyzes_org] ON [dbo].[catalyzes]([organism_group_id]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_catalyzes_gene] ON [dbo].[catalyzes]([gene_product_id]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_catalyzes_ec] ON [dbo].[catalyzes]([ec_number]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_catalyzes_process_org] ON [dbo].[catalyzes]([process_id], [organism_group_id]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_catalyzes_except_ec] ON [dbo].[catalyzes]([process_id], [organism_group_id], [gene_product_id]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_catalyzes_gene_process] ON [dbo].[catalyzes]([gene_product_id], [process_id]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_catalyzes_gene_org] ON [dbo].[catalyzes]([organism_group_id], [gene_product_id]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[proteins] ADD 
	CONSTRAINT [CK_proteins_check_type] CHECK ([dbo].[Get_molecular_entity_type]([id]) = 'proteins')
GO

ALTER TABLE [dbo].[rnas] ADD 
	CONSTRAINT [CK_rnas_check_type] CHECK ([dbo].[Get_molecular_entity_type]([id]) = 'rnas')
GO

ALTER TABLE [dbo].[organism_groups] ADD 
	CONSTRAINT [FK_organism_groups_organism_groups] FOREIGN KEY 
	(
		[parent_id]
	) REFERENCES [dbo].[organism_groups] (
		[id]
	)
GO

ALTER TABLE [dbo].[ec_number_name_lookups] ADD 
	CONSTRAINT [FK_ec_number_name_lookups_ec_numbers] FOREIGN KEY 
	(
		[ec_number]
	) REFERENCES [dbo].[ec_numbers] (
		[ec_number]
	),
	CONSTRAINT [FK_ec_number_name_lookups_molecular_entity_names] FOREIGN KEY 
	(
		[name_id]
	) REFERENCES [dbo].[molecular_entity_names] (
		[id]
	)
GO

ALTER TABLE [dbo].[external_database_links] ADD 
	CONSTRAINT [FK_external_database_links_external_databases] FOREIGN KEY 
	(
		[external_database_id]
	) REFERENCES [dbo].[external_databases] (
		[id]
	)
GO

ALTER TABLE [dbo].[genes] ADD 
	CONSTRAINT [FK_genes_chromosomes] FOREIGN KEY 
	(
		[chromosome_id]
	) REFERENCES [dbo].[chromosomes] (
		[id]
	),
	CONSTRAINT [FK_genes_organism_groups] FOREIGN KEY 
	(
		[organism_group_id]
	) REFERENCES [dbo].[organism_groups] (
		[id]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[molecular_entities] ADD 
	CONSTRAINT [FK_molecular_entities_molecular_entity_names] FOREIGN KEY 
	(
		[name]
	) REFERENCES [dbo].[molecular_entity_names] (
		[name]
	)
GO

ALTER TABLE [dbo].[organisms] ADD 
	CONSTRAINT [FK_organisms_organism_groups] FOREIGN KEY 
	(
		[id]
	) REFERENCES [dbo].[organism_groups] (
		[id]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[pathway_processes] ADD 
	CONSTRAINT [FK_pathway_processes_pathways] FOREIGN KEY 
	(
		[pathway_id]
	) REFERENCES [dbo].[pathways] (
		[id]
	),
	CONSTRAINT [FK_pathway_processes_processes] FOREIGN KEY 
	(
		[process_id]
	) REFERENCES [dbo].[processes] (
		[id]
	)
GO

ALTER TABLE [dbo].[pathway_to_pathway_groups] ADD 
	CONSTRAINT [FK_pathway_to_pathway_groups_pathway_groups] FOREIGN KEY 
	(
		[group_id]
	) REFERENCES [dbo].[pathway_groups] (
		[group_id]
	),
	CONSTRAINT [FK_pathway_to_pathway_groups_pathways] FOREIGN KEY 
	(
		[pathway_id]
	) REFERENCES [dbo].[pathways] (
		[id]
	)
GO

ALTER TABLE [dbo].[basic_molecules] ADD 
	CONSTRAINT [FK_basic_molecules_molecular_entities] FOREIGN KEY 
	(
		[id]
	) REFERENCES [dbo].[molecular_entities] (
		[id]
	)
GO

ALTER TABLE [dbo].[entity_name_lookups] ADD 
	CONSTRAINT [FK_entity_name_lookups_molecular_entities] FOREIGN KEY 
	(
		[entity_id]
	) REFERENCES [dbo].[molecular_entities] (
		[id]
	),
	CONSTRAINT [FK_entity_name_lookups_molecular_entity_names] FOREIGN KEY 
	(
		[name_id]
	) REFERENCES [dbo].[molecular_entity_names] (
		[id]
	)
GO

ALTER TABLE [dbo].[gene_products] ADD 
	CONSTRAINT [FK_gene_products_molecular_entities] FOREIGN KEY 
	(
		[id]
	) REFERENCES [dbo].[molecular_entities] (
		[id]
	)
GO

ALTER TABLE [dbo].[pathway_links] ADD 
	CONSTRAINT [FK_pathway_links_molecular_entities] FOREIGN KEY 
	(
		[entity_id]
	) REFERENCES [dbo].[molecular_entities] (
		[id]
	),
	CONSTRAINT [FK_pathway_links_pathways] FOREIGN KEY 
	(
		[pathway_id_1]
	) REFERENCES [dbo].[pathways] (
		[id]
	),
	CONSTRAINT [FK_pathway_links_pathways1] FOREIGN KEY 
	(
		[pathway_id_2]
	) REFERENCES [dbo].[pathways] (
		[id]
	)
GO

ALTER TABLE [dbo].[process_entities] ADD 
	CONSTRAINT [FK_process_entities_molecular_entities] FOREIGN KEY 
	(
		[entity_id]
	) REFERENCES [dbo].[molecular_entities] (
		[id]
	),
	CONSTRAINT [FK_process_entities_processes] FOREIGN KEY 
	(
		[process_id]
	) REFERENCES [dbo].[processes] (
		[id]
	)
GO

ALTER TABLE [dbo].[catalyzes] ADD 
	CONSTRAINT [FK_catalyzes_ec_numbers] FOREIGN KEY 
	(
		[ec_number]
	) REFERENCES [dbo].[ec_numbers] (
		[ec_number]
	),
	CONSTRAINT [FK_catalyzes_gene_products] FOREIGN KEY 
	(
		[gene_product_id]
	) REFERENCES [dbo].[gene_products] (
		[id]
	),
	CONSTRAINT [FK_catalyzes_organism_groups] FOREIGN KEY 
	(
		[organism_group_id]
	) REFERENCES [dbo].[organism_groups] (
		[id]
	) ON DELETE CASCADE  ON UPDATE CASCADE ,
	CONSTRAINT [FK_catalyzes_processes] FOREIGN KEY 
	(
		[process_id]
	) REFERENCES [dbo].[processes] (
		[id]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[common_molecules] ADD 
	CONSTRAINT [FK_common_molecules_basic_molecules] FOREIGN KEY 
	(
		[id]
	) REFERENCES [dbo].[basic_molecules] (
		[id]
	)
GO

ALTER TABLE [dbo].[gene_encodings] ADD 
	CONSTRAINT [FK_gene_and_gene_products_gene_products] FOREIGN KEY 
	(
		[gene_product_id]
	) REFERENCES [dbo].[gene_products] (
		[id]
	),
	CONSTRAINT [FK_gene_and_gene_products_genes] FOREIGN KEY 
	(
		[gene_id]
	) REFERENCES [dbo].[genes] (
		[id]
	)
GO

ALTER TABLE [dbo].[proteins] ADD 
	CONSTRAINT [FK_proteins_gene_products] FOREIGN KEY 
	(
		[id]
	) REFERENCES [dbo].[gene_products] (
		[id]
	)
GO

ALTER TABLE [dbo].[rnas] ADD 
	CONSTRAINT [FK_rnas_gene_products] FOREIGN KEY 
	(
		[id]
	) REFERENCES [dbo].[gene_products] (
		[id]
	)
GO


exec sp_addextendedproperty N'MS_Description', N'the default name or official name of the EC#', N'user', N'dbo', N'table', N'ec_numbers', N'column', N'name'


GO


exec sp_addextendedproperty N'MS_Description', N'The scientific name', N'user', N'dbo', N'table', N'organism_groups', N'column', N'scientific_name'
GO
exec sp_addextendedproperty N'MS_Description', N'the immediate parent organism', N'user', N'dbo', N'table', N'organism_groups', N'column', N'parent_id'
GO
exec sp_addextendedproperty N'MS_Description', N'Designates if this is an organism or group', N'user', N'dbo', N'table', N'organism_groups', N'column', N'is_organism'


GO


exec sp_addextendedproperty N'MS_Description', N'the homolog gene group. it''s also used to identify the homolog proteins and processes.', N'user', N'dbo', N'table', N'genes', N'column', N'homologue_group_id'


GO


exec sp_addextendedproperty N'MS_Description', N'the default name or offical name of the molecular entity', N'user', N'dbo', N'table', N'molecular_entities', N'column', N'name'


GO