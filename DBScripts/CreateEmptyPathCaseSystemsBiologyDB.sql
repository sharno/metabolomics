USE [master]
GO
/****** Object:  Database [PathCase_SystemBiology_NEW]    Script Date: 03/29/2011 20:59:11 ******/
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'PathCase_SystemBiology_NEW')
BEGIN
CREATE DATABASE [PathCase_SystemBiology_NEW] ON  PRIMARY 
( NAME = N'PathCase_SystemBiology_Data', FILENAME = N'C:\PathCase Databases\PathCase System Biology NEW\PathCase_SystemBiology_Data.MDF' , SIZE = 5461120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 10%)
 LOG ON 
( NAME = N'PathCase_SystemBiology_Log', FILENAME = N'C:\PathCase Databases\PathCase System Biology NEW\PathCase_SystemBiology_Log.LDF' , SIZE = 2614848KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
END

EXEC dbo.sp_dbcmptlevel @dbname=N'PathCase_SystemBiology_NEW', @new_cmptlevel=80
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PathCase_SystemBiology_NEW].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET ARITHABORT OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET  DISABLE_BROKER 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET  READ_WRITE 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET  MULTI_USER 
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET PAGE_VERIFY TORN_PAGE_DETECTION  
GO
ALTER DATABASE [PathCase_SystemBiology_NEW] SET DB_CHAINING OFF 

USE [PathCase_SystemBiology_NEW]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'NETWORK SERVICE')
CREATE USER [NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE] WITH DEFAULT_SCHEMA=[dbo]
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'pathcase')
EXEC sys.sp_executesql N'CREATE SCHEMA [pathcase] AUTHORIZATION [dbo]'

GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'NETWORK SERVICE')
EXEC sys.sp_executesql N'CREATE SCHEMA [NETWORK SERVICE] AUTHORIZATION [NETWORK SERVICE]'

