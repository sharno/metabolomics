using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace PathwaysLib.Utilities
{
    public class ChromosomeBandLoader
    {
        public static void UpdateChromosomeLength()
        {            
            string query = @"declare @crs cursor
                            set @crs=cursor for
                            select distinct chromosome_id from chromosome_bands

                            declare @id uniqueidentifier
                            declare @length bigint
                            declare @cloc int

                            open @crs
                            fetch next from @crs into @id

                            while @@fetch_status=0
                            begin
	                            set @length = -1
	                            set @cloc = -1
	                            select @length=max(bp_stop) from chromosome_bands where chromosome_id=@id
	                            select @cloc=bp_start from chromosome_bands where chromosome_id=@id and arm='cen'
	                            update chromosomes set length=@length, centromere_location=@cloc where id=@id	
	                            fetch next from @crs into @id
                            end";
            SqlCommand cmd = DBWrapper.BuildCommand(query);
            DBWrapper.Instance.ExecuteNonQuery(ref cmd);
        }
        public static void CreateChromosomeBandForOrganism(ServerOrganismGroup org, StreamReader ideogramReader)
        {
            SqlCommand command = DBWrapper.BuildCommand(@"update genes set chromosome_id=null where organism_group_id=@orgId",
                                                          "@orgId", SqlDbType.UniqueIdentifier, org.ID);
            
            DBWrapper.Instance.ExecuteNonQuery(ref command);

            command = DBWrapper.BuildCommand(@"delete from chromosome_bands where chromosome_id in
                                               (select id from chromosomes where organism_group_id=@orgId)",
                                               "@orgId", SqlDbType.UniqueIdentifier, org.ID);

            DBWrapper.Instance.ExecuteNonQuery(ref command);
            
            command = DBWrapper.BuildCommand(@"delete from chromosomes where organism_group_id=@orgId",                                                          
                                                          "@orgId", SqlDbType.UniqueIdentifier, org.ID);
            
            DBWrapper.Instance.ExecuteNonQuery(ref command);

            Dictionary<string, ServerChromosome> chromosomes = new Dictionary<string, ServerChromosome>();
            ServerChromosome ch;
            char[] delim = {'\t'};
            string line = "";
            string[] tokens;
            string chromosomeName, arm, band, stain;
            float density;
            int iscnStart, iscnStop, bpStart, bpStop;
            long bases;
            ideogramReader.ReadLine();
            while ((line = ideogramReader.ReadLine()) != null)
            {
                iscnStart = 0; iscnStop = 0; bpStart = 0; bpStop = 0; bases = 0; density = 0f ;
                tokens = line.Split(delim);
                chromosomeName = tokens[0];
                arm = tokens[1];
                band = tokens[2];
                
                if (tokens[3].Trim().Length > 0)
                    iscnStart = int.Parse(tokens[3]);
                
                if (tokens[4].Trim().Length > 0)
                    iscnStop = int.Parse(tokens[4]);

                if (tokens[5].Trim().Length > 0)
                    bpStart = int.Parse(tokens[5]);

                if (tokens[6].Trim().Length > 0)
                    bpStop = int.Parse(tokens[6]);

                stain = tokens[7];

                if (tokens[8].Trim().Length > 0)
                    density = float.Parse(tokens[8]);

                if (tokens[9].Trim().Length > 0)
                    bases = long.Parse(tokens[9]);

                //Console.WriteLine(chromosomeName + "\t" + arm + "\t" + band + "\t" + iscnStart + "\t" + iscnStop + "\t" + bpStart + "\t" + bpStop + "\t" + stain + "\t" + density + "\t" + bases);
                //Console.ReadLine ();
                if (!chromosomes.TryGetValue(chromosomeName, out ch))
                {
                    ch = new ServerChromosome(chromosomeName, org.ID, -1, -1, "");
                    ch.UpdateDatabase();
                    chromosomes.Add(chromosomeName, ch);                   
                }

                command = DBWrapper.BuildCommand(@"insert into chromosome_bands values(@chId, @chName, @arm, @band, @iscnStart, @iscnStop, @bpStart, @bpStop, @stain, @density, @bases)",
                                                          "@chId", SqlDbType.UniqueIdentifier, ch.ID,
                                                          "@chName", SqlDbType.VarChar, chromosomeName,
                                                          "@arm", SqlDbType.VarChar, arm,
                                                          "@band", SqlDbType.VarChar, band,
                                                          "@iscnStart", SqlDbType.Int, iscnStart,
                                                          "@iscnStop", SqlDbType.Int, iscnStop,
                                                          "@bpStart", SqlDbType.Int, bpStart,
                                                          "@bpStop", SqlDbType.Int, bpStop,
                                                          "@stain", SqlDbType.VarChar, stain,
                                                          "@density", SqlDbType.Float, density,
                                                          "@bases", SqlDbType.BigInt, bases);
                
                DBWrapper.Instance.ExecuteNonQuery(ref command);
            }
            ideogramReader.Close();
        }
    }
}
