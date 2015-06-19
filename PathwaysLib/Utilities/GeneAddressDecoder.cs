using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;
using PathwaysLib.SoapObjects;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace PathwaysLib.Utilities
{
    public class GeneAddressDecoder
    {

        static StreamWriter logFile;
        /// <summary>
        /// Given an organism, for each gene in the organism, decodes the address in the raw_address field
        /// into either cytogenetic address or genetic address field.
        /// Populates the relative address and chromosome id fields of each gene.
        /// </summary>
        /// <param name="organism"></param>
        public static void DecodeGenesOfOrganism(ServerOrganismGroup organism)
        {
            logFile = new StreamWriter("..//..//GeneDecoder.Log." + organism.CommonName + ".txt");
            // First, load the chromosomes of the organism...
            // Assumption: Each chromosome has a unique name (which is a reasonable assumption)
            ServerChromosome[] chromosomes = ServerChromosome.GetAllChromosomesForOrganism(organism.ID);
            Dictionary<string, ServerChromosome> chromosomeDict = new Dictionary<string, ServerChromosome>();

            foreach (ServerChromosome ch in chromosomes)
                chromosomeDict.Add(ch.Name, ch);

            //Get the cM Unit Length for the organism for linkage map address
            SqlCommand command = DBWrapper.BuildCommand(@"select cm_unit_length from organisms
                                                          where id=@orgId",
                                                          "@orgId", SqlDbType.UniqueIdentifier, organism.ID);
            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            int cMUnit = int.Parse(ds.Tables[0].Rows[0][0].ToString());

            //Load all genes
            ServerGene[] genes = ServerGene.GetAllGenesForOrganism(organism.ID);
            string chromosomeName, cytogeneticAddress;
            long relativeAddress, geneticAddress;

            int i = 0;
            foreach (ServerGene gene in genes)
            {
                i++;
                if (gene.RawAddress.Trim().Length == 0)
                    continue;
                //Console.Write(i + ": ");
                DecodeGeneAddress(gene.RawAddress, out chromosomeName, out relativeAddress, out cytogeneticAddress, out geneticAddress, chromosomeDict, cMUnit);                
                gene.RelativeAddress = relativeAddress;
                gene.CytogenicAddress = cytogeneticAddress;
                
                if(relativeAddress > -1)
                    gene.ChromosomeID = chromosomeDict[chromosomeName].ID;
                gene.UpdateDatabase();
            }
            logFile.WriteLine("\n\nTotal Processed Genes for organism {0} : {1}", organism.CommonName, i);
            logFile.Close();
        }

        /*********************************************************************************************************
         * Decodes different kinds of genetic addresses and extracts chromosome number and relative address
         * 
         * Currently can handle gene addresses in the following formats:
         *  - human cytogenetic addresses like 21q22.11
         *  - mouse cytogenetic addresses like 16 A1.1
         *  - linkage map addresses like 6 35.15 cM
         *  - genome sequence intervals like X:24931..25419
         *  - single chromosome number like 5, 11, X, Y, etc.
         * ********************************************************************************************************/
        /// <summary>
        /// Decodes different kinds of genetic addresses and extracts chromosome number and relative address
        /// </summary>
        /// <param name="rawAddress"></param>
        /// <param name="chromosomeName"></param>
        /// <param name="relativeAddress"></param>
        /// <param name="cytoGenAddress"></param>
        /// <param name="chromosomeDict"></param>
        private static void DecodeGeneAddress(string rawAddress, out string chromosomeName, out long relativeAddress, out string cytoGenAddress, out long geneticAddress, Dictionary<string, ServerChromosome> chromosomeDict, int cMUnit)
        {
            Regex humanCytogeneticAddress = new Regex("([XY]|[1-9][0-9]?)([pq]|cen|ter)"); // e.g., 21q22.11, 12q12-q14, 9cen-q34, 11q13.3-q13.5
            Regex mouseCytogeneticAddress = new Regex("([XY]|[1-9][0-9]?)\\s?([A-H]|centromere)[1-9]?(\\.[1-9])?"); // e.g., 16 A1.1, 16 C3-qter, 6 F1-pter, 16 cen-C3, 19 centromere 13 B-C2, 1 F
            /*
             * Since there are some differences between human and mouse cytogenetic address scheme, 
             * we decided to handle them separately in order to avoid a complicated, hard to understand
             * code piece that can decode all kinds of address schemes within the same code.
             * */
            Regex linkageMapAddress = new Regex("([XY]|[1-9][0-9]?)\\s[0-9]{1,3}\\.[0-9]{1,3}\\scM"); //e.g., 6 35.15 cM 
            Regex sequenceInterval = new Regex("[0-9a-zA-Z]\\s?:\\s?[0-9]+\\.\\.[0-9]+"); //e.g., X:24931..25419
            Regex singleChromosomeNumber = new Regex("[XY]|([1-9][0-9]?)"); //e.g., 1, 2, 24, 99
            Regex mitochondrialGene = new Regex("MT"); //e.g., MT
            
            Regex addressSeparator = new Regex("(;|:|and|or)(\\s)?");

            relativeAddress = -1;
            geneticAddress = -1;
            chromosomeName = "NA";
            cytoGenAddress = "NA";

            if (humanCytogeneticAddress.IsMatch(rawAddress)) // process cytogenetic address
            {
                //Console.WriteLine("Decoding Human Cytogenetic Address: " + rawAddress);

                Regex chromosome = new Regex("([XY]|[1-9][0-9]?)(?=([pq]|cen|ter))");
                Regex armRegex = new Regex("[pq]|cen|ter");                              

                if (addressSeparator.IsMatch(rawAddress)) // if there are multiple alternative addresses, simply adopt the first oen as the gene address
                {                  
                    MatchCollection matches = addressSeparator.Matches(rawAddress);
                    string longestAddress = "";
                    int startIndex = 0;
                    foreach (Match mc in matches)
                    {
                        if (mc.Index - startIndex > longestAddress.Length && humanCytogeneticAddress.IsMatch(rawAddress.Substring(startIndex, mc.Index - startIndex)))                        
                            longestAddress = rawAddress.Substring(startIndex, mc.Index - startIndex);
                        
                        startIndex = mc.Index + mc.Value.Length;
                    }
                    if (rawAddress.Length - startIndex > longestAddress.Length && humanCytogeneticAddress.IsMatch(rawAddress.Substring(startIndex)))
                        longestAddress = rawAddress.Substring(startIndex);

                    rawAddress = longestAddress;
                }

                cytoGenAddress = rawAddress;

                Match match = chromosome.Match(rawAddress);
                chromosomeName = match.Value;

                string startBand, endBand = null, chArm, chBand;
                startBand = rawAddress.Substring(chromosomeName.Length);
                if (startBand.IndexOf("-") > 0)
                {
                    endBand = startBand.Substring(startBand.IndexOf("-")+1);
                    startBand = startBand.Substring(0, startBand.IndexOf("-"));
                }

                chArm = armRegex.Match(startBand).Value;
                if (startBand.IndexOf(chArm) + chArm.Length < startBand.Length)
                    chBand = startBand.Substring(startBand.IndexOf(chArm) + chArm.Length);
                else
                    chBand = "";

                Guid chromosomeId = chromosomeDict[chromosomeName].ID;
                int startBP = -1, endBP = -1;

                FetchBandStartEndBP(chromosomeId, chArm, chBand, ref startBP, ref endBP);

                // check if this is a range type of address
                if (endBand != null)
                {
                    if (armRegex.IsMatch(endBand))
                    {
                        chArm = armRegex.Match(endBand).Value;
                        if (endBand.IndexOf(chArm) + chArm.Length < endBand.Length)
                            chBand = endBand.Substring(endBand.IndexOf(chArm) + chArm.Length);
                        else
                            chBand = "";
                    }
                    else
                        chBand = endBand;

                    int tempStartBP = -1;
                    FetchBandStartEndBP(chromosomeId, chArm, chBand, ref tempStartBP, ref endBP);                                  
                }

                // Relattive address is computed as the middle point of the region defined by the bands
                relativeAddress = (startBP + endBP) / 2;

            }
            else if (mouseCytogeneticAddress.IsMatch(rawAddress)) // process mouse cytogenetic address
            {
                //Console.WriteLine("Decoding Mouse Cytogenetic Address: " + rawAddress);
                // e.g., 16 A1.1, 16 C3-qter, 6 F1-pter, 16 cen-C3, 19 centromere
                // 13 B-C2, 1 F
                Regex chromosome = new Regex("([XY]|[1-9][0-9]?)(?=\\s?([A-H]|centromere)[1-9]?(\\.[1-9])?)");
                Regex armRegex = new Regex("[pq]|centromere|cen");

                if (addressSeparator.IsMatch(rawAddress)) // if there are multiple alternative addresses, simply adopt the first oen as the gene address
                {
                    MatchCollection matches = addressSeparator.Matches(rawAddress);
                    string longestAddress = "";
                    int startIndex = 0;
                    foreach (Match mc in matches)
                    {
                        if (mc.Index - startIndex > longestAddress.Length && mouseCytogeneticAddress.IsMatch(rawAddress.Substring(startIndex, mc.Index - startIndex)))
                            longestAddress = rawAddress.Substring(startIndex, mc.Index - startIndex);

                        startIndex = mc.Index + mc.Value.Length;
                    }
                    if (rawAddress.Length - startIndex > longestAddress.Length && mouseCytogeneticAddress.IsMatch(rawAddress.Substring(startIndex)))
                        longestAddress = rawAddress.Substring(startIndex);

                    rawAddress = longestAddress;
                }

                cytoGenAddress = rawAddress;

                Match match = chromosome.Match(rawAddress);
                chromosomeName = match.Value;

                string startBand, endBand = null, chArm, chBand;
                startBand = rawAddress.Substring(chromosomeName.Length);
                if (startBand.IndexOf("-") > 0)
                {
                    endBand = startBand.Substring(startBand.IndexOf("-") + 1);
                    startBand = startBand.Substring(0, startBand.IndexOf("-"));
                }

                if (armRegex.IsMatch(startBand))
                {
                    chArm = armRegex.Match(startBand).Value;
                    if (startBand.IndexOf(chArm) + chArm.Length < startBand.Length)
                        chBand = startBand.Substring(startBand.IndexOf(chArm) + chArm.Length);
                    else
                        chBand = "";
                    
                    if(chArm == "centromere")
                        chArm = "cen";
                }
                else
                {
                    chArm = "q";
                    chBand = startBand.Trim();
                }

                Guid chromosomeId = chromosomeDict[chromosomeName].ID;
                int startBP = -1, endBP = -1;

                FetchBandStartEndBP(chromosomeId, chArm, chBand, ref startBP, ref endBP);

                // check if this is a range type of address
                if (endBand != null)
                {
                    if (armRegex.IsMatch(endBand))
                    {
                        chArm = armRegex.Match(endBand).Value;
                        if (endBand.IndexOf(chArm) + chArm.Length < endBand.Length)
                            chBand = endBand.Substring(endBand.IndexOf(chArm) + chArm.Length);
                        else
                            chBand = "";

                        if (chArm == "centromere")
                            chArm = "cen";
                    }
                    else
                    {
                        chArm = "q";
                        chBand = endBand;
                    }

                    int startBP2 = -1, endBP2 = -1;
                    FetchBandStartEndBP(chromosomeId, chArm, chBand, ref startBP2, ref endBP2);
                    if (startBP2 > -1 && startBP2 < startBP)
                        startBP = startBP2;
                    else if(endBP2 > -1)
                        endBP = endBP2;
                }

                // Relattive address is computed as the middle point of the region defined by the bands
                relativeAddress = (startBP + endBP) / 2;

            }
            else if (linkageMapAddress.IsMatch(rawAddress)) // process linkage map address
            {
                //Console.WriteLine("Decoding Linkage Map Address: " + rawAddress);
                /*  6 35.85 cM
                 *    begin -- we assume that 1 cM is approximately 1 million base pairs
	                    declare @loci float
	                    declare @cM_unit int
	                    exec getLinkageMapRegion @caddr, @loci OUTPUT
	                    select @cM_unit=cM_unit_length from organisms where id=@orgId 
	                    set @reladdr = @loci * @cM_unit
	                    select @reladdr
                       end
                 * */

                if (addressSeparator.IsMatch(rawAddress)) // if there are multiple alternative addresses, simply adopt the first oen as the gene address
                {
                    MatchCollection matches = addressSeparator.Matches(rawAddress);
                    string longestAddress = "";
                    int startIndex = 0;
                    foreach (Match mc in matches)
                    {
                        if (mc.Index - startIndex > longestAddress.Length && linkageMapAddress.IsMatch(rawAddress.Substring(startIndex, mc.Index - startIndex)))
                            longestAddress = rawAddress.Substring(startIndex, mc.Index - startIndex);

                        startIndex = mc.Index + mc.Value.Length;
                    }
                    if (rawAddress.Length - startIndex > longestAddress.Length && linkageMapAddress.IsMatch(rawAddress.Substring(startIndex)))
                        longestAddress = rawAddress.Substring(startIndex);

                    rawAddress = longestAddress;
                }

                cytoGenAddress = rawAddress;
                
                Regex chromosome = new Regex("([XY]|[1-9][0-9]?)(?=\\s[0-9]{1,3}\\.[0-9]{1,3}\\scM)");
                chromosomeName = chromosome.Match(rawAddress).Value;
                float cMCount = float.Parse(rawAddress.Substring(rawAddress.IndexOf(" ") + 1, rawAddress.IndexOf("cM") - rawAddress.IndexOf(" ") - 2).Trim());
                relativeAddress = (long)Math.Ceiling(cMCount * cMUnit);            
            }
            else if (sequenceInterval.IsMatch(rawAddress)) // process genome sequence interval address
            {
                //Console.WriteLine("Decoding Genome Sequence Interval Address: " + rawAddress);
                if (addressSeparator.IsMatch(rawAddress)) // if there are multiple alternative addresses, simply adopt the first oen as the gene address
                {
                    MatchCollection matches = addressSeparator.Matches(rawAddress);
                    string longestAddress = "";
                    int startIndex = 0;
                    foreach (Match mc in matches)
                    {
                        if (mc.Index - startIndex > longestAddress.Length && sequenceInterval.IsMatch(rawAddress.Substring(startIndex, mc.Index - startIndex)))
                            longestAddress = rawAddress.Substring(startIndex, mc.Index - startIndex);

                        startIndex = mc.Index + mc.Value.Length;
                    }
                    if (rawAddress.Length - startIndex > longestAddress.Length && sequenceInterval.IsMatch(rawAddress.Substring(startIndex)))
                        longestAddress = rawAddress.Substring(startIndex);

                    rawAddress = longestAddress;
                }                                     
                Regex chromosome = new Regex("([0-9a-zA-Z]+)(?=\\s?:\\s?[0-9]+\\.\\.[0-9]+)");
                chromosomeName = chromosome.Match(rawAddress).Value;
                int startBP = int.Parse(rawAddress.Substring(rawAddress.IndexOf(":") + 1, rawAddress.IndexOf("..") - rawAddress.IndexOf(":") - 1).Trim());
                int endBP = int.Parse(rawAddress.Substring(rawAddress.IndexOf("..") + 2).Trim());
                geneticAddress = startBP;
                relativeAddress = startBP + endBP / 2;
            }
            else if (singleChromosomeNumber.IsMatch(rawAddress)) // process addresses which just contain a single chromosome number
            {
                if (addressSeparator.IsMatch(rawAddress)) // if there are multiple alternative addresses, simply adopt the first one
                {
                    bool foundValidAddress = false;
                    int startIndex = 0;
                    MatchCollection matches = addressSeparator.Matches(rawAddress);
                    foreach (Match mc in matches)
                    {
                        if (singleChromosomeNumber.IsMatch(rawAddress.Substring(startIndex, mc.Index - startIndex)))
                        {
                            rawAddress = rawAddress.Substring(startIndex, mc.Index - startIndex).Trim();
                            foundValidAddress = true;
                            break;
                        }
                        startIndex = mc.Index + mc.Value.Length;
                    }
                    if (!foundValidAddress)
                        rawAddress = rawAddress.Substring(startIndex).Trim();                    
                }
                // For such cases, we just assign the center of the chromosome as the location of that gene
                relativeAddress = (int)chromosomeDict[rawAddress].Length / 2;
                cytoGenAddress = rawAddress;
                chromosomeName = cytoGenAddress;
            }
            else if (mitochondrialGene.IsMatch(rawAddress))
            {
                Console.WriteLine("Mitochondrial gene (was not processed): " + rawAddress);
                logFile.WriteLine("Mitochondrial gene (was not processed): " + rawAddress);
            }
            else
            {
                //throw new Exception("Unrecognized gene address format: " + rawAddress);
                Console.WriteLine("Unrecognized gene address format: " + rawAddress);
                logFile.WriteLine("Unrecognized gene address format: " + rawAddress);
            }
        }

        private static void FetchBandStartEndBP(Guid chromosomeId, string chArm, string chBand, ref int startBP, ref int endBP)
        {
            string query = @"select bp_start, bp_stop
                                 from chromosome_bands
		                         where chromosome_id = @cid and arm=@arm";

            SqlCommand command = null;
            if (chBand.Length > 0)
            {
                query += " and band = @band;";
                command = DBWrapper.BuildCommand(query,
                                                 "@cid", SqlDbType.UniqueIdentifier, chromosomeId,
                                                 "@arm", SqlDbType.VarChar, chArm,
                                                 "@band", SqlDbType.VarChar, chBand);
            }
            else
            {
                query += ";";
                command = DBWrapper.BuildCommand(query,
                                                 "@cid", SqlDbType.UniqueIdentifier, chromosomeId,
                                                 "@arm", SqlDbType.VarChar, chArm);
            }

            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);

            if (ds.Tables[0].Rows.Count > 0)
            {
                endBP = int.Parse(ds.Tables[0].Rows[0][1].ToString());
                startBP = int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
        }

    }
}