/****** Object:  Table [dbo].[CompartmentClassDictionary]    Script Date: 03/29/2011 22:54:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CompartmentClassDictionary]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CompartmentClassDictionary](
	[compartmentName] [varchar](100) NULL,
	[compartmentClassId] [uniqueidentifier] NULL
) ON [PRIMARY]
END
GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[common_species]    Script Date: 03/29/2011 22:52:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[common_species]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[common_species](
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_common_species] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[CompartmentClass]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CompartmentClass]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CompartmentClass](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[parentid] [uniqueidentifier] NULL,
 CONSTRAINT [UQ__Compartm__3213E83E7DB89C09] UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DataSource]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataSource]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DataSource](
	[id] [smallint] NOT NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[url] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_DataSource] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DataSource]') AND name = N'IX_DataSource')
CREATE UNIQUE NONCLUSTERED INDEX [IX_DataSource] ON [dbo].[DataSource] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[entity_graph_nodes]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[entity_graph_nodes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[entity_graph_nodes](
	[pathwayId] [uniqueidentifier] NULL,
	[entityId] [uniqueidentifier] NOT NULL,
	[graphNodeId] [uniqueidentifier] NULL
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[entity_graph_nodes]') AND name = N'IX_entity_graph_nodes')
CREATE UNIQUE CLUSTERED INDEX [IX_entity_graph_nodes] ON [dbo].[entity_graph_nodes] 
(
	[pathwayId] ASC,
	[entityId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[entity_graph_nodes]') AND name = N'IX_pathway_entities')
CREATE NONCLUSTERED INDEX [IX_pathway_entities] ON [dbo].[entity_graph_nodes] 
(
	[graphNodeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ec_numbers]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ec_numbers]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ec_numbers](
	[ec_number] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[nodeCode] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ec_numbers] PRIMARY KEY CLUSTERED 
(
	[ec_number] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
ELSE
BEGIN
   ALTER TABLE [dbo].[ec_numbers]
   ALTER COLUMN [notes] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
   EXEC sp_rename 'dbo.ec_numbers.notes', 'nodeCode', 'COLUMN';
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_numbers]') AND name = N'IX_ec_numbers')
CREATE NONCLUSTERED INDEX [IX_ec_numbers] ON [dbo].[ec_numbers] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_numbers]') AND name = N'IX_ec_numbers_1')
CREATE NONCLUSTERED INDEX [IX_ec_numbers_1] ON [dbo].[ec_numbers] 
(
	[nodeCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'ec_numbers', N'COLUMN',N'name'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'the default name or official name of the EC#' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ec_numbers', @level2type=N'COLUMN',@level2name=N'name'
GO
/****** Object:  Table [dbo].[attribute_names]    Script Date: 03/29/2011 20:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[attribute_names]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[attribute_names](
	[attributeId] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_attribute_names] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[attribute_names]') AND name = N'IX_attribute_names')
CREATE UNIQUE NONCLUSTERED INDEX [IX_attribute_names] ON [dbo].[attribute_names] 
(
	[attributeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnotationQualifier]    Script Date: 03/29/2011 20:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AnnotationQualifier]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AnnotationQualifier](
	[id] [smallint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_AnnotationQualifier] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[chromosomes_pathcase]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[chromosomes_pathcase]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[chromosomes_pathcase](
	[id] [uniqueidentifier] NOT NULL,
	[organism_group_id] [uniqueidentifier] NULL,
	[name] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[length] [bigint] NULL,
	[centromere_location] [int] NOT NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[chromosomes]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[chromosomes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[chromosomes](
	[id] [uniqueidentifier] NOT NULL,
	[organism_group_id] [uniqueidentifier] NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[length] [bigint] NULL,
	[centromere_location] [int] NOT NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_chromosomes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[chromosomes]') AND name = N'IX_chromosomes')
CREATE NONCLUSTERED INDEX [IX_chromosomes] ON [dbo].[chromosomes] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[chromosomes]') AND name = N'IX_chromosomes_1')
CREATE NONCLUSTERED INDEX [IX_chromosomes_1] ON [dbo].[chromosomes] 
(
	[organism_group_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[chromosome_bands]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[chromosome_bands]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[chromosome_bands](
	[chromosome_id] [uniqueidentifier] NOT NULL,
	[chromosome_name] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[arm] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[band] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[iscn_start] [int] NULL,
	[iscn_stop] [int] NULL,
	[bp_start] [int] NULL,
	[bp_stop] [int] NULL,
	[stain] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[density] [float] NULL,
	[bases] [bigint] NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[chromosome_bands]') AND name = N'IX_chromosome_bands')
CREATE NONCLUSTERED INDEX [IX_chromosome_bands] ON [dbo].[chromosome_bands] 
(
	[chromosome_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[chromosome_bands]') AND name = N'IX_chromosome_bands_1')
CREATE NONCLUSTERED INDEX [IX_chromosome_bands_1] ON [dbo].[chromosome_bands] 
(
	[chromosome_name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Author]    Script Date: 03/29/2011 20:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Author]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Author](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Surname] [nvarchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[EMail] [nvarchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OrgName] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_AUTHOR] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ec_go_orig]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ec_go_orig]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ec_go_orig](
	[ec_number] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[go_id] [varchar](7) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ec_go]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ec_go]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ec_go](
	[ec_number] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[go_id] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_go]') AND name = N'IX_Ec2gomap1')
CREATE UNIQUE CLUSTERED INDEX [IX_Ec2gomap1] ON [dbo].[ec_go] 
(
	[ec_number] ASC,
	[go_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_go]') AND name = N'IX_Ec2gomap2')
CREATE NONCLUSTERED INDEX [IX_Ec2gomap2] ON [dbo].[ec_go] 
(
	[ec_number] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_go]') AND name = N'IX_Ec2gomap3')
CREATE NONCLUSTERED INDEX [IX_Ec2gomap3] ON [dbo].[ec_go] 
(
	[go_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[external_databases]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[external_databases]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[external_databases](
	[id] [int] NOT NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[fullname] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[url] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[external_database_urls]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[external_database_urls]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[external_database_urls](
	[external_database_id] [int] NOT NULL,
	[type] [varchar](16) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[url_template] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[external_database_links]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[external_database_links]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[external_database_links](
	[local_id] [uniqueidentifier] NOT NULL,
	[external_database_id] [int] NOT NULL,
	[id_in_external_database] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[name_in_external_database] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[go_terms_hierarchy]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[go_terms_hierarchy]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[go_terms_hierarchy](
	[ParentID] [varchar](7) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ChildID] [varchar](7) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Type] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[TermLevel] [int] NULL,
	[OnPathUnderCatalyticActivity] [bit] NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[go_terms]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[go_terms]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[go_terms](
	[ID] [varchar](7) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Name] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[SubtreeHeight] [int] NOT NULL,
	[TotalDescendants] [int] NOT NULL,
 CONSTRAINT [PK_go_terms] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[go_terms]') AND name = N'IX_go_terms')
CREATE UNIQUE NONCLUSTERED INDEX [IX_go_terms] ON [dbo].[go_terms] 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[go_pathway_group_annotation_counts]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[go_pathway_group_annotation_counts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[go_pathway_group_annotation_counts](
	[pathway_group_id] [uniqueidentifier] NOT NULL,
	[go_id] [varchar](7) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[hierarchy_level] [int] NOT NULL,
	[number_annotations] [int] NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[go_pathway_annotation_counts]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[go_pathway_annotation_counts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[go_pathway_annotation_counts](
	[go_id] [varchar](7) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[hierarchy_level] [int] NOT NULL,
	[number_annotations] [int] NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[go_annotation_pathways]    Script Date: 03/29/2011 20:59:08 ******/
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
	[serialized_image_map] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[date_generated] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[MapSbaseGO]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapSbaseGO]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MapSbaseGO](
	[sbaseId] [uniqueidentifier] NOT NULL,
	[goId] [varchar](7) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[qualifierId] [smallint] NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[processes]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[processes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[processes](
	[id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[name] [varchar](800) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[reversible] [bit] NULL,
	[location] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[generic_process_id] [uniqueidentifier] NULL,
 CONSTRAINT [PK_processes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[processes]') AND name = N'IX_processes')
CREATE NONCLUSTERED INDEX [IX_processes] ON [dbo].[processes] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[processes]') AND name = N'IX_processes_2')
CREATE NONCLUSTERED INDEX [IX_processes_2] ON [dbo].[processes] 
(
	[generic_process_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[process_graph_nodes]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[process_graph_nodes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[process_graph_nodes](
	[pathwayId] [uniqueidentifier] NULL,
	[genericProcessId] [uniqueidentifier] NOT NULL,
	[graphNodeId] [uniqueidentifier] NULL
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_graph_nodes]') AND name = N'IX_process_graph_nodes')
CREATE UNIQUE CLUSTERED INDEX [IX_process_graph_nodes] ON [dbo].[process_graph_nodes] 
(
	[pathwayId] ASC,
	[genericProcessId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_graph_nodes]') AND name = N'IX_process_graph_nodes_1')
CREATE NONCLUSTERED INDEX [IX_process_graph_nodes_1] ON [dbo].[process_graph_nodes] 
(
	[graphNodeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[process_entity_roles]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[process_entity_roles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[process_entity_roles](
	[role_id] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](18) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_process_entity_roles] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_entity_roles]') AND name = N'IX_process_entity_roles')
CREATE UNIQUE NONCLUSTERED INDEX [IX_process_entity_roles] ON [dbo].[process_entity_roles] 
(
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pathway_groups]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[pathway_groups]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[pathway_groups](
	[group_id] [uniqueidentifier] NOT NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_groups] PRIMARY KEY CLUSTERED 
(
	[group_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ModelMetadata]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ModelMetadata]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ModelMetadata](
	[Id] [uniqueidentifier] NOT NULL,
	[ModelName] [nvarchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PublicationId] [int] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationDate] [datetime] NULL,
	[Notes] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ModelMetadata] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[organisms]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[organisms]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[organisms](
	[id] [uniqueidentifier] NOT NULL,
	[taxonomy_id] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[cM_unit_length] [int] NOT NULL,
 CONSTRAINT [PK_organisms] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[organism_groups]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[organism_groups]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[organism_groups](
	[id] [uniqueidentifier] NOT NULL,
	[scientific_name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[common_name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[parent_id] [uniqueidentifier] NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[is_organism] [bit] NOT NULL,
	[nodeLabel] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_organism_groups] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[organism_groups]') AND name = N'IX_organism_groups')
CREATE NONCLUSTERED INDEX [IX_organism_groups] ON [dbo].[organism_groups] 
(
	[common_name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[organism_groups]') AND name = N'IX_organism_groups_1')
CREATE NONCLUSTERED INDEX [IX_organism_groups_1] ON [dbo].[organism_groups] 
(
	[parent_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[organism_groups]') AND name = N'IX_organism_groups_2')
CREATE NONCLUSTERED INDEX [IX_organism_groups_2] ON [dbo].[organism_groups] 
(
	[nodeLabel] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[organism_groups]') AND name = N'IX_organism_groups_3')
CREATE NONCLUSTERED INDEX [IX_organism_groups_3] ON [dbo].[organism_groups] 
(
	[is_organism] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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
/****** Object:  Table [dbo].[OLD_external_databases]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OLD_external_databases]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OLD_external_databases](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_external_databases] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[name_types]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[name_types]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[name_types](
	[name_type_id] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_name_types] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[name_types]') AND name = N'IX_name_types')
CREATE UNIQUE NONCLUSTERED INDEX [IX_name_types] ON [dbo].[name_types] 
(
	[name_type_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[molecular_entity_types]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entity_types]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[molecular_entity_types](
	[type_id] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_molecular_entity_types] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entity_types]') AND name = N'IX_molecular_entity_types')
CREATE UNIQUE NONCLUSTERED INDEX [IX_molecular_entity_types] ON [dbo].[molecular_entity_types] 
(
	[type_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[molecular_entity_names]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entity_names]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[molecular_entity_names](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_molecular_entity_names] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entity_names]') AND name = N'IX_molecular_entity_names')
CREATE UNIQUE NONCLUSTERED INDEX [IX_molecular_entity_names] ON [dbo].[molecular_entity_names] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[viewstate_nodes]    Script Date: 03/29/2011 20:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[viewstate_nodes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[viewstate_nodes](
	[viewID] [uniqueidentifier] NULL,
	[level] [int] NULL,
	[openNodeID] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[opennodeType] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[viewState]    Script Date: 03/29/2011 20:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[viewState]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[viewState](
	[viewID] [uniqueidentifier] NOT NULL,
	[openSection] [varchar](32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[organism] [varchar](32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[openNode1ID] [uniqueidentifier] NULL,
	[openNode1Type] [varchar](32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[openNode2ID] [uniqueidentifier] NULL,
	[openNode2Type] [varchar](32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[openNode3ID] [uniqueidentifier] NULL,
	[openNode3Type] [varchar](32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[displayItemID] [uniqueidentifier] NULL,
	[displayItemType] [varchar](32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[viewGraph] [tinyint] NULL,
	[timeStamp] [datetime] NULL,
 CONSTRAINT [PK_viewState] PRIMARY KEY CLUSTERED 
(
	[viewID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rna_types]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rna_types]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[rna_types](
	[rna_type_id] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_rna_types] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[rna_types]') AND name = N'IX_rna_types')
CREATE UNIQUE NONCLUSTERED INDEX [IX_rna_types] ON [dbo].[rna_types] 
(
	[rna_type_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReactionSpeciesRole]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReactionSpeciesRole]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ReactionSpeciesRole](
	[id] [tinyint] NOT NULL,
	[role] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ReactionSpeciesRole] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RuleType]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RuleType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RuleType](
	[type] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[id] [tinyint] NOT NULL,
 CONSTRAINT [PK_RuleType_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Sbase]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sbase]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Sbase](
	[id] [uniqueidentifier] NOT NULL,
	[metaId] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[sboTerm] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[notes] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[annotation] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Sbase] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[pathway_types]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[pathway_types]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[pathway_types](
	[pathway_type_id] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_pathway_types] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_types]') AND name = N'IX_pathway_types')
CREATE UNIQUE NONCLUSTERED INDEX [IX_pathway_types] ON [dbo].[pathway_types] 
(
	[pathway_type_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pathways]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[pathways]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[pathways](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[pathway_type_id] [tinyint] NOT NULL,
	[status] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[layout] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_pathways] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathways]') AND name = N'IX_pathways')
CREATE NONCLUSTERED INDEX [IX_pathways] ON [dbo].[pathways] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathways]') AND name = N'IX_pathways_1')
CREATE NONCLUSTERED INDEX [IX_pathways_1] ON [dbo].[pathways] 
(
	[pathway_type_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoichiometryMath]    Script Date: 03/29/2011 20:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoichiometryMath]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoichiometryMath](
	[id] [uniqueidentifier] NOT NULL,
	[math] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_StoichiometryMath] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[molecular_entities]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entities]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[molecular_entities](
	[id] [uniqueidentifier] NOT NULL,
	[type_id] [tinyint] NOT NULL,
	[name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_molecular_entities] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entities]') AND name = N'IX_molecular_entities')
CREATE NONCLUSTERED INDEX [IX_molecular_entities] ON [dbo].[molecular_entities] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entities]') AND name = N'IX_molecular_entities_1')
CREATE NONCLUSTERED INDEX [IX_molecular_entities_1] ON [dbo].[molecular_entities] 
(
	[type_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[molecular_entities]') AND name = N'IX_molecular_entities_2')
CREATE NONCLUSTERED INDEX [IX_molecular_entities_2] ON [dbo].[molecular_entities] 
(
	[name] ASC,
	[type_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'molecular_entities', N'COLUMN',N'name'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'the default name or offical name of the molecular entity' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'molecular_entities', @level2type=N'COLUMN',@level2name=N'name'
GO
/****** Object:  Table [dbo].[Model]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Model]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Model](
	[id] [uniqueidentifier] NOT NULL,
	[sbmlId] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[sbmlLevel] [tinyint] NULL,
	[sbmlVersion] [tinyint] NULL,
	[dataSourceId] [smallint] NULL,
	[sbmlFile] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[sbmlFileName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Model] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OLD_external_database_links]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OLD_external_database_links]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OLD_external_database_links](
	[local_id] [uniqueidentifier] NOT NULL,
	[external_database_id] [uniqueidentifier] NOT NULL,
	[id_in_external_database] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_external_database_links] PRIMARY KEY CLUSTERED 
(
	[local_id] ASC,
	[external_database_id] ASC,
	[id_in_external_database] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[KineticLaw]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KineticLaw]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[KineticLaw](
	[id] [uniqueidentifier] NOT NULL,
	[math] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_KineticLaw] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GONodeCodes]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GONodeCodes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[GONodeCodes](
	[goid] [varchar](7) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[nodeCode] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_GONodeCodes] PRIMARY KEY CLUSTERED 
(
	[goid] ASC,
	[nodeCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GONodeCodes]') AND name = N'IX_GONodeCodes')
CREATE NONCLUSTERED INDEX [IX_GONodeCodes] ON [dbo].[GONodeCodes] 
(
	[goid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventTrigger]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventTrigger]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EventTrigger](
	[id] [uniqueidentifier] NOT NULL,
	[math] [xml] NOT NULL,
 CONSTRAINT [PK_EventTrigger] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[EventDelay]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventDelay]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EventDelay](
	[id] [uniqueidentifier] NOT NULL,
	[math] [xml] NOT NULL,
 CONSTRAINT [PK_EventDelay] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[genes]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[genes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[genes](
	[id] [uniqueidentifier] NOT NULL,
	[organism_group_id] [uniqueidentifier] NOT NULL,
	[chromosome_id] [uniqueidentifier] NULL,
	[homologue_group_id] [uniqueidentifier] NOT NULL,
	[raw_address] [varchar](8000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[cytogenic_address] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[genetic_address] [bigint] NULL,
	[relative_address] [bigint] NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_genes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY],
 CONSTRAINT [IX_genes] UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[genes]') AND name = N'IX_genes_1')
CREATE NONCLUSTERED INDEX [IX_genes_1] ON [dbo].[genes] 
(
	[organism_group_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[genes]') AND name = N'IX_genes_2')
CREATE NONCLUSTERED INDEX [IX_genes_2] ON [dbo].[genes] 
(
	[homologue_group_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[genes]') AND name = N'IX_genes_3')
CREATE NONCLUSTERED INDEX [IX_genes_3] ON [dbo].[genes] 
(
	[organism_group_id] ASC,
	[chromosome_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[genes]') AND name = N'IX_genes_4')
CREATE NONCLUSTERED INDEX [IX_genes_4] ON [dbo].[genes] 
(
	[chromosome_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'genes', N'COLUMN',N'homologue_group_id'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'the homolog gene group. it''s also used to identify the homolog proteins and processes.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'genes', @level2type=N'COLUMN',@level2name=N'homologue_group_id'
GO
/****** Object:  Table [dbo].[DesignedBy]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DesignedBy]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DesignedBy](
	[Id] [uniqueidentifier] NOT NULL,
	[ModelMetadataId] [uniqueidentifier] NOT NULL,
	[AuthorId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AuthorOf] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[attribute_values]    Script Date: 03/29/2011 20:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[attribute_values]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[attribute_values](
	[attributeId] [int] NOT NULL,
	[itemId] [uniqueidentifier] NOT NULL,
	[value] [varchar](800) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[textValue] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_attribute_values] PRIMARY KEY NONCLUSTERED 
(
	[attributeId] ASC,
	[itemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[attribute_values]') AND name = N'IX_attribute_values')
CREATE CLUSTERED INDEX [IX_attribute_values] ON [dbo].[attribute_values] 
(
	[attributeId] ASC,
	[itemId] ASC,
	[value] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[attribute_values]') AND name = N'attribute_value_lookup')
CREATE NONCLUSTERED INDEX [attribute_value_lookup] ON [dbo].[attribute_values] 
(
	[attributeId] ASC,
	[value] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[attribute_values]') AND name = N'IX_attribute_values_1')
CREATE UNIQUE NONCLUSTERED INDEX [IX_attribute_values_1] ON [dbo].[attribute_values] 
(
	[attributeId] ASC,
	[itemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ec_number_name_lookups]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ec_number_name_lookups](
	[ec_number] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[name_id] [uniqueidentifier] NOT NULL,
	[name_type_id] [tinyint] NOT NULL,
 CONSTRAINT [PK_ec_number_name_lookups] PRIMARY KEY CLUSTERED 
(
	[ec_number] ASC,
	[name_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]') AND name = N'IX_ec_number_name_lookups')
CREATE NONCLUSTERED INDEX [IX_ec_number_name_lookups] ON [dbo].[ec_number_name_lookups] 
(
	[ec_number] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]') AND name = N'IX_ec_number_name_lookups_1')
CREATE NONCLUSTERED INDEX [IX_ec_number_name_lookups_1] ON [dbo].[ec_number_name_lookups] 
(
	[name_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]') AND name = N'IX_ec_number_name_lookups_2')
CREATE NONCLUSTERED INDEX [IX_ec_number_name_lookups_2] ON [dbo].[ec_number_name_lookups] 
(
	[ec_number] ASC,
	[name_type_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Constraint]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Constraint]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Constraint](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NOT NULL,
	[math] [xml] NOT NULL,
	[message] [xml] NULL,
 CONSTRAINT [PK_Constraint] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[basic_molecules]    Script Date: 03/29/2011 20:59:08 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Event]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Event]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Event](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NOT NULL,
	[sbmlId] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[eventTriggerId] [uniqueidentifier] NOT NULL,
	[eventDelayId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[entity_name_lookups]    Script Date: 03/29/2011 20:59:08 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]') AND name = N'IX_entity_name_lookups')
CREATE NONCLUSTERED INDEX [IX_entity_name_lookups] ON [dbo].[entity_name_lookups] 
(
	[entity_id] ASC,
	[name_type_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]') AND name = N'IX_entity_name_lookups_1')
CREATE NONCLUSTERED INDEX [IX_entity_name_lookups_1] ON [dbo].[entity_name_lookups] 
(
	[name_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]') AND name = N'IX_entity_name_lookups_2')
CREATE NONCLUSTERED INDEX [IX_entity_name_lookups_2] ON [dbo].[entity_name_lookups] 
(
	[entity_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[gene_products]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[gene_products]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[gene_products](
	[id] [uniqueidentifier] NOT NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_gene_products] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY],
 CONSTRAINT [IX_gene_products] UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  UserDefinedFunction [pathcase].[Get_molecular_entity_type]    Script Date: 03/29/2011 20:59:11 ******/
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
/****** Object:  Table [dbo].[FunctionDefinition]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FunctionDefinition]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FunctionDefinition](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NOT NULL,
	[sbmlId] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[name] [varchar](MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[lambda] [xml] NOT NULL,
 CONSTRAINT [PK_FunctionDefinition] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CompartmentType]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CompartmentType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CompartmentType](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NOT NULL,
	[sbmlId] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_CompartmentType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InitialAssignment]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InitialAssignment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InitialAssignment](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NOT NULL,
	[symbol] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[math] [xml] NOT NULL,
 CONSTRAINT [PK_InitialAssignment] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ModelLayout]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ModelLayout]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ModelLayout](
	[id] [uniqueidentifier] NOT NULL,
	[layout] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ModelLayout] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ModelOrganism]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ModelOrganism]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ModelOrganism](
	[modelId] [uniqueidentifier] NOT NULL,
	[organismGroupId] [uniqueidentifier] NULL,
	[NCBITaxonomyId] [int] NOT NULL,
	[qualifierId] [smallint] NOT NULL,
 CONSTRAINT [PK_ModelOrganism] PRIMARY KEY CLUSTERED 
(
	[modelId] ASC,
	[NCBITaxonomyId] ASC,
	[qualifierId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[process_entities]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[process_entities]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[process_entities](
	[process_id] [uniqueidentifier] NOT NULL,
	[entity_id] [uniqueidentifier] NOT NULL,
	[role_id] [tinyint] NOT NULL,
	[quantity] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_process_entities] PRIMARY KEY CLUSTERED 
(
	[process_id] ASC,
	[entity_id] ASC,
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_entities]') AND name = N'IX_process_entities')
CREATE NONCLUSTERED INDEX [IX_process_entities] ON [dbo].[process_entities] 
(
	[entity_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_entities]') AND name = N'IX_process_entities_1')
CREATE NONCLUSTERED INDEX [IX_process_entities_1] ON [dbo].[process_entities] 
(
	[entity_id] ASC,
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_entities]') AND name = N'IX_process_entities_2')
CREATE NONCLUSTERED INDEX [IX_process_entities_2] ON [dbo].[process_entities] 
(
	[process_id] ASC,
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_entities]') AND name = N'IX_process_entities_3')
CREATE NONCLUSTERED INDEX [IX_process_entities_3] ON [dbo].[process_entities] 
(
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[process_entities]') AND name = N'IX_process_id')
CREATE NONCLUSTERED INDEX [IX_process_id] ON [dbo].[process_entities] 
(
	[process_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[view_go_terms_nodecodes]    Script Date: 03/29/2011 20:59:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[view_go_terms_nodecodes]'))
EXEC dbo.sp_executesql @statement = N'



CREATE VIEW [dbo].[view_go_terms_nodecodes] with schemabinding 
AS
SELECT     dbo.go_terms.ID,dbo.go_terms.Name,dbo.go_terms.SubtreeHeight,dbo.go_terms.TotalDescendants,
			dbo.GONodeCodes.nodeCode
FROM         dbo.go_terms INNER JOIN
                      dbo.GONodeCodes ON dbo.go_terms.ID = dbo.GONodeCodes.goid




'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPane1' , N'SCHEMA',N'dbo', N'VIEW',N'view_go_terms_nodecodes', NULL,NULL))
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
         Configuration = "(H (4 [30] 2 [40] 3))"
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
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "go_terms (dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 123
               Right = 213
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "GONodeCodes (dbo)"
            Begin Extent = 
               Top = 6
               Left = 251
               Bottom = 93
               Right = 411
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'view_go_terms_nodecodes'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPaneCount' , N'SCHEMA',N'dbo', N'VIEW',N'view_go_terms_nodecodes', NULL,NULL))
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'view_go_terms_nodecodes'
GO
/****** Object:  Table [dbo].[UnitDefinition]    Script Date: 03/29/2011 20:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UnitDefinition]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UnitDefinition](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NULL,
	[sbmlId] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[isBaseUnit] [bit] NOT NULL,
 CONSTRAINT [PK_UnitDefinition] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MapModelsPathways]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapModelsPathways]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MapModelsPathways](
	[modelId] [uniqueidentifier] NOT NULL,
	[pathwayId] [uniqueidentifier] NOT NULL,
	[qualifierId] [smallint] NOT NULL,
	[organismGroupId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_MapModelsPathways] PRIMARY KEY CLUSTERED 
(
	[modelId] ASC,
	[pathwayId] ASC,
	[qualifierId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[SpeciesType]    Script Date: 03/29/2011 20:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SpeciesType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SpeciesType](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NOT NULL,
	[sbmlId] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_SpeciesType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Rule]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rule]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Rule](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NOT NULL,
	[variable] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[math] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ruleTypeId] [tinyint] NOT NULL,
 CONSTRAINT [PK_Rule] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[pathway_to_pathway_groups]    Script Date: 03/29/2011 20:59:09 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]') AND name = N'IX_pathway_id')
CREATE NONCLUSTERED INDEX [IX_pathway_id] ON [dbo].[pathway_to_pathway_groups] 
(
	[pathway_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]') AND name = N'IX_pathway_to_pathway_groups')
CREATE NONCLUSTERED INDEX [IX_pathway_to_pathway_groups] ON [dbo].[pathway_to_pathway_groups] 
(
	[group_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pathway_processes]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[pathway_processes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[pathway_processes](
	[pathway_id] [uniqueidentifier] NOT NULL,
	[process_id] [uniqueidentifier] NOT NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_pathway_processes] PRIMARY KEY CLUSTERED 
(
	[pathway_id] ASC,
	[process_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_processes]') AND name = N'IX_pathway_id')
CREATE NONCLUSTERED INDEX [IX_pathway_id] ON [dbo].[pathway_processes] 
(
	[pathway_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_processes]') AND name = N'IX_pathway_processes')
CREATE NONCLUSTERED INDEX [IX_pathway_processes] ON [dbo].[pathway_processes] 
(
	[process_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pathway_links]    Script Date: 03/29/2011 20:59:09 ******/
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
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_pathway_links] PRIMARY KEY CLUSTERED 
(
	[pathway_id_1] ASC,
	[pathway_id_2] ASC,
	[entity_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_links]') AND name = N'IX_pathway_links')
CREATE NONCLUSTERED INDEX [IX_pathway_links] ON [dbo].[pathway_links] 
(
	[pathway_id_1] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_links]') AND name = N'IX_pathway_links_1')
CREATE NONCLUSTERED INDEX [IX_pathway_links_1] ON [dbo].[pathway_links] 
(
	[pathway_id_2] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[pathway_links]') AND name = N'IX_pathway_links_2')
CREATE NONCLUSTERED INDEX [IX_pathway_links_2] ON [dbo].[pathway_links] 
(
	[entity_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reaction]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reaction]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Reaction](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NOT NULL,
	[sbmlId] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[name] [varchar](MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[reversible] [bit] NOT NULL,
	[fast] [bit] NOT NULL,
	[kineticLawId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Reaction] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[SaveLayout]    Script Date: 03/29/2011 20:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveLayout]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'




CREATE PROCEDURE [dbo].[SaveLayout]
@id uniqueidentifier,
@layout nvarchar(MAX)
AS
BEGIN
SET NOCOUNT ON
UPDATE    ModelLayout
SET              layout = @layout
WHERE     id = @id

IF @@ROWCOUNT = 0 
BEGIN
INSERT INTO ModelLayout VALUES(@id,@layout);

END
RETURN 
END





' 
END
GO
/****** Object:  Table [dbo].[proteins]    Script Date: 03/29/2011 20:59:09 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[rnas]    Script Date: 03/29/2011 20:59:09 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Unit]    Script Date: 03/29/2011 20:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Unit]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Unit](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NULL,
	[kind] [uniqueidentifier] NOT NULL,
	[exponent] [int] NOT NULL,
	[scale] [int] NOT NULL,
	[multiplier] [float] NOT NULL,
 CONSTRAINT [PK_Unit] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  StoredProcedure [dbo].[PopulateProcessGraphNodes]    Script Date: 03/29/2011 20:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PopulateProcessGraphNodes]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


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

	
end
' 
END
GO
/****** Object:  StoredProcedure [dbo].[PopulatePathwayEntities]    Script Date: 03/29/2011 20:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PopulatePathwayEntities]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
-- =============================================
-- Author:		Ali Cakmak
-- Create date: Feb 7th, 2008
-- Description:	Precomputes the content of Pathway Entities table.
-- =============================================
CREATE PROCEDURE [dbo].[PopulatePathwayEntities]
AS
BEGIN

delete from entity_graph_nodes

insert into entity_graph_nodes
select distinct pp.pathway_id, pe.entity_id, null
from pathway_processes pp, process_entities pe, process_entity_roles per
where pp.process_id = pe.process_id
and pe.role_id = per.role_id
and (per.name = ''substrate'' OR per.name=''product'')

declare @sourcePathway uniqueidentifier
declare @destinationPathway uniqueidentifier
declare @linkingEntity uniqueidentifier
declare @nodeId1 uniqueidentifier
declare @nodeId2 uniqueidentifier

declare crs cursor for
select pathway_id_1, pathway_id_2, entity_id 
from pathway_links

open crs

fetch crs into @sourcePathway, @destinationPathway, @linkingEntity

WHILE @@FETCH_STATUS = 0
begin

select @nodeId1=graphNodeId
from entity_graph_nodes
where pathwayId=@sourcePathway
and entityId=@linkingEntity

select @nodeId2=graphNodeId
from entity_graph_nodes
where pathwayId=@sourcePathway
and entityId=@linkingEntity

if(@nodeId1 is null)
begin
	if(@nodeId2 is null)
	begin	
		select @nodeId1=newId()

		update entity_graph_nodes
		set graphNodeId=@nodeId1
		where (pathwayId=@sourcePathway or pathwayId=@destinationPathway)
		and entityId=@linkingEntity
	end
	else -- nodeId2 is not null
	begin
		update entity_graph_nodes
		set graphNodeId=@nodeId2
		where pathwayId=@sourcePathway
		and entityId=@linkingEntity
	end
end
else -- nodeId1 is not null
begin
	if(@nodeId2 is null)
	begin	
		update entity_graph_nodes
		set graphNodeId=@nodeId1
		where pathwayId=@destinationPathway
		and entityId=@linkingEntity
	end
	else -- nodeId2 is not null --> merge all
	begin
		select @nodeId1=newId()

		update entity_graph_nodes
		set graphNodeId=@nodeId1
		where pathwayId in (
			(select pathway_id_1 from pathway_links where (pathway_id_2=@sourcePathway or pathway_id_2=@destinationPathway) and entity_id=@linkingEntity)
			union
			(select pathway_id_2 from pathway_links where (pathway_id_1=@sourcePathway or pathway_id_1=@destinationPathway) and entity_id=@linkingEntity)		
        )
		and entityId=@linkingEntity
	end
end

fetch crs into @sourcePathway, @destinationPathway, @linkingEntity

end

-- handles processes that are not members of any pathway --- 
---- each such process is considered to be a single process pathway
---- disconnected from the metabolic network
insert into entity_graph_nodes
select distinct pe.process_id, pe.entity_id, null
from process_entities pe, process_entity_roles per
where pe.role_id = per.role_id
and (per.name = ''substrate'' OR per.name=''product'')
and pe.process_id not in
(
	select process_id from pathway_processes
)

update entity_graph_nodes
set graphNodeId=newId()
where graphNodeId is null
end
' 
END
GO
/****** Object:  Table [dbo].[Parameter]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Parameter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Parameter](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NULL,
	[reactionId] [uniqueidentifier] NULL,
	[sbmlId] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[value] [float] NULL,
	[unitsId] [uniqueidentifier] NULL,
	[constant] [bit] NOT NULL,
 CONSTRAINT [PK_Parameter] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MapReactionsProcessEntities]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapReactionsProcessEntities]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MapReactionsProcessEntities](
	[reactionId] [uniqueidentifier] NOT NULL,
	[processId] [uniqueidentifier] NOT NULL,
	[qualifierId] [smallint] NOT NULL,
 CONSTRAINT [PK_MapReactionsProcessEntities_1] PRIMARY KEY CLUSTERED 
(
	[reactionId] ASC,
	[processId] ASC,
	[qualifierId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MapReactionsProcessEntities]') AND name = N'IX_MapReactionsProcessEntities')
CREATE NONCLUSTERED INDEX [IX_MapReactionsProcessEntities] ON [dbo].[MapReactionsProcessEntities] 
(
	[processId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MapReactionECNumber]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapReactionECNumber]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MapReactionECNumber](
	[reactionId] [uniqueidentifier] NOT NULL,
	[ecNumber] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[qualifierId] [smallint] NOT NULL,
 CONSTRAINT [PK_MapReactionECNumber] PRIMARY KEY CLUSTERED 
(
	[reactionId] ASC,
	[ecNumber] ASC,
	[qualifierId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[gene_encodings]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[gene_encodings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[gene_encodings](
	[gene_id] [uniqueidentifier] NOT NULL,
	[gene_product_id] [uniqueidentifier] NOT NULL,
	[notes] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_gene_and_gene_products] PRIMARY KEY CLUSTERED 
(
	[gene_id] ASC,
	[gene_product_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[gene_encodings]') AND name = N'IX_gene_encodings')
CREATE NONCLUSTERED INDEX [IX_gene_encodings] ON [dbo].[gene_encodings] 
(
	[gene_product_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[gene_encodings]') AND name = N'IX_gene_encodings_1')
CREATE NONCLUSTERED INDEX [IX_gene_encodings_1] ON [dbo].[gene_encodings] 
(
	[gene_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventAssignment]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventAssignment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EventAssignment](
	[id] [uniqueidentifier] NOT NULL,
	[eventId] [uniqueidentifier] NOT NULL,
	[variable] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[math] [xml] NOT NULL,
 CONSTRAINT [PK_EventAssignment] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[catalyzes]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[catalyzes](
	[process_id] [uniqueidentifier] NOT NULL,
	[organism_group_id] [uniqueidentifier] NULL,
	[gene_product_id] [uniqueidentifier] NULL,
	[ec_number] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [IX_catalyzes_all] UNIQUE NONCLUSTERED 
(
	[gene_product_id] ASC,
	[process_id] ASC,
	[ec_number] ASC,
	[organism_group_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_ec')
CREATE NONCLUSTERED INDEX [IX_catalyzes_ec] ON [dbo].[catalyzes] 
(
	[ec_number] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_except_ec')
CREATE NONCLUSTERED INDEX [IX_catalyzes_except_ec] ON [dbo].[catalyzes] 
(
	[process_id] ASC,
	[organism_group_id] ASC,
	[gene_product_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_gene')
CREATE NONCLUSTERED INDEX [IX_catalyzes_gene] ON [dbo].[catalyzes] 
(
	[gene_product_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_gene_organism_group')
CREATE NONCLUSTERED INDEX [IX_catalyzes_gene_organism_group] ON [dbo].[catalyzes] 
(
	[organism_group_id] ASC,
	[gene_product_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_gene_process')
CREATE NONCLUSTERED INDEX [IX_catalyzes_gene_process] ON [dbo].[catalyzes] 
(
	[process_id] ASC,
	[gene_product_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_organism_group')
CREATE NONCLUSTERED INDEX [IX_catalyzes_organism_group] ON [dbo].[catalyzes] 
(
	[organism_group_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_process')
CREATE NONCLUSTERED INDEX [IX_catalyzes_process] ON [dbo].[catalyzes] 
(
	[process_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[catalyzes]') AND name = N'IX_catalyzes_process_organism_group')
CREATE NONCLUSTERED INDEX [IX_catalyzes_process_organism_group] ON [dbo].[catalyzes] 
(
	[process_id] ASC,
	[organism_group_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Compartment]    Script Date: 03/29/2011 20:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Compartment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Compartment](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NOT NULL,
	[sbmlId] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[compartmentTypeId] [uniqueidentifier] NULL,
	[spatialDimensions] [tinyint] NOT NULL,
	[size] [float] NULL,
	[unitsId] [uniqueidentifier] NULL,
	[compartmentClassId] [uniqueidentifier] NULL,
	[outside] [uniqueidentifier] NULL,
	[constant] [bit] NOT NULL,
 CONSTRAINT [PK_Compartment] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[common_molecules]    Script Date: 03/29/2011 20:59:08 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  StoredProcedure [pathcase].[ComputePathwayLinks2]    Script Date: 03/29/2011 20:59:05 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[pathcase].[ComputePathwayLinks2]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROC [pathcase].[ComputePathwayLinks2]
--CREATE PROCEDURE [pathcase].[ComputePathwayLinks2]
AS
delete from pathway_links
insert into pathway_links (pathway_id_1, pathway_id_2, entity_id)
select distinct t1.pathway_id pathway_id_1,t2.pathway_id pathway_id_2,t1.entity_id
from 
	(
	select distinct entity_id,pathway_id
	from process_entities pe, pathway_processes pp, process_entity_roles per, processes p
	where ((pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in (''product'') and p.reversible=0)
        OR (pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in (''product'', ''substrate'') and p.reversible=1))
  		and entity_id not in (select id from common_molecules)
		and p.id = pe.process_id
	) t1,
	(
	select distinct entity_id,pathway_id
	from process_entities pe, pathway_processes pp, process_entity_roles per, processes p
	where ((pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in (''substrate'') and p.reversible=0)
        OR (pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in (''product'', ''substrate'') and p.reversible=1))
  		and entity_id not in (select id from common_molecules)
		and p.id = pe.process_id
	) t2
where t1.entity_id = t2.entity_id and t1.pathway_id != t2.pathway_id
--end
' 
END
GO
/****** Object:  View [dbo].[View_Process_Entities]    Script Date: 03/29/2011 20:59:12 ******/
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
/****** Object:  Table [dbo].[UnitComposition]    Script Date: 03/29/2011 20:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UnitComposition]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UnitComposition](
	[unitDefinitionId] [uniqueidentifier] NOT NULL,
	[unitId] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[UnitComposition]') AND name = N'IX_UnitComposition')
CREATE NONCLUSTERED INDEX [IX_UnitComposition] ON [dbo].[UnitComposition] 
(
	[unitDefinitionId] ASC,
	[unitId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Species]    Script Date: 03/29/2011 20:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Species]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Species](
	[id] [uniqueidentifier] NOT NULL,
	[modelId] [uniqueidentifier] NOT NULL,
	[sbmlId] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[speciesTypeId] [uniqueidentifier] NULL,
	[compartmentId] [uniqueidentifier] NOT NULL,
	[initialAmount] [float] NULL,
	[initialConcentration] [float] NULL,
	[substanceUnitsId] [uniqueidentifier] NULL,
	[hasOnlySubstanceUnits] [bit] NOT NULL,
	[boundaryCondition] [bit] NOT NULL,
	[charge] [int] NULL,
	[constant] [bit] NOT NULL,
 CONSTRAINT [PK_Species] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[SetLinkingEntitiesInPathwayLinks]    Script Date: 03/29/2011 20:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SetLinkingEntitiesInPathwayLinks]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
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
	where ((pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in (''product'') and p.reversible=0)
        OR (pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in (''product'', ''substrate'') and p.reversible=1))
  		and entity_id not in (select id from common_molecules)
		and p.id = pe.process_id
		and pp.pathway_id = @sourcePathway 
	) t1,
	(
	select distinct entity_id
	from process_entities pe, pathway_processes pp, process_entity_roles per, processes p
	where ((pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in (''substrate'') and p.reversible=0)
        OR (pe.process_id = pp.process_id and pe.role_id=per.role_id and per.name in (''product'', ''substrate'') and p.reversible=1))
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
end

--USE [Pathcase_KEGG_NEW]
--GO



' 
END
GO
/****** Object:  Table [dbo].[ReactionSpecies]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReactionSpecies]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ReactionSpecies](
	[id] [uniqueidentifier] NOT NULL,
	[reactionId] [uniqueidentifier] NOT NULL,
	[speciesId] [uniqueidentifier] NOT NULL,
	[roleId] [tinyint] NOT NULL,
	[stoichiometry] [float] NOT NULL,
	[stoichiometryMathId] [uniqueidentifier] NULL,
	[sbmlId] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[name] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ReactionSpecies] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MapSpeciesMolecularEntities]    Script Date: 03/29/2011 20:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapSpeciesMolecularEntities]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MapSpeciesMolecularEntities](
	[speciesId] [uniqueidentifier] NOT NULL,
	[qualifierId] [smallint] NOT NULL,
	[molecularEntityId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_MapSpeciesMolecularEntities_1] PRIMARY KEY CLUSTERED 
(
	[speciesId] ASC,
	[qualifierId] ASC,
	[molecularEntityId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Default [DF_chromosomes_centromere_location]    Script Date: 03/29/2011 20:59:08 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_chromosomes_centromere_location]') AND parent_object_id = OBJECT_ID(N'[dbo].[chromosomes]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_chromosomes_centromere_location]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[chromosomes] ADD  CONSTRAINT [DF_chromosomes_centromere_location]  DEFAULT ((0)) FOR [centromere_location]
END


End
GO
/****** Object:  Default [DF_Compartment_spatialDimensions]    Script Date: 03/29/2011 20:59:08 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Compartment_spatialDimensions]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compartment]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Compartment_spatialDimensions]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Compartment] ADD  CONSTRAINT [DF_Compartment_spatialDimensions]  DEFAULT ((3)) FOR [spatialDimensions]
END


End
GO
/****** Object:  Default [DF_Compartment_constant]    Script Date: 03/29/2011 20:59:08 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Compartment_constant]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compartment]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Compartment_constant]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Compartment] ADD  CONSTRAINT [DF_Compartment_constant]  DEFAULT ((1)) FOR [constant]
END


End
GO
/****** Object:  Default [DF_organism_groups_id]    Script Date: 03/29/2011 20:59:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_organism_groups_id]') AND parent_object_id = OBJECT_ID(N'[dbo].[organism_groups]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_organism_groups_id]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[organism_groups] ADD  CONSTRAINT [DF_organism_groups_id]  DEFAULT (newid()) FOR [id]
END


End
GO
/****** Object:  Default [DF_organism_groups_is_organism]    Script Date: 03/29/2011 20:59:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_organism_groups_is_organism]') AND parent_object_id = OBJECT_ID(N'[dbo].[organism_groups]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_organism_groups_is_organism]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[organism_groups] ADD  CONSTRAINT [DF_organism_groups_is_organism]  DEFAULT ((0)) FOR [is_organism]
END


End
GO
/****** Object:  Default [DF_organisms_cM_unit_length]    Script Date: 03/29/2011 20:59:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_organisms_cM_unit_length]') AND parent_object_id = OBJECT_ID(N'[dbo].[organisms]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_organisms_cM_unit_length]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[organisms] ADD  CONSTRAINT [DF_organisms_cM_unit_length]  DEFAULT ((1000000)) FOR [cM_unit_length]
END


End
GO
/****** Object:  Default [DF_Parameter_constant]    Script Date: 03/29/2011 20:59:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Parameter_constant]') AND parent_object_id = OBJECT_ID(N'[dbo].[Parameter]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Parameter_constant]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Parameter] ADD  CONSTRAINT [DF_Parameter_constant]  DEFAULT ((1)) FOR [constant]
END


End
GO
/****** Object:  Default [DF_groups_group_id]    Script Date: 03/29/2011 20:59:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_groups_group_id]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_groups]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_groups_group_id]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[pathway_groups] ADD  CONSTRAINT [DF_groups_group_id]  DEFAULT (newid()) FOR [group_id]
END


End
GO
/****** Object:  Default [DF_process_entities_quantity]    Script Date: 03/29/2011 20:59:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_process_entities_quantity]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_process_entities_quantity]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[process_entities] ADD  CONSTRAINT [DF_process_entities_quantity]  DEFAULT ((1)) FOR [quantity]
END


End
GO
/****** Object:  Default [DF_processes_id]    Script Date: 03/29/2011 20:59:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_processes_id]') AND parent_object_id = OBJECT_ID(N'[dbo].[processes]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_processes_id]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[processes] ADD  CONSTRAINT [DF_processes_id]  DEFAULT (newid()) FOR [id]
END


End
GO
/****** Object:  Default [DF_processes_reversible]    Script Date: 03/29/2011 20:59:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_processes_reversible]') AND parent_object_id = OBJECT_ID(N'[dbo].[processes]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_processes_reversible]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[processes] ADD  CONSTRAINT [DF_processes_reversible]  DEFAULT ((0)) FOR [reversible]
END


End
GO
/****** Object:  Default [DF_Reaction_reversible]    Script Date: 03/29/2011 20:59:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Reaction_reversible]') AND parent_object_id = OBJECT_ID(N'[dbo].[Reaction]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Reaction_reversible]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Reaction] ADD  CONSTRAINT [DF_Reaction_reversible]  DEFAULT ((1)) FOR [reversible]
END


End
GO
/****** Object:  Default [DF_Reaction_fast]    Script Date: 03/29/2011 20:59:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Reaction_fast]') AND parent_object_id = OBJECT_ID(N'[dbo].[Reaction]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Reaction_fast]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Reaction] ADD  CONSTRAINT [DF_Reaction_fast]  DEFAULT ((0)) FOR [fast]
END


End
GO
/****** Object:  Default [DF_ReactionSpecies_stoichiometry]    Script Date: 03/29/2011 20:59:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ReactionSpecies_stoichiometry]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReactionSpecies]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ReactionSpecies_stoichiometry]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ReactionSpecies] ADD  CONSTRAINT [DF_ReactionSpecies_stoichiometry]  DEFAULT ((1)) FOR [stoichiometry]
END


End
GO
/****** Object:  Default [DF_Species_hasOnlySubstanceUnits]    Script Date: 03/29/2011 20:59:10 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Species_hasOnlySubstanceUnits]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Species_hasOnlySubstanceUnits]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Species] ADD  CONSTRAINT [DF_Species_hasOnlySubstanceUnits]  DEFAULT ((0)) FOR [hasOnlySubstanceUnits]
END


End
GO
/****** Object:  Default [DF_Species_boundaryCondition]    Script Date: 03/29/2011 20:59:10 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Species_boundaryCondition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Species_boundaryCondition]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Species] ADD  CONSTRAINT [DF_Species_boundaryCondition]  DEFAULT ((0)) FOR [boundaryCondition]
END


End
GO
/****** Object:  Default [DF_Species_constant]    Script Date: 03/29/2011 20:59:10 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Species_constant]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Species_constant]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Species] ADD  CONSTRAINT [DF_Species_constant]  DEFAULT ((0)) FOR [constant]
END


End
GO
/****** Object:  Default [DF_Unit_exponent]    Script Date: 03/29/2011 20:59:10 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Unit_exponent]') AND parent_object_id = OBJECT_ID(N'[dbo].[Unit]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Unit_exponent]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Unit] ADD  CONSTRAINT [DF_Unit_exponent]  DEFAULT ((1)) FOR [exponent]
END


End
GO
/****** Object:  Default [DF_Unit_scale]    Script Date: 03/29/2011 20:59:10 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Unit_scale]') AND parent_object_id = OBJECT_ID(N'[dbo].[Unit]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Unit_scale]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Unit] ADD  CONSTRAINT [DF_Unit_scale]  DEFAULT ((0)) FOR [scale]
END


End
GO
/****** Object:  Default [DF_Unit_multiplier]    Script Date: 03/29/2011 20:59:10 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Unit_multiplier]') AND parent_object_id = OBJECT_ID(N'[dbo].[Unit]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Unit_multiplier]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Unit] ADD  CONSTRAINT [DF_Unit_multiplier]  DEFAULT ((1)) FOR [multiplier]
END


End
GO
/****** Object:  Default [DF_UnitDefinition_isBaseUnit]    Script Date: 03/29/2011 20:59:10 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_UnitDefinition_isBaseUnit]') AND parent_object_id = OBJECT_ID(N'[dbo].[UnitDefinition]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_UnitDefinition_isBaseUnit]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[UnitDefinition] ADD  CONSTRAINT [DF_UnitDefinition_isBaseUnit]  DEFAULT ((0)) FOR [isBaseUnit]
END


End
GO
/****** Object:  Check [CK_ec_numbers]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_numbers]'))
ALTER TABLE [dbo].[ec_numbers]  WITH CHECK ADD  CONSTRAINT [CK_ec_numbers] CHECK  (([ec_number] like '[1-6].%.%.%'))
GO
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_numbers]'))
ALTER TABLE [dbo].[ec_numbers] CHECK CONSTRAINT [CK_ec_numbers]
GO
/****** Object:  Check [CK_organism_groups]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[organism_groups]'))
ALTER TABLE [dbo].[organism_groups]  WITH CHECK ADD  CONSTRAINT [CK_organism_groups] CHECK  (([common_name] IS NOT NULL OR [scientific_name] IS NOT NULL))
GO
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[organism_groups]'))
ALTER TABLE [dbo].[organism_groups] CHECK CONSTRAINT [CK_organism_groups]
GO
/****** Object:  Check [CK_process_entities_quantity]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_process_entities_quantity]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH CHECK ADD  CONSTRAINT [CK_process_entities_quantity] CHECK  (([quantity]>=(1)))
GO
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_process_entities_quantity]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities] CHECK CONSTRAINT [CK_process_entities_quantity]
GO
/****** Object:  ForeignKey [FK_attribute_values_attribute_names]    Script Date: 03/29/2011 20:59:07 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_attribute_values_attribute_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[attribute_values]'))
ALTER TABLE [dbo].[attribute_values]  WITH NOCHECK ADD  CONSTRAINT [FK_attribute_values_attribute_names] FOREIGN KEY([attributeId])
REFERENCES [dbo].[attribute_names] ([attributeId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_attribute_values_attribute_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[attribute_values]'))
ALTER TABLE [dbo].[attribute_values] CHECK CONSTRAINT [FK_attribute_values_attribute_names]
GO
/****** Object:  ForeignKey [FK_basic_molecules_molecular_entities]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_basic_molecules_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[basic_molecules]'))
ALTER TABLE [dbo].[basic_molecules]  WITH CHECK ADD  CONSTRAINT [FK_basic_molecules_molecular_entities] FOREIGN KEY([id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_basic_molecules_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[basic_molecules]'))
ALTER TABLE [dbo].[basic_molecules] CHECK CONSTRAINT [FK_basic_molecules_molecular_entities]
GO
/****** Object:  ForeignKey [FK_catalyzes_ec_numbers]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes]  WITH CHECK ADD  CONSTRAINT [FK_catalyzes_ec_numbers] FOREIGN KEY([ec_number])
REFERENCES [dbo].[ec_numbers] ([ec_number])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes] CHECK CONSTRAINT [FK_catalyzes_ec_numbers]
GO
/****** Object:  ForeignKey [FK_catalyzes_gene_products]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes]  WITH CHECK ADD  CONSTRAINT [FK_catalyzes_gene_products] FOREIGN KEY([gene_product_id])
REFERENCES [dbo].[gene_products] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes] CHECK CONSTRAINT [FK_catalyzes_gene_products]
GO
/****** Object:  ForeignKey [FK_catalyzes_organism_groups]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes]  WITH CHECK ADD  CONSTRAINT [FK_catalyzes_organism_groups] FOREIGN KEY([organism_group_id])
REFERENCES [dbo].[organism_groups] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes] CHECK CONSTRAINT [FK_catalyzes_organism_groups]
GO
/****** Object:  ForeignKey [FK_catalyzes_processes]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes]  WITH CHECK ADD  CONSTRAINT [FK_catalyzes_processes] FOREIGN KEY([process_id])
REFERENCES [dbo].[processes] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_catalyzes_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[catalyzes]'))
ALTER TABLE [dbo].[catalyzes] CHECK CONSTRAINT [FK_catalyzes_processes]
GO
/****** Object:  ForeignKey [FK_common_molecules_basic_molecules]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_common_molecules_basic_molecules]') AND parent_object_id = OBJECT_ID(N'[dbo].[common_molecules]'))
ALTER TABLE [dbo].[common_molecules]  WITH CHECK ADD  CONSTRAINT [FK_common_molecules_basic_molecules] FOREIGN KEY([id])
REFERENCES [dbo].[basic_molecules] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_common_molecules_basic_molecules]') AND parent_object_id = OBJECT_ID(N'[dbo].[common_molecules]'))
ALTER TABLE [dbo].[common_molecules] CHECK CONSTRAINT [FK_common_molecules_basic_molecules]
GO
/****** Object:  ForeignKey [FK_Compartment_Compartment]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compartment_Compartment]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compartment]'))
ALTER TABLE [dbo].[Compartment]  WITH CHECK ADD  CONSTRAINT [FK_Compartment_Compartment] FOREIGN KEY([outside])
REFERENCES [dbo].[Compartment] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compartment_Compartment]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compartment]'))
ALTER TABLE [dbo].[Compartment] CHECK CONSTRAINT [FK_Compartment_Compartment]
GO
/****** Object:  ForeignKey [FK_Compartment_Model]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compartment_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compartment]'))
ALTER TABLE [dbo].[Compartment]  WITH CHECK ADD  CONSTRAINT [FK_Compartment_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compartment_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compartment]'))
ALTER TABLE [dbo].[Compartment] CHECK CONSTRAINT [FK_Compartment_Model]
GO
/****** Object:  ForeignKey [FK_Compartment_Sbase]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compartment_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compartment]'))
ALTER TABLE [dbo].[Compartment]  WITH CHECK ADD  CONSTRAINT [FK_Compartment_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compartment_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compartment]'))
ALTER TABLE [dbo].[Compartment] CHECK CONSTRAINT [FK_Compartment_Sbase]
GO
/****** Object:  ForeignKey [FK_Compartment_UnitDefinition]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compartment_UnitDefinition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compartment]'))
ALTER TABLE [dbo].[Compartment]  WITH CHECK ADD  CONSTRAINT [FK_Compartment_UnitDefinition] FOREIGN KEY([unitsId])
REFERENCES [dbo].[UnitDefinition] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compartment_UnitDefinition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compartment]'))
ALTER TABLE [dbo].[Compartment] CHECK CONSTRAINT [FK_Compartment_UnitDefinition]
GO
/****** Object:  ForeignKey [FK_CompartmentType_Model]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CompartmentType_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[CompartmentType]'))
ALTER TABLE [dbo].[CompartmentType]  WITH CHECK ADD  CONSTRAINT [FK_CompartmentType_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CompartmentType_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[CompartmentType]'))
ALTER TABLE [dbo].[CompartmentType] CHECK CONSTRAINT [FK_CompartmentType_Model]
GO
/****** Object:  ForeignKey [FK_CompartmentType_Sbase]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CompartmentType_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[CompartmentType]'))
ALTER TABLE [dbo].[CompartmentType]  WITH CHECK ADD  CONSTRAINT [FK_CompartmentType_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CompartmentType_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[CompartmentType]'))
ALTER TABLE [dbo].[CompartmentType] CHECK CONSTRAINT [FK_CompartmentType_Sbase]
GO
/****** Object:  ForeignKey [FK_Constraint_Model]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Constraint_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Constraint]'))
ALTER TABLE [dbo].[Constraint]  WITH CHECK ADD  CONSTRAINT [FK_Constraint_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Constraint_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Constraint]'))
ALTER TABLE [dbo].[Constraint] CHECK CONSTRAINT [FK_Constraint_Model]
GO
/****** Object:  ForeignKey [FK_Constraint_Sbase]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Constraint_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Constraint]'))
ALTER TABLE [dbo].[Constraint]  WITH CHECK ADD  CONSTRAINT [FK_Constraint_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Constraint_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Constraint]'))
ALTER TABLE [dbo].[Constraint] CHECK CONSTRAINT [FK_Constraint_Sbase]
GO
/****** Object:  ForeignKey [FK_DesignedBy_Author]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DesignedBy_Author]') AND parent_object_id = OBJECT_ID(N'[dbo].[DesignedBy]'))
ALTER TABLE [dbo].[DesignedBy]  WITH CHECK ADD  CONSTRAINT [FK_DesignedBy_Author] FOREIGN KEY([AuthorId])
REFERENCES [dbo].[Author] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DesignedBy_Author]') AND parent_object_id = OBJECT_ID(N'[dbo].[DesignedBy]'))
ALTER TABLE [dbo].[DesignedBy] CHECK CONSTRAINT [FK_DesignedBy_Author]
GO
/****** Object:  ForeignKey [FK_DesignedBy_ModelMetadata]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DesignedBy_ModelMetadata]') AND parent_object_id = OBJECT_ID(N'[dbo].[DesignedBy]'))
ALTER TABLE [dbo].[DesignedBy]  WITH CHECK ADD  CONSTRAINT [FK_DesignedBy_ModelMetadata] FOREIGN KEY([ModelMetadataId])
REFERENCES [dbo].[ModelMetadata] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DesignedBy_ModelMetadata]') AND parent_object_id = OBJECT_ID(N'[dbo].[DesignedBy]'))
ALTER TABLE [dbo].[DesignedBy] CHECK CONSTRAINT [FK_DesignedBy_ModelMetadata]
GO
/****** Object:  ForeignKey [FK_ec_number_name_lookups_ec_numbers]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ec_number_name_lookups_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]'))
ALTER TABLE [dbo].[ec_number_name_lookups]  WITH CHECK ADD  CONSTRAINT [FK_ec_number_name_lookups_ec_numbers] FOREIGN KEY([ec_number])
REFERENCES [dbo].[ec_numbers] ([ec_number])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ec_number_name_lookups_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]'))
ALTER TABLE [dbo].[ec_number_name_lookups] CHECK CONSTRAINT [FK_ec_number_name_lookups_ec_numbers]
GO
/****** Object:  ForeignKey [FK_ec_number_name_lookups_molecular_entity_names]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ec_number_name_lookups_molecular_entity_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]'))
ALTER TABLE [dbo].[ec_number_name_lookups]  WITH CHECK ADD  CONSTRAINT [FK_ec_number_name_lookups_molecular_entity_names] FOREIGN KEY([name_id])
REFERENCES [dbo].[molecular_entity_names] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ec_number_name_lookups_molecular_entity_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]'))
ALTER TABLE [dbo].[ec_number_name_lookups] CHECK CONSTRAINT [FK_ec_number_name_lookups_molecular_entity_names]
GO
/****** Object:  ForeignKey [FK_ec_number_name_lookups_name_types]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ec_number_name_lookups_name_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]'))
ALTER TABLE [dbo].[ec_number_name_lookups]  WITH CHECK ADD  CONSTRAINT [FK_ec_number_name_lookups_name_types] FOREIGN KEY([name_type_id])
REFERENCES [dbo].[name_types] ([name_type_id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ec_number_name_lookups_name_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[ec_number_name_lookups]'))
ALTER TABLE [dbo].[ec_number_name_lookups] CHECK CONSTRAINT [FK_ec_number_name_lookups_name_types]
GO
/****** Object:  ForeignKey [FK_entity_name_lookups_molecular_entities]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_entity_name_lookups_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]'))
ALTER TABLE [dbo].[entity_name_lookups]  WITH CHECK ADD  CONSTRAINT [FK_entity_name_lookups_molecular_entities] FOREIGN KEY([entity_id])
REFERENCES [dbo].[molecular_entities] ([id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_entity_name_lookups_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]'))
ALTER TABLE [dbo].[entity_name_lookups] CHECK CONSTRAINT [FK_entity_name_lookups_molecular_entities]
GO
/****** Object:  ForeignKey [FK_entity_name_lookups_molecular_entity_names]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_entity_name_lookups_molecular_entity_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]'))
ALTER TABLE [dbo].[entity_name_lookups]  WITH CHECK ADD  CONSTRAINT [FK_entity_name_lookups_molecular_entity_names] FOREIGN KEY([name_id])
REFERENCES [dbo].[molecular_entity_names] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_entity_name_lookups_molecular_entity_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]'))
ALTER TABLE [dbo].[entity_name_lookups] CHECK CONSTRAINT [FK_entity_name_lookups_molecular_entity_names]
GO
/****** Object:  ForeignKey [FK_entity_name_lookups_name_types]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_entity_name_lookups_name_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]'))
ALTER TABLE [dbo].[entity_name_lookups]  WITH CHECK ADD  CONSTRAINT [FK_entity_name_lookups_name_types] FOREIGN KEY([name_type_id])
REFERENCES [dbo].[name_types] ([name_type_id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_entity_name_lookups_name_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[entity_name_lookups]'))
ALTER TABLE [dbo].[entity_name_lookups] CHECK CONSTRAINT [FK_entity_name_lookups_name_types]
GO
/****** Object:  ForeignKey [FK_Event_EventDelay]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_EventDelay]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event]'))
ALTER TABLE [dbo].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_EventDelay] FOREIGN KEY([eventDelayId])
REFERENCES [dbo].[EventDelay] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_EventDelay]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event]'))
ALTER TABLE [dbo].[Event] CHECK CONSTRAINT [FK_Event_EventDelay]
GO
/****** Object:  ForeignKey [FK_Event_EventTrigger]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_EventTrigger]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event]'))
ALTER TABLE [dbo].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_EventTrigger] FOREIGN KEY([eventTriggerId])
REFERENCES [dbo].[EventTrigger] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_EventTrigger]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event]'))
ALTER TABLE [dbo].[Event] CHECK CONSTRAINT [FK_Event_EventTrigger]
GO
/****** Object:  ForeignKey [FK_Event_Model]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event]'))
ALTER TABLE [dbo].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event]'))
ALTER TABLE [dbo].[Event] CHECK CONSTRAINT [FK_Event_Model]
GO
/****** Object:  ForeignKey [FK_Event_Sbase]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event]'))
ALTER TABLE [dbo].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event]'))
ALTER TABLE [dbo].[Event] CHECK CONSTRAINT [FK_Event_Sbase]
GO
/****** Object:  ForeignKey [FK_EventAssignment_Event]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EventAssignment_Event]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventAssignment]'))
ALTER TABLE [dbo].[EventAssignment]  WITH CHECK ADD  CONSTRAINT [FK_EventAssignment_Event] FOREIGN KEY([eventId])
REFERENCES [dbo].[Event] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EventAssignment_Event]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventAssignment]'))
ALTER TABLE [dbo].[EventAssignment] CHECK CONSTRAINT [FK_EventAssignment_Event]
GO
/****** Object:  ForeignKey [FK_EventAssignment_Sbase]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EventAssignment_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventAssignment]'))
ALTER TABLE [dbo].[EventAssignment]  WITH CHECK ADD  CONSTRAINT [FK_EventAssignment_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EventAssignment_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventAssignment]'))
ALTER TABLE [dbo].[EventAssignment] CHECK CONSTRAINT [FK_EventAssignment_Sbase]
GO
/****** Object:  ForeignKey [FK_EventDelay_Sbase]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EventDelay_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventDelay]'))
ALTER TABLE [dbo].[EventDelay]  WITH CHECK ADD  CONSTRAINT [FK_EventDelay_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EventDelay_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventDelay]'))
ALTER TABLE [dbo].[EventDelay] CHECK CONSTRAINT [FK_EventDelay_Sbase]
GO
/****** Object:  ForeignKey [FK_EventTrigger_Sbase]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EventTrigger_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventTrigger]'))
ALTER TABLE [dbo].[EventTrigger]  WITH CHECK ADD  CONSTRAINT [FK_EventTrigger_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EventTrigger_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventTrigger]'))
ALTER TABLE [dbo].[EventTrigger] CHECK CONSTRAINT [FK_EventTrigger_Sbase]
GO
/****** Object:  ForeignKey [FK_FunctionDefinition_Model]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FunctionDefinition_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[FunctionDefinition]'))
ALTER TABLE [dbo].[FunctionDefinition]  WITH CHECK ADD  CONSTRAINT [FK_FunctionDefinition_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FunctionDefinition_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[FunctionDefinition]'))
ALTER TABLE [dbo].[FunctionDefinition] CHECK CONSTRAINT [FK_FunctionDefinition_Model]
GO
/****** Object:  ForeignKey [FK_FunctionDefinition_Sbase]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FunctionDefinition_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[FunctionDefinition]'))
ALTER TABLE [dbo].[FunctionDefinition]  WITH CHECK ADD  CONSTRAINT [FK_FunctionDefinition_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FunctionDefinition_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[FunctionDefinition]'))
ALTER TABLE [dbo].[FunctionDefinition] CHECK CONSTRAINT [FK_FunctionDefinition_Sbase]
GO
/****** Object:  ForeignKey [FK_gene_and_gene_products_gene_products]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_gene_and_gene_products_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[gene_encodings]'))
ALTER TABLE [dbo].[gene_encodings]  WITH CHECK ADD  CONSTRAINT [FK_gene_and_gene_products_gene_products] FOREIGN KEY([gene_product_id])
REFERENCES [dbo].[gene_products] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_gene_and_gene_products_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[gene_encodings]'))
ALTER TABLE [dbo].[gene_encodings] CHECK CONSTRAINT [FK_gene_and_gene_products_gene_products]
GO
/****** Object:  ForeignKey [FK_gene_products_molecular_entities]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_gene_products_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[gene_products]'))
ALTER TABLE [dbo].[gene_products]  WITH CHECK ADD  CONSTRAINT [FK_gene_products_molecular_entities] FOREIGN KEY([id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_gene_products_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[gene_products]'))
ALTER TABLE [dbo].[gene_products] CHECK CONSTRAINT [FK_gene_products_molecular_entities]
GO
/****** Object:  ForeignKey [FK_genes_chromosomes]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_genes_chromosomes]') AND parent_object_id = OBJECT_ID(N'[dbo].[genes]'))
ALTER TABLE [dbo].[genes]  WITH CHECK ADD  CONSTRAINT [FK_genes_chromosomes] FOREIGN KEY([chromosome_id])
REFERENCES [dbo].[chromosomes] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_genes_chromosomes]') AND parent_object_id = OBJECT_ID(N'[dbo].[genes]'))
ALTER TABLE [dbo].[genes] CHECK CONSTRAINT [FK_genes_chromosomes]
GO
/****** Object:  ForeignKey [FK_genes_organism_groups]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_genes_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[genes]'))
ALTER TABLE [dbo].[genes]  WITH CHECK ADD  CONSTRAINT [FK_genes_organism_groups] FOREIGN KEY([organism_group_id])
REFERENCES [dbo].[organism_groups] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_genes_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[genes]'))
ALTER TABLE [dbo].[genes] CHECK CONSTRAINT [FK_genes_organism_groups]
GO
/****** Object:  ForeignKey [FK_GONodeCodes_go_terms]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GONodeCodes_go_terms]') AND parent_object_id = OBJECT_ID(N'[dbo].[GONodeCodes]'))
ALTER TABLE [dbo].[GONodeCodes]  WITH CHECK ADD  CONSTRAINT [FK_GONodeCodes_go_terms] FOREIGN KEY([goid])
REFERENCES [dbo].[go_terms] ([ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GONodeCodes_go_terms]') AND parent_object_id = OBJECT_ID(N'[dbo].[GONodeCodes]'))
ALTER TABLE [dbo].[GONodeCodes] CHECK CONSTRAINT [FK_GONodeCodes_go_terms]
GO
/****** Object:  ForeignKey [FK_InitialAssignment_Model]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InitialAssignment_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[InitialAssignment]'))
ALTER TABLE [dbo].[InitialAssignment]  WITH CHECK ADD  CONSTRAINT [FK_InitialAssignment_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InitialAssignment_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[InitialAssignment]'))
ALTER TABLE [dbo].[InitialAssignment] CHECK CONSTRAINT [FK_InitialAssignment_Model]
GO
/****** Object:  ForeignKey [FK_InitialAssignment_Sbase]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InitialAssignment_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[InitialAssignment]'))
ALTER TABLE [dbo].[InitialAssignment]  WITH CHECK ADD  CONSTRAINT [FK_InitialAssignment_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InitialAssignment_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[InitialAssignment]'))
ALTER TABLE [dbo].[InitialAssignment] CHECK CONSTRAINT [FK_InitialAssignment_Sbase]
GO
/****** Object:  ForeignKey [FK_KineticLaw_Sbase]    Script Date: 03/29/2011 20:59:08 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KineticLaw_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[KineticLaw]'))
ALTER TABLE [dbo].[KineticLaw]  WITH CHECK ADD  CONSTRAINT [FK_KineticLaw_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KineticLaw_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[KineticLaw]'))
ALTER TABLE [dbo].[KineticLaw] CHECK CONSTRAINT [FK_KineticLaw_Sbase]
GO
/****** Object:  ForeignKey [FK_MapModelsPathways_AnnotationQualifier]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapModelsPathways_AnnotationQualifier]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapModelsPathways]'))
ALTER TABLE [dbo].[MapModelsPathways]  WITH CHECK ADD  CONSTRAINT [FK_MapModelsPathways_AnnotationQualifier] FOREIGN KEY([qualifierId])
REFERENCES [dbo].[AnnotationQualifier] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapModelsPathways_AnnotationQualifier]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapModelsPathways]'))
ALTER TABLE [dbo].[MapModelsPathways] CHECK CONSTRAINT [FK_MapModelsPathways_AnnotationQualifier]
GO
/****** Object:  ForeignKey [FK_MapModelsPathways_Model]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapModelsPathways_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapModelsPathways]'))
ALTER TABLE [dbo].[MapModelsPathways]  WITH CHECK ADD  CONSTRAINT [FK_MapModelsPathways_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapModelsPathways_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapModelsPathways]'))
ALTER TABLE [dbo].[MapModelsPathways] CHECK CONSTRAINT [FK_MapModelsPathways_Model]
GO
/****** Object:  ForeignKey [FK_MapModelsPathways_organism_groups]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapModelsPathways_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapModelsPathways]'))
ALTER TABLE [dbo].[MapModelsPathways]  WITH CHECK ADD  CONSTRAINT [FK_MapModelsPathways_organism_groups] FOREIGN KEY([organismGroupId])
REFERENCES [dbo].[organism_groups] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapModelsPathways_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapModelsPathways]'))
ALTER TABLE [dbo].[MapModelsPathways] CHECK CONSTRAINT [FK_MapModelsPathways_organism_groups]
GO
/****** Object:  ForeignKey [FK_MapModelsPathways_pathways]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapModelsPathways_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapModelsPathways]'))
ALTER TABLE [dbo].[MapModelsPathways]  WITH CHECK ADD  CONSTRAINT [FK_MapModelsPathways_pathways] FOREIGN KEY([pathwayId])
REFERENCES [dbo].[pathways] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapModelsPathways_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapModelsPathways]'))
ALTER TABLE [dbo].[MapModelsPathways] CHECK CONSTRAINT [FK_MapModelsPathways_pathways]
GO
/****** Object:  ForeignKey [FK_MapReactionECNumber_AnnotationQualifier]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapReactionECNumber_AnnotationQualifier]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapReactionECNumber]'))
ALTER TABLE [dbo].[MapReactionECNumber]  WITH CHECK ADD  CONSTRAINT [FK_MapReactionECNumber_AnnotationQualifier] FOREIGN KEY([qualifierId])
REFERENCES [dbo].[AnnotationQualifier] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapReactionECNumber_AnnotationQualifier]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapReactionECNumber]'))
ALTER TABLE [dbo].[MapReactionECNumber] CHECK CONSTRAINT [FK_MapReactionECNumber_AnnotationQualifier]
GO
/****** Object:  ForeignKey [FK_MapReactionECNumber_ec_numbers]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapReactionECNumber_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapReactionECNumber]'))
ALTER TABLE [dbo].[MapReactionECNumber]  WITH CHECK ADD  CONSTRAINT [FK_MapReactionECNumber_ec_numbers] FOREIGN KEY([ecNumber])
REFERENCES [dbo].[ec_numbers] ([ec_number])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapReactionECNumber_ec_numbers]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapReactionECNumber]'))
ALTER TABLE [dbo].[MapReactionECNumber] CHECK CONSTRAINT [FK_MapReactionECNumber_ec_numbers]
GO
/****** Object:  ForeignKey [FK_MapReactionECNumber_Reaction]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapReactionECNumber_Reaction]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapReactionECNumber]'))
ALTER TABLE [dbo].[MapReactionECNumber]  WITH CHECK ADD  CONSTRAINT [FK_MapReactionECNumber_Reaction] FOREIGN KEY([reactionId])
REFERENCES [dbo].[Reaction] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapReactionECNumber_Reaction]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapReactionECNumber]'))
ALTER TABLE [dbo].[MapReactionECNumber] CHECK CONSTRAINT [FK_MapReactionECNumber_Reaction]
GO
/****** Object:  ForeignKey [FK_MapReactionsProcessEntities_AnnotationQualifier]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapReactionsProcessEntities_AnnotationQualifier]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapReactionsProcessEntities]'))
ALTER TABLE [dbo].[MapReactionsProcessEntities]  WITH CHECK ADD  CONSTRAINT [FK_MapReactionsProcessEntities_AnnotationQualifier] FOREIGN KEY([qualifierId])
REFERENCES [dbo].[AnnotationQualifier] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapReactionsProcessEntities_AnnotationQualifier]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapReactionsProcessEntities]'))
ALTER TABLE [dbo].[MapReactionsProcessEntities] CHECK CONSTRAINT [FK_MapReactionsProcessEntities_AnnotationQualifier]
GO
/****** Object:  ForeignKey [FK_MapReactionsProcessEntities_processes]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapReactionsProcessEntities_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapReactionsProcessEntities]'))
ALTER TABLE [dbo].[MapReactionsProcessEntities]  WITH CHECK ADD  CONSTRAINT [FK_MapReactionsProcessEntities_processes] FOREIGN KEY([processId])
REFERENCES [dbo].[processes] ([id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapReactionsProcessEntities_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapReactionsProcessEntities]'))
ALTER TABLE [dbo].[MapReactionsProcessEntities] CHECK CONSTRAINT [FK_MapReactionsProcessEntities_processes]
GO
/****** Object:  ForeignKey [FK_MapReactionsProcessEntities_Reaction]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapReactionsProcessEntities_Reaction]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapReactionsProcessEntities]'))
ALTER TABLE [dbo].[MapReactionsProcessEntities]  WITH CHECK ADD  CONSTRAINT [FK_MapReactionsProcessEntities_Reaction] FOREIGN KEY([reactionId])
REFERENCES [dbo].[Reaction] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapReactionsProcessEntities_Reaction]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapReactionsProcessEntities]'))
ALTER TABLE [dbo].[MapReactionsProcessEntities] CHECK CONSTRAINT [FK_MapReactionsProcessEntities_Reaction]
GO
/****** Object:  ForeignKey [FK_MapSpeciesMolecularEntities_molecular_entities]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapSpeciesMolecularEntities_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapSpeciesMolecularEntities]'))
ALTER TABLE [dbo].[MapSpeciesMolecularEntities]  WITH CHECK ADD  CONSTRAINT [FK_MapSpeciesMolecularEntities_molecular_entities] FOREIGN KEY([molecularEntityId])
REFERENCES [dbo].[molecular_entities] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapSpeciesMolecularEntities_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapSpeciesMolecularEntities]'))
ALTER TABLE [dbo].[MapSpeciesMolecularEntities] CHECK CONSTRAINT [FK_MapSpeciesMolecularEntities_molecular_entities]
GO
/****** Object:  ForeignKey [FK_MapSpeciesMolecularEntities_Species]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapSpeciesMolecularEntities_Species]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapSpeciesMolecularEntities]'))
ALTER TABLE [dbo].[MapSpeciesMolecularEntities]  WITH CHECK ADD  CONSTRAINT [FK_MapSpeciesMolecularEntities_Species] FOREIGN KEY([speciesId])
REFERENCES [dbo].[Species] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MapSpeciesMolecularEntities_Species]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapSpeciesMolecularEntities]'))
ALTER TABLE [dbo].[MapSpeciesMolecularEntities] CHECK CONSTRAINT [FK_MapSpeciesMolecularEntities_Species]
GO
/****** Object:  ForeignKey [FK_SpeciesMolecularEntityMapping_AnnotationQualifier]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SpeciesMolecularEntityMapping_AnnotationQualifier]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapSpeciesMolecularEntities]'))
ALTER TABLE [dbo].[MapSpeciesMolecularEntities]  WITH CHECK ADD  CONSTRAINT [FK_SpeciesMolecularEntityMapping_AnnotationQualifier] FOREIGN KEY([qualifierId])
REFERENCES [dbo].[AnnotationQualifier] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SpeciesMolecularEntityMapping_AnnotationQualifier]') AND parent_object_id = OBJECT_ID(N'[dbo].[MapSpeciesMolecularEntities]'))
ALTER TABLE [dbo].[MapSpeciesMolecularEntities] CHECK CONSTRAINT [FK_SpeciesMolecularEntityMapping_AnnotationQualifier]
GO
/****** Object:  ForeignKey [FK_Model_DataSource]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_DataSource]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model]'))
ALTER TABLE [dbo].[Model]  WITH CHECK ADD  CONSTRAINT [FK_Model_DataSource] FOREIGN KEY([dataSourceId])
REFERENCES [dbo].[DataSource] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_DataSource]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model]'))
ALTER TABLE [dbo].[Model] CHECK CONSTRAINT [FK_Model_DataSource]
GO
/****** Object:  ForeignKey [FK_Model_Sbase]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model]'))
ALTER TABLE [dbo].[Model]  WITH CHECK ADD  CONSTRAINT [FK_Model_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Model_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Model]'))
ALTER TABLE [dbo].[Model] CHECK CONSTRAINT [FK_Model_Sbase]
GO
/****** Object:  ForeignKey [FK_ModelLayout_Model]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ModelLayout_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[ModelLayout]'))
ALTER TABLE [dbo].[ModelLayout]  WITH CHECK ADD  CONSTRAINT [FK_ModelLayout_Model] FOREIGN KEY([id])
REFERENCES [dbo].[Model] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ModelLayout_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[ModelLayout]'))
ALTER TABLE [dbo].[ModelLayout] CHECK CONSTRAINT [FK_ModelLayout_Model]
GO
/****** Object:  ForeignKey [FK_ModelOrganism_AnnotationQualifier]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ModelOrganism_AnnotationQualifier]') AND parent_object_id = OBJECT_ID(N'[dbo].[ModelOrganism]'))
ALTER TABLE [dbo].[ModelOrganism]  WITH CHECK ADD  CONSTRAINT [FK_ModelOrganism_AnnotationQualifier] FOREIGN KEY([qualifierId])
REFERENCES [dbo].[AnnotationQualifier] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ModelOrganism_AnnotationQualifier]') AND parent_object_id = OBJECT_ID(N'[dbo].[ModelOrganism]'))
ALTER TABLE [dbo].[ModelOrganism] CHECK CONSTRAINT [FK_ModelOrganism_AnnotationQualifier]
GO
/****** Object:  ForeignKey [FK_ModelOrganism_Model]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ModelOrganism_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[ModelOrganism]'))
ALTER TABLE [dbo].[ModelOrganism]  WITH CHECK ADD  CONSTRAINT [FK_ModelOrganism_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ModelOrganism_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[ModelOrganism]'))
ALTER TABLE [dbo].[ModelOrganism] CHECK CONSTRAINT [FK_ModelOrganism_Model]
GO
/****** Object:  ForeignKey [FK_ModelOrganism_organism_groups]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ModelOrganism_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[ModelOrganism]'))
ALTER TABLE [dbo].[ModelOrganism]  WITH CHECK ADD  CONSTRAINT [FK_ModelOrganism_organism_groups] FOREIGN KEY([organismGroupId])
REFERENCES [dbo].[organism_groups] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ModelOrganism_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[ModelOrganism]'))
ALTER TABLE [dbo].[ModelOrganism] CHECK CONSTRAINT [FK_ModelOrganism_organism_groups]
GO
/****** Object:  ForeignKey [FK_molecular_entities_molecular_entity_names]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_molecular_entities_molecular_entity_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[molecular_entities]'))
ALTER TABLE [dbo].[molecular_entities]  WITH CHECK ADD  CONSTRAINT [FK_molecular_entities_molecular_entity_names] FOREIGN KEY([name])
REFERENCES [dbo].[molecular_entity_names] ([name])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_molecular_entities_molecular_entity_names]') AND parent_object_id = OBJECT_ID(N'[dbo].[molecular_entities]'))
ALTER TABLE [dbo].[molecular_entities] CHECK CONSTRAINT [FK_molecular_entities_molecular_entity_names]
GO
/****** Object:  ForeignKey [FK_molecular_entities_molecular_entity_types]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_molecular_entities_molecular_entity_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[molecular_entities]'))
ALTER TABLE [dbo].[molecular_entities]  WITH CHECK ADD  CONSTRAINT [FK_molecular_entities_molecular_entity_types] FOREIGN KEY([type_id])
REFERENCES [dbo].[molecular_entity_types] ([type_id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_molecular_entities_molecular_entity_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[molecular_entities]'))
ALTER TABLE [dbo].[molecular_entities] CHECK CONSTRAINT [FK_molecular_entities_molecular_entity_types]
GO
/****** Object:  ForeignKey [FK_external_database_links_external_databases]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_external_database_links_external_databases]') AND parent_object_id = OBJECT_ID(N'[dbo].[OLD_external_database_links]'))
ALTER TABLE [dbo].[OLD_external_database_links]  WITH CHECK ADD  CONSTRAINT [FK_external_database_links_external_databases] FOREIGN KEY([external_database_id])
REFERENCES [dbo].[OLD_external_databases] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_external_database_links_external_databases]') AND parent_object_id = OBJECT_ID(N'[dbo].[OLD_external_database_links]'))
ALTER TABLE [dbo].[OLD_external_database_links] CHECK CONSTRAINT [FK_external_database_links_external_databases]
GO
/****** Object:  ForeignKey [FK_organism_groups_organism_groups]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_organism_groups_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[organism_groups]'))
ALTER TABLE [dbo].[organism_groups]  WITH CHECK ADD  CONSTRAINT [FK_organism_groups_organism_groups] FOREIGN KEY([parent_id])
REFERENCES [dbo].[organism_groups] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_organism_groups_organism_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[organism_groups]'))
ALTER TABLE [dbo].[organism_groups] CHECK CONSTRAINT [FK_organism_groups_organism_groups]
GO
/****** Object:  ForeignKey [FK_Parameter_Model]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Parameter_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Parameter]'))
ALTER TABLE [dbo].[Parameter]  WITH CHECK ADD  CONSTRAINT [FK_Parameter_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Parameter_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Parameter]'))
ALTER TABLE [dbo].[Parameter] CHECK CONSTRAINT [FK_Parameter_Model]
GO
/****** Object:  ForeignKey [FK_Parameter_Sbase]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Parameter_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Parameter]'))
ALTER TABLE [dbo].[Parameter]  WITH CHECK ADD  CONSTRAINT [FK_Parameter_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Parameter_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Parameter]'))
ALTER TABLE [dbo].[Parameter] CHECK CONSTRAINT [FK_Parameter_Sbase]
GO
/****** Object:  ForeignKey [FK_Parameter_UnitDefinition]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Parameter_UnitDefinition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Parameter]'))
ALTER TABLE [dbo].[Parameter]  WITH CHECK ADD  CONSTRAINT [FK_Parameter_UnitDefinition] FOREIGN KEY([unitsId])
REFERENCES [dbo].[UnitDefinition] ([id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Parameter_UnitDefinition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Parameter]'))
ALTER TABLE [dbo].[Parameter] CHECK CONSTRAINT [FK_Parameter_UnitDefinition]
GO
/****** Object:  ForeignKey [FK_pathway_links_molecular_entities]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_links_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_links]'))
ALTER TABLE [dbo].[pathway_links]  WITH CHECK ADD  CONSTRAINT [FK_pathway_links_molecular_entities] FOREIGN KEY([entity_id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_links_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_links]'))
ALTER TABLE [dbo].[pathway_links] CHECK CONSTRAINT [FK_pathway_links_molecular_entities]
GO
/****** Object:  ForeignKey [FK_pathway_links_pathways]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_links_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_links]'))
ALTER TABLE [dbo].[pathway_links]  WITH CHECK ADD  CONSTRAINT [FK_pathway_links_pathways] FOREIGN KEY([pathway_id_1])
REFERENCES [dbo].[pathways] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_links_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_links]'))
ALTER TABLE [dbo].[pathway_links] CHECK CONSTRAINT [FK_pathway_links_pathways]
GO
/****** Object:  ForeignKey [FK_pathway_links_pathways1]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_links_pathways1]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_links]'))
ALTER TABLE [dbo].[pathway_links]  WITH CHECK ADD  CONSTRAINT [FK_pathway_links_pathways1] FOREIGN KEY([pathway_id_2])
REFERENCES [dbo].[pathways] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_links_pathways1]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_links]'))
ALTER TABLE [dbo].[pathway_links] CHECK CONSTRAINT [FK_pathway_links_pathways1]
GO
/****** Object:  ForeignKey [FK_pathway_processes_pathways]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_processes_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_processes]'))
ALTER TABLE [dbo].[pathway_processes]  WITH CHECK ADD  CONSTRAINT [FK_pathway_processes_pathways] FOREIGN KEY([pathway_id])
REFERENCES [dbo].[pathways] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_processes_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_processes]'))
ALTER TABLE [dbo].[pathway_processes] CHECK CONSTRAINT [FK_pathway_processes_pathways]
GO
/****** Object:  ForeignKey [FK_pathway_processes_processes]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_processes_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_processes]'))
ALTER TABLE [dbo].[pathway_processes]  WITH CHECK ADD  CONSTRAINT [FK_pathway_processes_processes] FOREIGN KEY([process_id])
REFERENCES [dbo].[processes] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_processes_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_processes]'))
ALTER TABLE [dbo].[pathway_processes] CHECK CONSTRAINT [FK_pathway_processes_processes]
GO
/****** Object:  ForeignKey [FK_pathway_to_pathway_groups_pathway_groups]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_to_pathway_groups_pathway_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]'))
ALTER TABLE [dbo].[pathway_to_pathway_groups]  WITH CHECK ADD  CONSTRAINT [FK_pathway_to_pathway_groups_pathway_groups] FOREIGN KEY([group_id])
REFERENCES [dbo].[pathway_groups] ([group_id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_to_pathway_groups_pathway_groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]'))
ALTER TABLE [dbo].[pathway_to_pathway_groups] CHECK CONSTRAINT [FK_pathway_to_pathway_groups_pathway_groups]
GO
/****** Object:  ForeignKey [FK_pathway_to_pathway_groups_pathways]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_to_pathway_groups_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]'))
ALTER TABLE [dbo].[pathway_to_pathway_groups]  WITH CHECK ADD  CONSTRAINT [FK_pathway_to_pathway_groups_pathways] FOREIGN KEY([pathway_id])
REFERENCES [dbo].[pathways] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathway_to_pathway_groups_pathways]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathway_to_pathway_groups]'))
ALTER TABLE [dbo].[pathway_to_pathway_groups] CHECK CONSTRAINT [FK_pathway_to_pathway_groups_pathways]
GO
/****** Object:  ForeignKey [FK_pathways_pathway_types]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathways_pathway_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathways]'))
ALTER TABLE [dbo].[pathways]  WITH CHECK ADD  CONSTRAINT [FK_pathways_pathway_types] FOREIGN KEY([pathway_type_id])
REFERENCES [dbo].[pathway_types] ([pathway_type_id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_pathways_pathway_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[pathways]'))
ALTER TABLE [dbo].[pathways] CHECK CONSTRAINT [FK_pathways_pathway_types]
GO
/****** Object:  ForeignKey [FK_process_entities_molecular_entities]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH CHECK ADD  CONSTRAINT [FK_process_entities_molecular_entities] FOREIGN KEY([entity_id])
REFERENCES [dbo].[molecular_entities] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities] CHECK CONSTRAINT [FK_process_entities_molecular_entities]
GO
/****** Object:  ForeignKey [FK_process_entities_process_entity_roles]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_process_entity_roles]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH CHECK ADD  CONSTRAINT [FK_process_entities_process_entity_roles] FOREIGN KEY([role_id])
REFERENCES [dbo].[process_entity_roles] ([role_id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_process_entity_roles]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities] CHECK CONSTRAINT [FK_process_entities_process_entity_roles]
GO
/****** Object:  ForeignKey [FK_process_entities_processes]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities]  WITH CHECK ADD  CONSTRAINT [FK_process_entities_processes] FOREIGN KEY([process_id])
REFERENCES [dbo].[processes] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_process_entities_processes]') AND parent_object_id = OBJECT_ID(N'[dbo].[process_entities]'))
ALTER TABLE [dbo].[process_entities] CHECK CONSTRAINT [FK_process_entities_processes]
GO
/****** Object:  ForeignKey [FK_proteins_gene_products]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_proteins_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[proteins]'))
ALTER TABLE [dbo].[proteins]  WITH CHECK ADD  CONSTRAINT [FK_proteins_gene_products] FOREIGN KEY([id])
REFERENCES [dbo].[gene_products] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_proteins_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[proteins]'))
ALTER TABLE [dbo].[proteins] CHECK CONSTRAINT [FK_proteins_gene_products]
GO
/****** Object:  ForeignKey [FK_Reaction_KineticLaw]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Reaction_KineticLaw]') AND parent_object_id = OBJECT_ID(N'[dbo].[Reaction]'))
ALTER TABLE [dbo].[Reaction]  WITH CHECK ADD  CONSTRAINT [FK_Reaction_KineticLaw] FOREIGN KEY([kineticLawId])
REFERENCES [dbo].[KineticLaw] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Reaction_KineticLaw]') AND parent_object_id = OBJECT_ID(N'[dbo].[Reaction]'))
ALTER TABLE [dbo].[Reaction] CHECK CONSTRAINT [FK_Reaction_KineticLaw]
GO
/****** Object:  ForeignKey [FK_Reaction_Model]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Reaction_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Reaction]'))
ALTER TABLE [dbo].[Reaction]  WITH CHECK ADD  CONSTRAINT [FK_Reaction_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Reaction_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Reaction]'))
ALTER TABLE [dbo].[Reaction] CHECK CONSTRAINT [FK_Reaction_Model]
GO
/****** Object:  ForeignKey [FK_Reaction_Sbase]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Reaction_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Reaction]'))
ALTER TABLE [dbo].[Reaction]  WITH CHECK ADD  CONSTRAINT [FK_Reaction_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Reaction_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Reaction]'))
ALTER TABLE [dbo].[Reaction] CHECK CONSTRAINT [FK_Reaction_Sbase]
GO
/****** Object:  ForeignKey [FK_ReactionSpecies_Reaction]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReactionSpecies_Reaction]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReactionSpecies]'))
ALTER TABLE [dbo].[ReactionSpecies]  WITH CHECK ADD  CONSTRAINT [FK_ReactionSpecies_Reaction] FOREIGN KEY([reactionId])
REFERENCES [dbo].[Reaction] ([id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReactionSpecies_Reaction]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReactionSpecies]'))
ALTER TABLE [dbo].[ReactionSpecies] CHECK CONSTRAINT [FK_ReactionSpecies_Reaction]
GO
/****** Object:  ForeignKey [FK_ReactionSpecies_ReactionSpeciesRole]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReactionSpecies_ReactionSpeciesRole]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReactionSpecies]'))
ALTER TABLE [dbo].[ReactionSpecies]  WITH CHECK ADD  CONSTRAINT [FK_ReactionSpecies_ReactionSpeciesRole] FOREIGN KEY([roleId])
REFERENCES [dbo].[ReactionSpeciesRole] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReactionSpecies_ReactionSpeciesRole]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReactionSpecies]'))
ALTER TABLE [dbo].[ReactionSpecies] CHECK CONSTRAINT [FK_ReactionSpecies_ReactionSpeciesRole]
GO
/****** Object:  ForeignKey [FK_ReactionSpecies_Sbase]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReactionSpecies_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReactionSpecies]'))
ALTER TABLE [dbo].[ReactionSpecies]  WITH CHECK ADD  CONSTRAINT [FK_ReactionSpecies_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReactionSpecies_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReactionSpecies]'))
ALTER TABLE [dbo].[ReactionSpecies] CHECK CONSTRAINT [FK_ReactionSpecies_Sbase]
GO
/****** Object:  ForeignKey [FK_ReactionSpecies_Species]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReactionSpecies_Species]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReactionSpecies]'))
ALTER TABLE [dbo].[ReactionSpecies]  WITH CHECK ADD  CONSTRAINT [FK_ReactionSpecies_Species] FOREIGN KEY([speciesId])
REFERENCES [dbo].[Species] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReactionSpecies_Species]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReactionSpecies]'))
ALTER TABLE [dbo].[ReactionSpecies] CHECK CONSTRAINT [FK_ReactionSpecies_Species]
GO
/****** Object:  ForeignKey [FK_ReactionSpecies_StoichiometryMath]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReactionSpecies_StoichiometryMath]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReactionSpecies]'))
ALTER TABLE [dbo].[ReactionSpecies]  WITH CHECK ADD  CONSTRAINT [FK_ReactionSpecies_StoichiometryMath] FOREIGN KEY([stoichiometryMathId])
REFERENCES [dbo].[StoichiometryMath] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReactionSpecies_StoichiometryMath]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReactionSpecies]'))
ALTER TABLE [dbo].[ReactionSpecies] CHECK CONSTRAINT [FK_ReactionSpecies_StoichiometryMath]
GO
/****** Object:  ForeignKey [FK_rnas_gene_products]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_rnas_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[rnas]'))
ALTER TABLE [dbo].[rnas]  WITH CHECK ADD  CONSTRAINT [FK_rnas_gene_products] FOREIGN KEY([id])
REFERENCES [dbo].[gene_products] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_rnas_gene_products]') AND parent_object_id = OBJECT_ID(N'[dbo].[rnas]'))
ALTER TABLE [dbo].[rnas] CHECK CONSTRAINT [FK_rnas_gene_products]
GO
/****** Object:  ForeignKey [FK_rnas_rna_types]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_rnas_rna_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[rnas]'))
ALTER TABLE [dbo].[rnas]  WITH CHECK ADD  CONSTRAINT [FK_rnas_rna_types] FOREIGN KEY([rna_type_id])
REFERENCES [dbo].[rna_types] ([rna_type_id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_rnas_rna_types]') AND parent_object_id = OBJECT_ID(N'[dbo].[rnas]'))
ALTER TABLE [dbo].[rnas] CHECK CONSTRAINT [FK_rnas_rna_types]
GO
/****** Object:  ForeignKey [FK_Rule_Model]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Rule_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Rule]'))
ALTER TABLE [dbo].[Rule]  WITH CHECK ADD  CONSTRAINT [FK_Rule_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Rule_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Rule]'))
ALTER TABLE [dbo].[Rule] CHECK CONSTRAINT [FK_Rule_Model]
GO
/****** Object:  ForeignKey [FK_Rule_RuleType]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Rule_RuleType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Rule]'))
ALTER TABLE [dbo].[Rule]  WITH CHECK ADD  CONSTRAINT [FK_Rule_RuleType] FOREIGN KEY([ruleTypeId])
REFERENCES [dbo].[RuleType] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Rule_RuleType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Rule]'))
ALTER TABLE [dbo].[Rule] CHECK CONSTRAINT [FK_Rule_RuleType]
GO
/****** Object:  ForeignKey [FK_Rule_Sbase]    Script Date: 03/29/2011 20:59:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Rule_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Rule]'))
ALTER TABLE [dbo].[Rule]  WITH CHECK ADD  CONSTRAINT [FK_Rule_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Rule_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Rule]'))
ALTER TABLE [dbo].[Rule] CHECK CONSTRAINT [FK_Rule_Sbase]
GO
/****** Object:  ForeignKey [FK_Species_Compartment]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Species_Compartment]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
ALTER TABLE [dbo].[Species]  WITH CHECK ADD  CONSTRAINT [FK_Species_Compartment] FOREIGN KEY([compartmentId])
REFERENCES [dbo].[Compartment] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Species_Compartment]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
ALTER TABLE [dbo].[Species] CHECK CONSTRAINT [FK_Species_Compartment]
GO
/****** Object:  ForeignKey [FK_Species_Model]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Species_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
ALTER TABLE [dbo].[Species]  WITH CHECK ADD  CONSTRAINT [FK_Species_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Species_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
ALTER TABLE [dbo].[Species] CHECK CONSTRAINT [FK_Species_Model]
GO
/****** Object:  ForeignKey [FK_Species_Sbase]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Species_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
ALTER TABLE [dbo].[Species]  WITH CHECK ADD  CONSTRAINT [FK_Species_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Species_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
ALTER TABLE [dbo].[Species] CHECK CONSTRAINT [FK_Species_Sbase]
GO
/****** Object:  ForeignKey [FK_Species_SpeciesType]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Species_SpeciesType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
ALTER TABLE [dbo].[Species]  WITH CHECK ADD  CONSTRAINT [FK_Species_SpeciesType] FOREIGN KEY([speciesTypeId])
REFERENCES [dbo].[SpeciesType] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Species_SpeciesType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
ALTER TABLE [dbo].[Species] CHECK CONSTRAINT [FK_Species_SpeciesType]
GO
/****** Object:  ForeignKey [FK_Species_UnitDefinition]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Species_UnitDefinition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
ALTER TABLE [dbo].[Species]  WITH CHECK ADD  CONSTRAINT [FK_Species_UnitDefinition] FOREIGN KEY([substanceUnitsId])
REFERENCES [dbo].[UnitDefinition] ([id])
ON UPDATE SET NULL
ON DELETE SET NULL
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Species_UnitDefinition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Species]'))
ALTER TABLE [dbo].[Species] CHECK CONSTRAINT [FK_Species_UnitDefinition]
GO
/****** Object:  ForeignKey [FK_SpeciesType_Model]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SpeciesType_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[SpeciesType]'))
ALTER TABLE [dbo].[SpeciesType]  WITH CHECK ADD  CONSTRAINT [FK_SpeciesType_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SpeciesType_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[SpeciesType]'))
ALTER TABLE [dbo].[SpeciesType] CHECK CONSTRAINT [FK_SpeciesType_Model]
GO
/****** Object:  ForeignKey [FK_SpeciesType_Sbase]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SpeciesType_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[SpeciesType]'))
ALTER TABLE [dbo].[SpeciesType]  WITH CHECK ADD  CONSTRAINT [FK_SpeciesType_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SpeciesType_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[SpeciesType]'))
ALTER TABLE [dbo].[SpeciesType] CHECK CONSTRAINT [FK_SpeciesType_Sbase]
GO
/****** Object:  ForeignKey [FK_StoichiometryMath_Sbase]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoichiometryMath_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoichiometryMath]'))
ALTER TABLE [dbo].[StoichiometryMath]  WITH CHECK ADD  CONSTRAINT [FK_StoichiometryMath_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoichiometryMath_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoichiometryMath]'))
ALTER TABLE [dbo].[StoichiometryMath] CHECK CONSTRAINT [FK_StoichiometryMath_Sbase]
GO
/****** Object:  ForeignKey [FK_Unit_Model]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Unit_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Unit]'))
ALTER TABLE [dbo].[Unit]  WITH CHECK ADD  CONSTRAINT [FK_Unit_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Unit_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[Unit]'))
ALTER TABLE [dbo].[Unit] CHECK CONSTRAINT [FK_Unit_Model]
GO
/****** Object:  ForeignKey [FK_Unit_Sbase]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Unit_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Unit]'))
ALTER TABLE [dbo].[Unit]  WITH CHECK ADD  CONSTRAINT [FK_Unit_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Unit_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[Unit]'))
ALTER TABLE [dbo].[Unit] CHECK CONSTRAINT [FK_Unit_Sbase]
GO
/****** Object:  ForeignKey [FK_Unit_UnitDefinition]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Unit_UnitDefinition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Unit]'))
ALTER TABLE [dbo].[Unit]  WITH CHECK ADD  CONSTRAINT [FK_Unit_UnitDefinition] FOREIGN KEY([kind])
REFERENCES [dbo].[UnitDefinition] ([id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Unit_UnitDefinition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Unit]'))
ALTER TABLE [dbo].[Unit] CHECK CONSTRAINT [FK_Unit_UnitDefinition]
GO
/****** Object:  ForeignKey [FK_UnitComposition_Unit]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UnitComposition_Unit]') AND parent_object_id = OBJECT_ID(N'[dbo].[UnitComposition]'))
ALTER TABLE [dbo].[UnitComposition]  WITH CHECK ADD  CONSTRAINT [FK_UnitComposition_Unit] FOREIGN KEY([unitId])
REFERENCES [dbo].[Unit] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UnitComposition_Unit]') AND parent_object_id = OBJECT_ID(N'[dbo].[UnitComposition]'))
ALTER TABLE [dbo].[UnitComposition] CHECK CONSTRAINT [FK_UnitComposition_Unit]
GO
/****** Object:  ForeignKey [FK_UnitComposition_UnitDefinition]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UnitComposition_UnitDefinition]') AND parent_object_id = OBJECT_ID(N'[dbo].[UnitComposition]'))
ALTER TABLE [dbo].[UnitComposition]  WITH CHECK ADD  CONSTRAINT [FK_UnitComposition_UnitDefinition] FOREIGN KEY([unitDefinitionId])
REFERENCES [dbo].[UnitDefinition] ([id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UnitComposition_UnitDefinition]') AND parent_object_id = OBJECT_ID(N'[dbo].[UnitComposition]'))
ALTER TABLE [dbo].[UnitComposition] CHECK CONSTRAINT [FK_UnitComposition_UnitDefinition]
GO
/****** Object:  ForeignKey [FK_UnitDefinition_Model]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UnitDefinition_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[UnitDefinition]'))
ALTER TABLE [dbo].[UnitDefinition]  WITH CHECK ADD  CONSTRAINT [FK_UnitDefinition_Model] FOREIGN KEY([modelId])
REFERENCES [dbo].[Model] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UnitDefinition_Model]') AND parent_object_id = OBJECT_ID(N'[dbo].[UnitDefinition]'))
ALTER TABLE [dbo].[UnitDefinition] CHECK CONSTRAINT [FK_UnitDefinition_Model]
GO
/****** Object:  ForeignKey [FK_UnitDefinition_Sbase]    Script Date: 03/29/2011 20:59:10 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UnitDefinition_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[UnitDefinition]'))
ALTER TABLE [dbo].[UnitDefinition]  WITH CHECK ADD  CONSTRAINT [FK_UnitDefinition_Sbase] FOREIGN KEY([id])
REFERENCES [dbo].[Sbase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UnitDefinition_Sbase]') AND parent_object_id = OBJECT_ID(N'[dbo].[UnitDefinition]'))
ALTER TABLE [dbo].[UnitDefinition] CHECK CONSTRAINT [FK_UnitDefinition_Sbase]
GO
