USE [PathCase_SystemBiology]
GO
/****** Object:  StoredProcedure [dbo].[PopulateProcessGraphNodes]    Script Date: 03/25/2008 14:17:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ali Cakmak
-- Create date: Feb 17th, 2008
-- Description:	Precomputes the content of process_graph_nodes table.
-- =============================================
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




