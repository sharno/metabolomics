USE [master]
GO
--IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'mathematical')
--BEGIN
--CREATE DATABASE [PathCase_SystemBiology] ON  PRIMARY 
--( NAME = N'mathematical', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\mathematical.mdf' , SIZE = 2048KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
-- LOG ON 
--( NAME = N'mathematical_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\mathematical_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
--END

GO
EXEC dbo.sp_dbcmptlevel @dbname=N'PathCase_SystemBiology', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PathCase_SystemBiology].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO
ALTER DATABASE [PathCase_SystemBiology] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET ARITHABORT OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [PathCase_SystemBiology] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PathCase_SystemBiology] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PathCase_SystemBiology] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET RECURSIVE_TRIGGERS OFF 
GO
--ALTER DATABASE [PathCase_SystemBiology] SET  ENABLE_BROKER 
--GO
ALTER DATABASE [PathCase_SystemBiology] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PathCase_SystemBiology] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PathCase_SystemBiology] SET  READ_WRITE 
GO
ALTER DATABASE [PathCase_SystemBiology] SET RECOVERY FULL 
GO
ALTER DATABASE [PathCase_SystemBiology] SET  MULTI_USER 
GO
ALTER DATABASE [PathCase_SystemBiology] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PathCase_SystemBiology] SET DB_CHAINING OFF 
USE [PathCase_SystemBiology]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Delay]') AND type in (N'U'))
BEGIN
CREATE TABLE [Delay](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[math_expression] [xml] NOT NULL,
 CONSTRAINT [PK_Delay] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* Pathways table already exists, do nothing! */
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Pathway]') AND type in (N'U'))
--BEGIN
--CREATE TABLE [Pathway](
--	[id] [uniqueidentifier] NOT NULL,
--	[name] [varchar](50) NOT NULL,
-- CONSTRAINT [PK_Pathway] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]
--END
--GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Trigger]') AND type in (N'U'))
BEGIN
CREATE TABLE [Trigger](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[math_expression] [xml] NOT NULL,
 CONSTRAINT [PK_Trigger] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Function]') AND type in (N'U'))
BEGIN
CREATE TABLE [Function](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[formula] [xml] NOT NULL,
 CONSTRAINT [PK_Function] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Parameter]') AND type in (N'U'))
BEGIN
CREATE TABLE [Parameter](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[constant] [bit] NOT NULL,
	[notes] [varchar](50) NULL,
 CONSTRAINT [PK_Parameter] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* Molecular Entity Types already exists, do nothing! */
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Molecular_Entity_Type]') AND type in (N'U'))
--BEGIN
--CREATE TABLE [Molecular_Entity_Type](
--	[id] [uniqueidentifier] NOT NULL,
--	[name] [varchar](50) NOT NULL,
-- CONSTRAINT [PK_Molecular_Entity_Type] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]
--END
--GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Compartment_Type]') AND type in (N'U'))
BEGIN
CREATE TABLE [Compartment_Type](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Compartment_Type] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Unit]') AND type in (N'U'))
BEGIN
CREATE TABLE [Unit](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[is_basic] [bit] NOT NULL,
 CONSTRAINT [PK_Unit] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* Processes table adds 'fast' column */
--ALTER TABLE [Processes] ADD [fast] [bit] NULL;

--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Process]') AND type in (N'U'))
--BEGIN
--CREATE TABLE [Process](
--	[id] [uniqueidentifier] NOT NULL,
--	[name] [varchar](50) NOT NULL,
--	[reversible] [bit] NULL,
--	[fast] [bit] NULL,
-- CONSTRAINT [PK_Process] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]
--END
--GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Kinetic_Law]') AND type in (N'U'))
BEGIN
CREATE TABLE [Kinetic_Law](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NULL,
	[math_expression] [xml] NOT NULL,
 CONSTRAINT [PK_Kinetic_law] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Model]') AND type in (N'U'))
