USE [Pathcase_SystemBiology]
GO
/****** Object:  StoredProcedure [dbo].[PopulatePathwayEntities]    Script Date: 03/25/2008 14:05:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