BEGIN
CREATE TABLE [Model](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[SBMLData] [xml] NOT NULL,
	[creation_date] [datetime] NULL,
	[last_modified_date] [datetime] NULL,
 CONSTRAINT [PK_Model] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rule_Type]') AND type in (N'U'))
BEGIN
CREATE TABLE [Rule_Type](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Rule_Type] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Event]') AND type in (N'U'))
BEGIN
CREATE TABLE [Event](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[notes] [varchar](50) NULL,
	[trigger_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Trigger_Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [Trigger_Argument](
	[trigger_id] [uniqueidentifier] NOT NULL,
	[arg_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Trigger_Argument] PRIMARY KEY CLUSTERED 
(
	[trigger_id] ASC,
	[arg_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Event_Trigger_Rule_Delay]') AND type in (N'U'))
BEGIN
CREATE TABLE [Event_Trigger_Rule_Delay](
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
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Delay_Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [Delay_Argument](
	[delay_id] [uniqueidentifier] NOT NULL,
	[arg_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Delay_Argument] PRIMARY KEY CLUSTERED 
(
	[delay_id] ASC,
	[arg_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Pathway_Model]') AND type in (N'U'))
BEGIN
CREATE TABLE [Pathway_Model](
	[pathway_id] [uniqueidentifier] NOT NULL,
	[model_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Pathway_Model] PRIMARY KEY CLUSTERED 
(
	[pathway_id] ASC,
	[model_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Model_Function]') AND type in (N'U'))
BEGIN
CREATE TABLE [Model_Function](
	[model_id] [uniqueidentifier] NOT NULL,
	[function_id] [uniqueidentifier] NOT NULL,
	[notes] [varchar](50) NULL,
 CONSTRAINT [PK_Has_Function] PRIMARY KEY CLUSTERED 
(
	[model_id] ASC,
	[function_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Function_Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [Function_Argument](
	[func_id] [uniqueidentifier] NOT NULL,
	[arg_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Function_Argument] PRIMARY KEY CLUSTERED 
(
	[func_id] ASC,
	[arg_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Variation_Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [Variation_Argument](
	[var_id] [uniqueidentifier] NOT NULL,
	[arg_id] [uniqueidentifier] NOT NULL,
	[value] [float] NOT NULL,
 CONSTRAINT [PK_Variation_Argument] PRIMARY KEY CLUSTERED 
(
	[var_id] ASC,
	[arg_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [Argument](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[molecule_id] [uniqueidentifier] NULL,
	[param_id] [uniqueidentifier] NULL,
	[compartment_id] [uniqueidentifier] NULL,
	[unit_id] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Argument] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* 4 new fields added to Process Entities */
--TODO: should these be non-null with default values???
--ALTER TABLE [process_entities] ADD [stochiometry] [int] NULL
--ALTER TABLE [process_entities] ADD [boundaryCondition] [bit] NULL
--ALTER TABLE [process_entities] ADD [charge] [tinyint] NULL
--ALTER TABLE [process_entities] ADD [constant] [bit] NULL
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[process_entities]') AND type in (N'U'))
--BEGIN
--CREATE TABLE [process_entities](
--	[process_id] [uniqueidentifier] NOT NULL,
--	[molecule_id] [uniqueidentifier] NOT NULL,
--	[molecule_role] [varchar](10) NOT NULL,
----TODO: need to add these 4 fields to the old schema!!!
--	[stochiometry] [int] NOT NULL,
--	[boundaryCondition] [bit] NOT NULL,
--	[charge] [tinyint] NULL,
--	[constant] [bit] NOT NULL,
-- CONSTRAINT [PK_process_entities] PRIMARY KEY CLUSTERED 
--(
--	[process_id] ASC,
--	[molecule_id] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]
--END
--GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Compartment]') AND type in (N'U'))
BEGIN
CREATE TABLE [Compartment](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[comp_type_id] [uniqueidentifier] NULL,
	[spatialDimensions] [int] NOT NULL,
	[size] [float] NOT NULL,
	[constant] [bit] NULL,
	[parent_comp_id] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Compartment] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Composed_Units]') AND type in (N'U'))
BEGIN
CREATE TABLE [Composed_Units](
	[generated_unit_id] [uniqueidentifier] NOT NULL,
	[used_unit_id] [uniqueidentifier] NOT NULL,
	[scale] [float] NULL,
	[exponent] [float] NULL,
	[multiplier] [float] NULL,
 CONSTRAINT [PK_Composed_Units] PRIMARY KEY CLUSTERED 
(
	[generated_unit_id] ASC,
	[used_unit_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Model_Parameter]') AND type in (N'U'))
BEGIN
CREATE TABLE [Model_Parameter](
	[model_id] [uniqueidentifier] NOT NULL,
	[param_id] [uniqueidentifier] NOT NULL,
	[notes] [varchar](50) NULL,
 CONSTRAINT [PK_Model_Parameter] PRIMARY KEY CLUSTERED 
(
	[model_id] ASC,
	[param_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* molecular entities table added 1 or 2 new fields?? */
--TODO: need to add [species_type_id]??? same as current [type_id] field???
--ALTER TABLE [molecular_entities] ADD [compartment_id] [uniqueidentifier] NULL;
--ALTER TABLE [molecular_entities] ADD [species_type_id] [uniqueidentifier] NULL;

--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[molecular_entities]') AND type in (N'U'))
--BEGIN
--CREATE TABLE [molecular_entities](
--	[id] [uniqueidentifier] NOT NULL,
--	[name] [varchar](50) NOT NULL,
--	[species_type_id] [uniqueidentifier] NOT NULL,
--	[compartment_id] [uniqueidentifier] NOT NULL,
-- CONSTRAINT [PK_molecular_entities] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]
--END
--GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rule_Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [Rule_Argument](
	[rule_id] [uniqueidentifier] NOT NULL,
	[arg_id] [uniqueidentifier] NOT NULL,
	[RightHandSide] [bit] NOT NULL,
 CONSTRAINT [PK_Rule_Argument] PRIMARY KEY CLUSTERED 
(
	[rule_id] ASC,
	[arg_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Assignment_Rule]') AND type in (N'U'))
BEGIN
CREATE TABLE [Assignment_Rule](
	[id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Assignment_Rule] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Model_Process]') AND type in (N'U'))
BEGIN
CREATE TABLE [Model_Process](
	[model_id] [uniqueidentifier] NOT NULL,
	[process_id] [uniqueidentifier] NOT NULL,
	[kinetic_id] [uniqueidentifier] NOT NULL,
	[notes] [varchar](50) NULL,
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[model_id] ASC,
	[process_id] ASC,
	[kinetic_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[KineticLaw_Argument]') AND type in (N'U'))
BEGIN
CREATE TABLE [KineticLaw_Argument](
	[law_id] [uniqueidentifier] NOT NULL,
	[arg_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_KineticLaw_Argument] PRIMARY KEY CLUSTERED 
(
	[law_id] ASC,
	[arg_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Variation]') AND type in (N'U'))
BEGIN
CREATE TABLE [Variation](
	[id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[notes] [varchar](50) NULL,
	[model_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Variation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rule]') AND type in (N'U'))
BEGIN
CREATE TABLE [Rule](
	[id] [uniqueidentifier] NOT NULL,
	[rule_type_id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[math_expression] [xml] NOT NULL,
	[model_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Rule] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Model_Hierarchy]') AND type in (N'U'))
BEGIN
CREATE TABLE [Model_Hierarchy](
	[parent_model_id] [uniqueidentifier] NOT NULL,
	[child_model_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Model_Hierarchy] PRIMARY KEY CLUSTERED 
(
	[parent_model_id] ASC,
	[child_model_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Model_Event]') AND type in (N'U'))
BEGIN
CREATE TABLE [Model_Event](
	[model_id] [uniqueidentifier] NOT NULL,
	[event_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Model_Event] PRIMARY KEY CLUSTERED 
(
	[model_id] ASC,
	[event_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Event_Trigger]') AND parent_object_id = OBJECT_ID(N'[Event]'))
ALTER TABLE [Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_Trigger] FOREIGN KEY([trigger_id])
REFERENCES [Trigger] ([id])
GO
ALTER TABLE [Event] CHECK CONSTRAINT [FK_Event_Trigger]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Trigger_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[Trigger_Argument]'))
ALTER TABLE [Trigger_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Trigger_Argument_Argument] FOREIGN KEY([arg_id])
REFERENCES [Argument] ([id])
GO
ALTER TABLE [Trigger_Argument] CHECK CONSTRAINT [FK_Trigger_Argument_Argument]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Trigger_Argument_Trigger]') AND parent_object_id = OBJECT_ID(N'[Trigger_Argument]'))
ALTER TABLE [Trigger_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Trigger_Argument_Trigger] FOREIGN KEY([trigger_id])
REFERENCES [Trigger] ([id])
GO
ALTER TABLE [Trigger_Argument] CHECK CONSTRAINT [FK_Trigger_Argument_Trigger]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Event_Trigger_Rule_Delay_Assignment_Rule]') AND parent_object_id = OBJECT_ID(N'[Event_Trigger_Rule_Delay]'))
ALTER TABLE [Event_Trigger_Rule_Delay]  WITH CHECK ADD  CONSTRAINT [FK_Event_Trigger_Rule_Delay_Assignment_Rule] FOREIGN KEY([assigment_rule_id])
REFERENCES [Assignment_Rule] ([id])
GO
ALTER TABLE [Event_Trigger_Rule_Delay] CHECK CONSTRAINT [FK_Event_Trigger_Rule_Delay_Assignment_Rule]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Event_Trigger_Rule_Delay_Delay]') AND parent_object_id = OBJECT_ID(N'[Event_Trigger_Rule_Delay]'))
ALTER TABLE [Event_Trigger_Rule_Delay]  WITH CHECK ADD  CONSTRAINT [FK_Event_Trigger_Rule_Delay_Delay] FOREIGN KEY([delay_id])
REFERENCES [Delay] ([id])
GO
ALTER TABLE [Event_Trigger_Rule_Delay] CHECK CONSTRAINT [FK_Event_Trigger_Rule_Delay_Delay]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Event_Trigger_Rule_Delay_Event]') AND parent_object_id = OBJECT_ID(N'[Event_Trigger_Rule_Delay]'))
ALTER TABLE [Event_Trigger_Rule_Delay]  WITH CHECK ADD  CONSTRAINT [FK_Event_Trigger_Rule_Delay_Event] FOREIGN KEY([event_id])
REFERENCES [Event] ([id])
GO
ALTER TABLE [Event_Trigger_Rule_Delay] CHECK CONSTRAINT [FK_Event_Trigger_Rule_Delay_Event]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Delay_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[Delay_Argument]'))
ALTER TABLE [Delay_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Delay_Argument_Argument] FOREIGN KEY([arg_id])
REFERENCES [Argument] ([id])
GO
ALTER TABLE [Delay_Argument] CHECK CONSTRAINT [FK_Delay_Argument_Argument]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Delay_Argument_Delay]') AND parent_object_id = OBJECT_ID(N'[Delay_Argument]'))
ALTER TABLE [Delay_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Delay_Argument_Delay] FOREIGN KEY([delay_id])
REFERENCES [Delay] ([id])
GO
ALTER TABLE [Delay_Argument] CHECK CONSTRAINT [FK_Delay_Argument_Delay]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Pathway_Model_Model]') AND parent_object_id = OBJECT_ID(N'[Pathway_Model]'))
ALTER TABLE [Pathway_Model]  WITH CHECK ADD  CONSTRAINT [FK_Pathway_Model_Model] FOREIGN KEY([model_id])
REFERENCES [Model] ([id])
GO
ALTER TABLE [Pathway_Model] CHECK CONSTRAINT [FK_Pathway_Model_Model]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Pathway_Model_Pathway]') AND parent_object_id = OBJECT_ID(N'[Pathway_Model]'))
ALTER TABLE [Pathway_Model]  WITH CHECK ADD  CONSTRAINT [FK_Pathway_Model_Pathway] FOREIGN KEY([pathway_id])
REFERENCES [Pathways] ([id])
GO
ALTER TABLE [Pathway_Model] CHECK CONSTRAINT [FK_Pathway_Model_Pathway]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Has_Function_Function]') AND parent_object_id = OBJECT_ID(N'[Model_Function]'))
ALTER TABLE [Model_Function]  WITH CHECK ADD  CONSTRAINT [FK_Has_Function_Function] FOREIGN KEY([function_id])
REFERENCES [Function] ([id])
GO
ALTER TABLE [Model_Function] CHECK CONSTRAINT [FK_Has_Function_Function]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Has_Function_Model]') AND parent_object_id = OBJECT_ID(N'[Model_Function]'))
ALTER TABLE [Model_Function]  WITH CHECK ADD  CONSTRAINT [FK_Has_Function_Model] FOREIGN KEY([model_id])
REFERENCES [Model] ([id])
GO
ALTER TABLE [Model_Function] CHECK CONSTRAINT [FK_Has_Function_Model]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Function_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[Function_Argument]'))
ALTER TABLE [Function_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Function_Argument_Argument] FOREIGN KEY([arg_id])
REFERENCES [Argument] ([id])
GO
ALTER TABLE [Function_Argument] CHECK CONSTRAINT [FK_Function_Argument_Argument]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Function_Argument_Function]') AND parent_object_id = OBJECT_ID(N'[Function_Argument]'))
ALTER TABLE [Function_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Function_Argument_Function] FOREIGN KEY([func_id])
REFERENCES [Function] ([id])
GO
ALTER TABLE [Function_Argument] CHECK CONSTRAINT [FK_Function_Argument_Function]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Variation_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[Variation_Argument]'))
ALTER TABLE [Variation_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Variation_Argument_Argument] FOREIGN KEY([arg_id])
REFERENCES [Argument] ([id])
GO
ALTER TABLE [Variation_Argument] CHECK CONSTRAINT [FK_Variation_Argument_Argument]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Variation_Argument_Variation]') AND parent_object_id = OBJECT_ID(N'[Variation_Argument]'))
ALTER TABLE [Variation_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Variation_Argument_Variation] FOREIGN KEY([var_id])
REFERENCES [Variation] ([id])
GO
ALTER TABLE [Variation_Argument] CHECK CONSTRAINT [FK_Variation_Argument_Variation]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[Argument]'))
ALTER TABLE [Argument]  WITH CHECK ADD  CONSTRAINT [FK_Argument_Argument] FOREIGN KEY([molecule_id])
REFERENCES [molecular_entities] ([id])
GO
ALTER TABLE [Argument] CHECK CONSTRAINT [FK_Argument_Argument]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Argument_Compartment]') AND parent_object_id = OBJECT_ID(N'[Argument]'))
ALTER TABLE [Argument]  WITH CHECK ADD  CONSTRAINT [FK_Argument_Compartment] FOREIGN KEY([compartment_id])
REFERENCES [Compartment] ([id])
GO
ALTER TABLE [Argument] CHECK CONSTRAINT [FK_Argument_Compartment]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Argument_Parameter]') AND parent_object_id = OBJECT_ID(N'[Argument]'))
ALTER TABLE [Argument]  WITH CHECK ADD  CONSTRAINT [FK_Argument_Parameter] FOREIGN KEY([param_id])
REFERENCES [Parameter] ([id])
GO
ALTER TABLE [Argument] CHECK CONSTRAINT [FK_Argument_Parameter]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Argument_Unit]') AND parent_object_id = OBJECT_ID(N'[Argument]'))
ALTER TABLE [Argument]  WITH CHECK ADD  CONSTRAINT [FK_Argument_Unit] FOREIGN KEY([unit_id])
REFERENCES [Unit] ([id])
GO
ALTER TABLE [Argument] CHECK CONSTRAINT [FK_Argument_Unit]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_process_entities_molecular_entities]') AND parent_object_id = OBJECT_ID(N'[process_entities]'))
ALTER TABLE [process_entities]  WITH CHECK ADD  CONSTRAINT [FK_process_entities_molecular_entities] FOREIGN KEY([molecule_id])
REFERENCES [molecular_entities] ([id])
GO
ALTER TABLE [process_entities] CHECK CONSTRAINT [FK_process_entities_molecular_entities]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_process_entities_Process]') AND parent_object_id = OBJECT_ID(N'[process_entities]'))
ALTER TABLE [process_entities]  WITH CHECK ADD  CONSTRAINT [FK_process_entities_Process] FOREIGN KEY([process_id])
REFERENCES [Processes] ([id])
GO
ALTER TABLE [process_entities] CHECK CONSTRAINT [FK_process_entities_Process]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Compartment_Compartment]') AND parent_object_id = OBJECT_ID(N'[Compartment]'))
ALTER TABLE [Compartment]  WITH CHECK ADD  CONSTRAINT [FK_Compartment_Compartment] FOREIGN KEY([parent_comp_id])
REFERENCES [Compartment] ([id])
GO
ALTER TABLE [Compartment] CHECK CONSTRAINT [FK_Compartment_Compartment]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Compartment_Compartment_Type]') AND parent_object_id = OBJECT_ID(N'[Compartment]'))
ALTER TABLE [Compartment]  WITH CHECK ADD  CONSTRAINT [FK_Compartment_Compartment_Type] FOREIGN KEY([comp_type_id])
REFERENCES [Compartment_Type] ([id])
GO
ALTER TABLE [Compartment] CHECK CONSTRAINT [FK_Compartment_Compartment_Type]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Composed_Units_Unit]') AND parent_object_id = OBJECT_ID(N'[Composed_Units]'))
ALTER TABLE [Composed_Units]  WITH CHECK ADD  CONSTRAINT [FK_Composed_Units_Unit] FOREIGN KEY([generated_unit_id])
REFERENCES [Unit] ([id])
GO
ALTER TABLE [Composed_Units] CHECK CONSTRAINT [FK_Composed_Units_Unit]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Composed_Units_Unit1]') AND parent_object_id = OBJECT_ID(N'[Composed_Units]'))
ALTER TABLE [Composed_Units]  WITH CHECK ADD  CONSTRAINT [FK_Composed_Units_Unit1] FOREIGN KEY([used_unit_id])
REFERENCES [Unit] ([id])
GO
ALTER TABLE [Composed_Units] CHECK CONSTRAINT [FK_Composed_Units_Unit1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Model_Parameter_Model]') AND parent_object_id = OBJECT_ID(N'[Model_Parameter]'))
ALTER TABLE [Model_Parameter]  WITH CHECK ADD  CONSTRAINT [FK_Model_Parameter_Model] FOREIGN KEY([model_id])
REFERENCES [Model] ([id])
GO
ALTER TABLE [Model_Parameter] CHECK CONSTRAINT [FK_Model_Parameter_Model]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Model_Parameter_Parameter]') AND parent_object_id = OBJECT_ID(N'[Model_Parameter]'))
ALTER TABLE [Model_Parameter]  WITH CHECK ADD  CONSTRAINT [FK_Model_Parameter_Parameter] FOREIGN KEY([param_id])
REFERENCES [Parameter] ([id])
GO
ALTER TABLE [Model_Parameter] CHECK CONSTRAINT [FK_Model_Parameter_Parameter]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_molecular_entities_Compartment]') AND parent_object_id = OBJECT_ID(N'[molecular_entities]'))
ALTER TABLE [molecular_entities]  WITH CHECK ADD  CONSTRAINT [FK_molecular_entities_Compartment] FOREIGN KEY([compartment_id])
REFERENCES [Compartment] ([id])
GO
ALTER TABLE [molecular_entities] CHECK CONSTRAINT [FK_molecular_entities_Compartment]
GO
--IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Molecular_Entity_Molecular_Entity_Type]') AND parent_object_id = OBJECT_ID(N'[Molecular_Entity]'))
--ALTER TABLE [Molecular_Entity]  WITH CHECK ADD  CONSTRAINT [FK_Molecular_Entity_Molecular_Entity_Type] FOREIGN KEY([species_type_id])
--REFERENCES [Molecular_Entity_Type] ([id])
--GO
--ALTER TABLE [molecular_entities] CHECK CONSTRAINT [FK_molecular_entities_molecular_entities_Type]
--GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Rule_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[Rule_Argument]'))
ALTER TABLE [Rule_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Rule_Argument_Argument] FOREIGN KEY([arg_id])
REFERENCES [Argument] ([id])
GO
ALTER TABLE [Rule_Argument] CHECK CONSTRAINT [FK_Rule_Argument_Argument]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Rule_Argument_Rule]') AND parent_object_id = OBJECT_ID(N'[Rule_Argument]'))
ALTER TABLE [Rule_Argument]  WITH CHECK ADD  CONSTRAINT [FK_Rule_Argument_Rule] FOREIGN KEY([rule_id])
REFERENCES [Rule] ([id])
GO
ALTER TABLE [Rule_Argument] CHECK CONSTRAINT [FK_Rule_Argument_Rule]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Assignment_Rule_Rule]') AND parent_object_id = OBJECT_ID(N'[Assignment_Rule]'))
ALTER TABLE [Assignment_Rule]  WITH CHECK ADD  CONSTRAINT [FK_Assignment_Rule_Rule] FOREIGN KEY([id])
REFERENCES [Rule] ([id])
GO
ALTER TABLE [Assignment_Rule] CHECK CONSTRAINT [FK_Assignment_Rule_Rule]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Model_Process_Kinetic_Law]') AND parent_object_id = OBJECT_ID(N'[Model_Process]'))
ALTER TABLE [Model_Process]  WITH CHECK ADD  CONSTRAINT [FK_Model_Process_Kinetic_Law] FOREIGN KEY([kinetic_id])
REFERENCES [Kinetic_Law] ([id])
GO
ALTER TABLE [Model_Process] CHECK CONSTRAINT [FK_Model_Process_Kinetic_Law]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Model_Process_Model]') AND parent_object_id = OBJECT_ID(N'[Model_Process]'))
ALTER TABLE [Model_Process]  WITH CHECK ADD  CONSTRAINT [FK_Model_Process_Model] FOREIGN KEY([model_id])
REFERENCES [Model] ([id])
GO
ALTER TABLE [Model_Process] CHECK CONSTRAINT [FK_Model_Process_Model]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Model_Process_Process]') AND parent_object_id = OBJECT_ID(N'[Model_Process]'))
ALTER TABLE [Model_Process]  WITH CHECK ADD  CONSTRAINT [FK_Model_Process_Process] FOREIGN KEY([process_id])
REFERENCES [Processes] ([id])
GO
ALTER TABLE [Model_Process] CHECK CONSTRAINT [FK_Model_Process_Process]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_KineticLaw_Argument_Argument]') AND parent_object_id = OBJECT_ID(N'[KineticLaw_Argument]'))
ALTER TABLE [KineticLaw_Argument]  WITH CHECK ADD  CONSTRAINT [FK_KineticLaw_Argument_Argument] FOREIGN KEY([arg_id])
REFERENCES [Argument] ([id])
GO
ALTER TABLE [KineticLaw_Argument] CHECK CONSTRAINT [FK_KineticLaw_Argument_Argument]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_KineticLaw_Argument_Kinetic_law]') AND parent_object_id = OBJECT_ID(N'[KineticLaw_Argument]'))
ALTER TABLE [KineticLaw_Argument]  WITH CHECK ADD  CONSTRAINT [FK_KineticLaw_Argument_Kinetic_law] FOREIGN KEY([law_id])
REFERENCES [Kinetic_Law] ([id])
GO
ALTER TABLE [KineticLaw_Argument] CHECK CONSTRAINT [FK_KineticLaw_Argument_Kinetic_law]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Variation_Model]') AND parent_object_id = OBJECT_ID(N'[Variation]'))
ALTER TABLE [Variation]  WITH CHECK ADD  CONSTRAINT [FK_Variation_Model] FOREIGN KEY([model_id])
REFERENCES [Model] ([id])
GO
ALTER TABLE [Variation] CHECK CONSTRAINT [FK_Variation_Model]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Rule_Model]') AND parent_object_id = OBJECT_ID(N'[Rule]'))
ALTER TABLE [Rule]  WITH CHECK ADD  CONSTRAINT [FK_Rule_Model] FOREIGN KEY([model_id])
REFERENCES [Model] ([id])
GO
ALTER TABLE [Rule] CHECK CONSTRAINT [FK_Rule_Model]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Rule_Rule_Type]') AND parent_object_id = OBJECT_ID(N'[Rule]'))
ALTER TABLE [Rule]  WITH CHECK ADD  CONSTRAINT [FK_Rule_Rule_Type] FOREIGN KEY([rule_type_id])
REFERENCES [Rule_Type] ([id])
GO
ALTER TABLE [Rule] CHECK CONSTRAINT [FK_Rule_Rule_Type]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Model_Hierarchy_Model]') AND parent_object_id = OBJECT_ID(N'[Model_Hierarchy]'))
ALTER TABLE [Model_Hierarchy]  WITH CHECK ADD  CONSTRAINT [FK_Model_Hierarchy_Model] FOREIGN KEY([child_model_id])
REFERENCES [Model] ([id])
GO
ALTER TABLE [Model_Hierarchy] CHECK CONSTRAINT [FK_Model_Hierarchy_Model]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Model_Hierarchy_Model_Hierarchy]') AND parent_object_id = OBJECT_ID(N'[Model_Hierarchy]'))
ALTER TABLE [Model_Hierarchy]  WITH CHECK ADD  CONSTRAINT [FK_Model_Hierarchy_Model_Hierarchy] FOREIGN KEY([parent_model_id])
REFERENCES [Model] ([id])
GO
ALTER TABLE [Model_Hierarchy] CHECK CONSTRAINT [FK_Model_Hierarchy_Model_Hierarchy]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Model_Event_Event]') AND parent_object_id = OBJECT_ID(N'[Model_Event]'))
ALTER TABLE [Model_Event]  WITH CHECK ADD  CONSTRAINT [FK_Model_Event_Event] FOREIGN KEY([event_id])
REFERENCES [Event] ([id])
GO
ALTER TABLE [Model_Event] CHECK CONSTRAINT [FK_Model_Event_Event]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Model_Event_Model]') AND parent_object_id = OBJECT_ID(N'[Model_Event]'))
ALTER TABLE [Model_Event]  WITH CHECK ADD  CONSTRAINT [FK_Model_Event_Model] FOREIGN KEY([model_id])
REFERENCES [Model] ([id])
GO
ALTER TABLE [Model_Event] CHECK CONSTRAINT [FK_Model_Event_Model]
