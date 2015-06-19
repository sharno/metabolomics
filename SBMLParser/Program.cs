#region "declaration region"
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using PathwaysLib.ServerObjects;
using PathwaysLib.SoapObjects;
using System.Configuration;
using libsbmlcs;
#endregion


/***********************************************************************************************************************************************************************************
 *     SBML Parser Instructions
 * *********************************************************************************************************************************************************************************
 * The parser can either be run within Visual Studio, or SBMLParser.exe file in ./SBMLParser/bin/debug folder can be invoked from the console screen.
 * 
 * It makes use of libSBML library to parse SBML files. Related DLL files that need to be added into the project references are located in
 *     ./SBMLParser/DLLs folder.
 *     
 * 
 * This program requires the existence and proper setting of the following parameters in the App.config file:  
 *  (1) dbConnectString: database connection string
 *      e.g.: <add key="dbConnectString" value="Persist Security Info=False;User ID=pathcase;Password=dblab;Initial Catalog=PathCase_SystemBiology_Test2;Data Source=dblab.case.edu;"/>
 *  (2) modelDirectory: full path to the directory that contains SBML files to be parsed 
 *      (this folder should not contain any other files as the parser does not check extension or type of input files)
 *      e.g.: <add key="modelDirectory" value="C:\Ali\BioModels\release_03December2008_sbmls\curated"/>
 * 
 * NOTE: After parsing the last file, the program may throw an "Access Violation" exception. This exception is generated within libSBML DLLs, and
 * the cause of the exception has not been resolved yet. However, this exception is thrown after everything finishes. Thus, it does not seem
 * that this exception has any effect on the parsing process -- but still annoying.
 * ********************************************************************************************************************************************************************************/


namespace PathwaysLib.SBMLParser
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var recon = File.ReadAllText(@"C:\Users\f\Downloads\BIOMD0000000064_SBML-L3V1.xml");
            ParseSbml(recon);
        }
        public static Guid ParseSbml(string xml)
        {
            Guid insertedModelId = new Guid();

            /********************** Counters ********************************************/
            int modelsWithPathwayMappings = 0;
            int reactionsWithProcessMappings = 0;
            int speciesWithMoleculeMappings = 0;

            int missedPathways = 0; // there is no such entry in the database
            int missedPathways2 = 0; // there is an entry but no corresponding pathway
            int missedProcesses = 0;
            int missedMolecules = 0;

            int modelsWithKegg = 0;
            int modelsWithReactome = 0;
            int modelsWithGO = 0;

            int ncbiDifKEGGOrgs = 0;
            int ncbiDifOrgDB = 0;
            int KEGGDifOrgDB = 0;

            LinkedList<string> missed1 = new LinkedList<string>();
            LinkedList<string> missed2 = new LinkedList<string>();

            Dictionary<Guid, string> modelsWithKeggAnnotations = new Dictionary<Guid, string>();
            Dictionary<string, string> pathwaysWithModelAnnotations = new Dictionary<string, string>();
            Dictionary<string, string> missedPathwayAnnotations = new Dictionary<string, string>();
            Dictionary<Guid, string> missedModelAnnotations = new Dictionary<Guid, string>();
            Dictionary<string, Guid> KeggNCBIMapping = new Dictionary<string, Guid>();

            Dictionary<string, string> GOIds = new Dictionary<string, string>();
            Dictionary<string, string> missedGOIds = new Dictionary<string, string>();
            Dictionary<string, string> goAnnotationsCache = new Dictionary<string, string>();

            Dictionary<string, string> ECNumbers = new Dictionary<string, string>();
            Dictionary<string, string> missedECNumbers = new Dictionary<string, string>();

            // we are retriving sql connection string from app.config file. 
            string strCon = ConfigurationManager.AppSettings["dbUserUploadsConnectString"];
            string builtOnNewDatabase = System.Configuration.ConfigurationManager.AppSettings["builtOnNewDatabase"];
            DBWrapper.Instance = new DBWrapper(strCon);
            DBreset D = new DBreset();
            //if (builtOnNewDatabase.CompareTo("true") == 0)
            //{
            //    D.copyExistingData();
            //}
            //else
            //{
            //    Console.WriteLine("Cleaning the database...");
            //    D.clearFKRelations();// Since some of the tables will be kept for ids (Model, reaction, species and compartment)
            //    //foreign key relations should be cleared otherwise constraints will be voilated.
            //    D.ClearDB(); //Only necessary when working on top of the existing database, instead of empty database
            //}
            /************************* Initializing the Dictionaries ************************************************/

            Dictionary<string, Guid> compartmentClasses = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, Guid> compartmentIds = new Dictionary<string, Guid>();
            Dictionary<Guid, string> compartmentOutsideIds = new Dictionary<Guid, string>();
            Dictionary<string, Guid> compartmentTypeIds = new Dictionary<string, Guid>();
            Dictionary<string, string> mapRuleTypes = new Dictionary<string, string>();
            Dictionary<string, int> roleIds = new Dictionary<string, int>();
            Dictionary<string, Guid> ruleTypeIds = new Dictionary<string, Guid>();
            Dictionary<string, Guid> speciesIds = new Dictionary<string, Guid>();
            Dictionary<string, Guid> speciesTypeIds = new Dictionary<string, Guid>();
            Dictionary<string, Guid> unitIds = new Dictionary<string, Guid>();
            Dictionary<Guid, ServerCompartment> compartments = new Dictionary<Guid, ServerCompartment>();


            Guid compartmentTypeId = Guid.Empty;
            Guid compartmentId = Guid.Empty;
            Guid outsideId = Guid.Empty;
            int roleId = -1;
            int ruleTypeId = -1;
            Guid speciesId = Guid.Empty;
            Guid speciesTypeId = Guid.Empty;
            Guid unitId = Guid.Empty;
            Guid mOrgId = Guid.Empty;

            //KeggNCBIMapping["9606"] = Guid.Parse("5A76B1FC-DC22-4BB1-ADDE-1593DD4C5415"); // 9606->stahp hooman 

            //// Download mapping between KEGG Organisms and NCBI Taxonomy
            //Console.WriteLine("Parsing KEGG-NCBI organism mapping from local file...");
            //TextReader sr = new StreamReader(ConfigurationManager.AppSettings["genome"]);
            //string line, orgCode = "", taxId;
            //Guid orgId = Guid.Empty;
            //while ((line = sr.ReadLine()) != null)
            //{
            //    if (line.StartsWith("NAME "))
            //    {
            //        orgCode = line.Substring(4).Trim(); // Each name contains an abbrevations this is what we are looking for
            //        orgCode = orgCode.Split(',')[0];
            //        orgId = AttributeManager.FindSingleItem("OrganismID", orgCode);
            //        if (orgId == Guid.Empty)
            //            KEGGDifOrgDB++;
            //    }
            //    else if (line.StartsWith("TAXONOMY "))
            //    {
            //        line = line.Substring(line.IndexOf(":") + 1).Trim();
            //        if (!KeggNCBIMapping.ContainsKey(line))
            //            KeggNCBIMapping.Add(line, orgId);
            //        orgId = Guid.Empty;
            //    }
            //}
            //sr.Close();
            //************************************** prefill RuleType Table **********************************

            mapRuleTypes.Add("rateRule", "Rate Rule");
            mapRuleTypes.Add("assignmentRule", "Assignment Rule");
            mapRuleTypes.Add("algebraicRule", "Algebraic Rule");


            /************************************** prefill AnnotationQualifier Table ************************
             * 
             * There are currently (as of April 2009) 11 annotation qualifiers defined by the MIRIAM protocol
             * of the Biomodels.net database. Each mapping between a model component to an external resource
             * is annotated with one of these qualifiers to describe the nature of annotation, i.e., 
             *   is: exact mapping, hasPart: partial mapping, and so on.
             * 
             * We pre-store these qualifiers in the database before starting the population process. Note that
             * this hard coded list needs to be updated as the set of qualifiers defined by the MIRIAM
             * protocol is extended over the time.
             * **********************************************************************************************/
            string[] qualifiers ={"is", "isDescribedBy", "encodes", "hasPart", "hasVersion", 
                                        "isEncodedBy", "isHomologTo", "isPartOf", "isVersionOf", "occursIn","isPropertyOf", "unknown"};
            foreach (string qualifier in qualifiers)
            {
                //AnnotationQualifierManager's GetQualifierId method inserts its parameter entry
                // into the database, if it is not in the database.
                AnnotationQualifierManager.GetQualifierId(qualifier, true);
            }

            /* 
            * The compartment classes are matched with the compartment class id by using compartment class dictionary
             * table.
             * When new models are added to database, this dictionary table should be updated if new comparments are proposed 
             * by these new biological models.
            * *****************************************************************************************/

            /* 
             * libmsbml handles qualifiers with a set of UPPER CASE named constants. In a dictionary,
             * we store a mapping from these constants to our version of qualifiers, which are more
             * readable.
             * *****************************************************************************************/
            Dictionary<int, int> qualifierIds = new Dictionary<int, int>();
            qualifierIds.Add(libsbml.BQB_ENCODES, AnnotationQualifierManager.GetQualifierId("encodes"));
            qualifierIds.Add(libsbml.BQB_HAS_PART, AnnotationQualifierManager.GetQualifierId("hasPart"));
            qualifierIds.Add(libsbml.BQB_HAS_VERSION, AnnotationQualifierManager.GetQualifierId("hasVersion"));
            qualifierIds.Add(libsbml.BQB_IS, AnnotationQualifierManager.GetQualifierId("is"));
            qualifierIds.Add(libsbml.BQB_IS_DESCRIBED_BY, AnnotationQualifierManager.GetQualifierId("isDescribedBy"));
            qualifierIds.Add(libsbml.BQB_IS_ENCODED_BY, AnnotationQualifierManager.GetQualifierId("isEncodedBy"));
            qualifierIds.Add(libsbml.BQB_IS_HOMOLOG_TO, AnnotationQualifierManager.GetQualifierId("isHomologTo"));
            qualifierIds.Add(libsbml.BQB_IS_PART_OF, AnnotationQualifierManager.GetQualifierId("isPartOf"));
            qualifierIds.Add(libsbml.BQB_IS_VERSION_OF, AnnotationQualifierManager.GetQualifierId("isVersionOf"));
            qualifierIds.Add(libsbml.BQB_OCCURS_IN, AnnotationQualifierManager.GetQualifierId("occursIn")); // is not supported by libSBML                
            qualifierIds.Add(libsbml.BQB_UNKNOWN, AnnotationQualifierManager.GetQualifierId("unknown"));
            qualifierIds.Add(libsbml.BQB_IS_PROPERTY_OF, AnnotationQualifierManager.GetQualifierId("isPropertyOf"));

            /*************************  Prefill UnitDefinition Table *************************************************/

            Console.WriteLine("Populating UnitDefinition Table...");

            ArrayList listOfBasics = new ArrayList();
            string[] basics ={ "ampere", "gram", "katal", "metre", "second", "watt", "becquerel", "gray", "kelvin", "mole", 
                                    "siemens", "weber", "candela", "henry", "kilogram", "newton", "sievert", "coulomb", "hertz", 
                                    "litre", "ohm", "steradian", "dimensionless", "item", "lumen", "pascal", "tesla", "farad", 
                                    "joule", "lux", "radian", "volt" };

            SoapUnitDefinition su;
            ServerUnitDefinition srvu;
            foreach (string unit in basics)
            {
                su = new SoapUnitDefinition("", "", "", "", Guid.Empty, unit, unit, true);
                srvu = new ServerUnitDefinition(su);
                srvu.UpdateDatabase();
            }

            //substance: mole
            su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "substance", "substance", false);
            srvu = new ServerUnitDefinition(su);
            srvu.UpdateDatabase();
            srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("mole"), 1, 0, 1);

            //volume: litre
            su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "volume", "volume", false);
            srvu = new ServerUnitDefinition(su);
            srvu.UpdateDatabase();
            srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("litre"), 1, 0, 1);

            //area: square metre
            su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "area", "area", false);
            srvu = new ServerUnitDefinition(su);
            srvu.UpdateDatabase();
            srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("metre"), 2, 0, 1);

            //length: metre
            su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "length", "length", false);
            srvu = new ServerUnitDefinition(su);
            srvu.UpdateDatabase();
            srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("metre"), 1, 0, 1);

            //time: second
            su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "time", "time", false);
            srvu = new ServerUnitDefinition(su);
            srvu.UpdateDatabase();
            srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("second"), 1, 0, 1);


            //************************************** prefill ReactionSpeciesRole Table ************************
            Console.WriteLine("Populating ReactionSpeciesRole Table...");
            string[] roles = { "Reactant", "Product", "Modifier" };

            SoapReactionSpeciesRole rsr;
            ServerReactionSpeciesRole srvrsr;

            foreach (string role in roles)
            {
                rsr = new SoapReactionSpeciesRole(role);
                srvrsr = new ServerReactionSpeciesRole(rsr);
                srvrsr.UpdateDatabase();
            }
            

            //************************ Get the data source from the database or create it if it is not available ********


            ServerDataSource dsource = ServerDataSource.LoadByName("BioModels");
            if (dsource == null)
            {
                SoapDataSource sds = new SoapDataSource("Biomodels", "http://www.ebi.ac.uk/biomodels/");
                dsource = new ServerDataSource(sds);
                dsource.UpdateDatabase();
            }
            //************************ Create an SBML reader to start parsing SBMl files *********************************
            SBMLReader _sbmlReader = null;
            try
            {
                _sbmlReader = new SBMLReader();
            }
            catch (TypeInitializationException e)
            {
                Console.WriteLine(e);
            }

            //string _fileName;
            bool skip = false;

            SBMLDocument _sbmlDocument;
            ArrayList docs = new ArrayList(); // dummy container for SBMLDocument objects of libSBML. The goal is to prevent the garbage collector from disposing 
            // document objects, which otherwise leads to a generic exception with no known way to resolve.

            if (!String.IsNullOrEmpty(xml)) // check if the directory exists
            {
                //******************************************  empty id caches *********************************************
                mOrgId = Guid.Empty;
                compartmentIds.Clear();
                compartmentTypeIds.Clear();
                roleIds.Clear();
                ruleTypeIds.Clear();
                speciesIds.Clear();
                speciesTypeIds.Clear();
                unitIds.Clear();
                compartments.Clear();
                compartmentOutsideIds.Clear();

                String meta = "";
                String sbo = "";
                String notes = "";
                String anno = "";
                String math = "";
                String id = "";
                String name = "";
                double value = 0;
                bool constant = false;
                String variable = "";

                compartmentTypeId = Guid.Empty;
                compartmentId = Guid.Empty;
                outsideId = Guid.Empty;
                roleId = -1;
                ruleTypeId = -1;
                speciesId = Guid.Empty;
                speciesTypeId = Guid.Empty;
                unitId = Guid.Empty;
                //****************************************************************************************************
                //_sbmlDocument = null;
                try
                {
                    _sbmlDocument = _sbmlReader.readSBMLFromString(xml);
                }
                catch (EntryPointNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    return new Guid();
                }

                docs.Add(_sbmlDocument);
                //**************************************** Initiate Model Object ***********************************************
                Model _model = _sbmlDocument.getModel();

                string modelString = xml;

                if (_model.getMetaId() != null)
                    meta = _model.getMetaId();

                if (_model.getSBOTerm() != -1)
                    sbo = _model.getSBOTerm().ToString();

                if (_model.getNotesString() != null)
                    notes = _model.getNotesString();

                try
                {
                    if (_model.getAnnotationString() != null)
                        anno = _model.getAnnotationString();
                }
                catch
                {
                    anno = "";
                }

                // create the model in the database
                string idstr, namestr;
                idstr = _model.getId().Trim();
                namestr = _model.getName().Trim();
                if (namestr == "")
                    namestr = idstr;
                else if (idstr == "")
                    idstr = namestr;
                SoapModel soapModel = new SoapModel(meta, sbo, notes, anno, idstr, namestr,
                                                    (int)_model.getLevel(), (int)_model.getVersion(), dsource.ID, modelString, Guid.NewGuid().ToString());
                ServerModel serverModel = new ServerModel(soapModel);
                serverModel.UpdateDatabase();
                insertedModelId = soapModel.ID;

                //****************************** Parsing Annotations ******************************                        
                int qualifierId;
                Guid[] keggIds;
                ServerMapModelsPathways mmp;
                SoapMapModelsPathways smmp;
                ServerModelOrganism smo;
                ServerMapSbaseGO smg;
                string annotation, anntext;
                string organism;
                Guid organismId;

                bool isKegg = false, isGO = false, isReactome = false;
                goAnnotationsCache.Clear();

                /****************************************************************************************************************
                 * CVTerm is a libSBML construct used as part of the libSBML support for annotations conforming to 
                 * the guidelines specified by MIRIAM ('Minimum Information Requested in the Annotation of biochemical Models'). 
                 * The general scheme is as follows. A set of RDF-based annotations attached to a given SBML <annotation> element
                 * are read by RDFAnnotationParser and converted into a list of CVTerm objects. Each CVTerm object instance stores
                 * the following components of an annotation:
                 *     # The qualifier, which can be a MIRIAM 'biological qualifier', a 'model qualifier', or 
                 *       an unknown qualifier (as far as the CVTerm class is concerned). Qualifiers are used 
                 *       in MIRIAM to indicate the nature of the relationship between the object being annotated 
                 *       and the resource. In CVTerm, the qualifiers can be manipulated using the methods getQualifierType(),
                 *       setQualifierType(), and related methods.
                 * 
                 *     # The resource, represent by a URI (note: not a URL). In CVTerm, the resource component can be 
                 *       manipulated using the methods addResource() and removeResource(). 
                 * 
                 * An example annotation to a KEGG pathway:
                 *       <bqbiol:is>
                 *          <rdf:Bag>
                 *               <rdf:li rdf:resource="urn:miriam:kegg.pathway:tbr00010"/>
                 *          </rdf:Bag>
                 *       </bqbiol:is>
                 * ************************************************************************************************************/
                int numCVTerms = (int)_model.getNumCVTerms();
                for (int i = 0; i < numCVTerms; i++)
                {
                    CVTerm term = _model.getCVTerm(i);
                    //we are only interested in the biological qualifiers with our current data model
                    if (term.getQualifierType() != libsbml.BIOLOGICAL_QUALIFIER)
                        continue; // skip if this is not a biological qualifier

                    //do the qualifier database id-libsbml qualifier constant conversion using 
                    //the above mentioned dictionary
                    qualifierId = qualifierIds[term.getBiologicalQualifierType()];

                    //resource is any external data source, such as KEGG, Reactome, Pubmed, etc., and 
                    //represented by XMLAttributes class in libsbml
                    XMLAttributes resources = term.getResources();
                    for (int j = 0; j < resources.getLength(); j++)
                    {
                        /* getValue returns "urn:miriam:kegg.pathway:tbr00010"
                         * for the above example annotation */
                        annotation = resources.getValue(j);
                        anntext = annotation;
                        /* Next, we make an attempt to figure out the resource being referred
                         * to in the annotation, i.e., whether it is a KEGG annotation or GO annotation, or etc.
                         * In the following, the first part is only for counting the number of annotations
                         * for different types of data resources. In the second part, actual parsing is done
                         * and the parsed data is stored in the database.
                         * */

                        // PART I: Count annotations for different data sources: KEGG, Reactome, GO (for statistical purposes)
                        if (annotation.IndexOf("kegg") > 0) // check if this is kegg annotation
                        {
                            if (!modelsWithKeggAnnotations.ContainsKey(serverModel.ID))
                                modelsWithKeggAnnotations.Add(serverModel.ID, "");
                        }
                        else if (annotation.IndexOf("reactome") > 0 && !isReactome)
                        {//check of this is a reactome annotation. Reactome annotations currently
                            // are not stored in the database, but we just parse it for statistical
                            // purposes
                            isReactome = true;
                            modelsWithReactome++;
                        }
                        else if (annotation.IndexOf("obo.go") > 0 && !isGO)
                        {//check if this is a GO annotation
                            isGO = true;
                            modelsWithGO++;
                        }
                        //continue;

                        // PART II: Parse the annotations and insert them into the database
                        if (annotation.IndexOf("kegg") > 0) //parse kegg annotation
                        {
                            //the following lines extracts the kegg id of the entry, i.e.,
                            //  "urn:miriam:kegg.pathway:tbr00010" --> tbr00010
                            annotation = annotation.Substring(annotation.LastIndexOf(":") + 1);
                            int k = 0;
                            for (; k < annotation.Length; k++)
                                if (Char.IsDigit(annotation[k]))
                                    break;
                            if (k == annotation.Length)
                                continue;

                            /* the first three letters of kegg pathway ids refer to 
                             * the kegg id of the corresponding organism. We also
                             * make an attempt to extract this information
                             * ****************************************************/
                            organism = annotation.Substring(0, k);
                            annotation = annotation.Substring(k).Trim();
                            if (!pathwaysWithModelAnnotations.ContainsKey(annotation.Trim()))
                                pathwaysWithModelAnnotations.Add(annotation.Trim(), "");

                            //Get the database id for the referred pathway
                            keggIds = AttributeManager.FindItemsEndsWith("KEGGPathwayID", annotation);

                            //If it is not in the database, then our database does not contain this pathway.
                            // This may happen for two reasons:
                            //   (1) The referred pathway is not a metabolic pathway (e.g., a signaling pathway)
                            //   (2) The database is not up-to-date
                            if (keggIds == null || keggIds.Length == 0)
                            {
                                //keep the below information just for statistical purposes, i.e., it is not stored in the database                                        
                                if (!missedPathwayAnnotations.ContainsKey(annotation))
                                    missedPathwayAnnotations.Add(annotation, "");
                                if (modelsWithKeggAnnotations.ContainsKey(serverModel.ID) && !missedModelAnnotations.ContainsKey(serverModel.ID))
                                    missedModelAnnotations.Add(serverModel.ID, "");

                                continue;
                            }
                            //get the database id for the referred organism in the annotation
                            organismId = AttributeManager.FindSingleItem("OrganismID", organism);

                            // for each kegg annotation, create a row in MapModelsPathways table with the extracted pathway info
                            // through the instances of the corresponding wrapper class.
                            foreach (Guid pid in keggIds)
                            {
                                if (!ServerPathway.Exists(pid))
                                    continue;

                                smmp = new SoapMapModelsPathways(serverModel.ID, pid, qualifierId, organismId);
                                mmp = new ServerMapModelsPathways(smmp);
                                mmp.UpdateDatabase();
                            }
                            keggIds = null;
                        }
                        else if (annotation.IndexOf("taxonomy") > 0) // parsing organism annotation
                        {
                            //extract the id of the entry
                            annotation = annotation.Substring(annotation.LastIndexOf("/") + 1).Trim();

                            //check of this entry is included in the KEGG-NCBI mapping dictionary  downloaded at the beginning
                            //if (!KeggNCBIMapping.ContainsKey(annotation))
                            //{
                            //    ncbiDifKEGGOrgs++; // statistical information, not inserted into the database
                            //    mOrgId = Guid.Empty;
                            //}
                            //else
                            mOrgId = KeggNCBIMapping[annotation];
                            //if (mOrgId == Guid.Empty)
                            //    ncbiDifOrgDB++; // statistical information, not inserted into the database

                            //create a row in ModelOrganism table with the organism annotation through the corresponding wrapper class.
                            smo = new ServerModelOrganism(serverModel.ID, mOrgId, int.Parse(annotation), qualifierId);
                            smo.UpdateDatabase();
                        }
                        else if (annotation.IndexOf("obo.go") > 0) // parsing GO annotation
                        {
                            //extract the GO id
                            annotation = annotation.Substring(annotation.Length - 7).Trim();
                            //check if this GO term is in the datbase
                            if (!GOIds.ContainsKey(annotation))
                            {
                                if (!missedGOIds.ContainsKey(annotation))
                                    missedGOIds.Add(annotation, "");
                                continue;
                            }
                            //check for duplicate annotations with the same GO term
                            if (goAnnotationsCache.ContainsKey(annotation))
                                continue;
                            goAnnotationsCache.Add(annotation, "");

                            //create a row in MapSbaseGO table with the GO term annotation through the corresponding wrapper class.                                    
                            smg = new ServerMapSbaseGO(serverModel.ID, annotation, (short)qualifierId);
                            smg.UpdateDatabase();
                        }
                    }
                }

                //******************************************* Initiate FunctionDefinition Object ***********************************
                for (int i = 0; i < _model.getNumFunctionDefinitions(); i++)
                {
                    FunctionDefinition _functionDefinition = _model.getFunctionDefinition(i);

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_functionDefinition.getMetaId() != null)
                        meta = _functionDefinition.getMetaId();

                    if (_functionDefinition.getSBOTerm() != -1)
                        sbo = _functionDefinition.getSBOTerm().ToString();

                    if (_functionDefinition.getNotesString() != null)
                        notes = _functionDefinition.getNotesString();

                    if (_functionDefinition.getAnnotationString() != null)
                        anno = _functionDefinition.getAnnotationString();

                    idstr = _functionDefinition.getId().Trim();
                    namestr = _functionDefinition.getName().Trim();
                    if (namestr == "")
                        namestr = idstr;
                    else if (idstr == "")
                        idstr = namestr;
                    SoapFunctionDefinition sFunctionDefinition = new SoapFunctionDefinition(meta, sbo, notes, anno, serverModel.ID, idstr,
                                                                                               namestr, libsbml.formulaToString(_functionDefinition.getMath()));
                    ServerFunctionDefinition serFunctionDefinition = new ServerFunctionDefinition(sFunctionDefinition);
                    serFunctionDefinition.UpdateDatabase();
                }

                //***************************************** Initiate UnitDefinition Object ***************************************
                string unitKind;

                for (int i = 0; i < _model.getNumUnitDefinitions(); i++)
                {
                    UnitDefinition _unitDefinition = _model.getUnitDefinition(i);

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_unitDefinition.getMetaId() != null)
                        meta = _unitDefinition.getMetaId();

                    if (_unitDefinition.getSBOTerm() != -1)
                        sbo = _unitDefinition.getSBOTerm().ToString();

                    if (_unitDefinition.getNotesString() != null)
                        notes = _unitDefinition.getNotesString();

                    if (_unitDefinition.getAnnotationString() != null)
                        anno = _unitDefinition.getAnnotationString();


                    idstr = _unitDefinition.getId().Trim();
                    namestr = _unitDefinition.getName().Trim();
                    if (namestr == "")
                        namestr = idstr;
                    else if (idstr == "")
                        idstr = namestr;
                    SoapUnitDefinition sUnitDefinition = new SoapUnitDefinition(meta, sbo, notes, anno, serverModel.ID, idstr, namestr, false);
                    ServerUnitDefinition serUnitDefinition = new ServerUnitDefinition(sUnitDefinition);

                    serUnitDefinition.UpdateDatabase();

                    ListOfUnits _listOfUnits = _unitDefinition.getListOfUnits();

                    for (int j = 0; j < _listOfUnits.size(); j++)
                    {
                        Unit _unit = (Unit)_listOfUnits.get(j);
                        unitKind = libsbml.UnitKind_toString(_unit.getKind());
                        serUnitDefinition.AddUnit(UnitDefinitionManager.GetBaseUnitId(unitKind), _unit.getExponent(), _unit.getScale(), _unit.getMultiplier());
                    }

                    unitIds.Add(serUnitDefinition.SbmlId, serUnitDefinition.ID);
                }

                //********************************* Initiate CompartmentType Object *****************************************

                for (int i = 0; i < _model.getNumCompartmentTypes(); i++)
                {
                    CompartmentType _compartmentType = _model.getCompartmentType(i);

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_compartmentType.getMetaId() != null)
                        meta = _compartmentType.getMetaId();

                    if (_compartmentType.getSBOTerm() != -1)
                        sbo = _compartmentType.getSBOTerm().ToString();

                    if (_compartmentType.getNotesString() != null)
                        notes = _compartmentType.getNotesString();

                    if (_compartmentType.getAnnotationString() != null)
                        anno = _compartmentType.getAnnotationString();


                    idstr = _compartmentType.getId().Trim();
                    namestr = _compartmentType.getName().Trim();
                    if (namestr == "")
                        namestr = idstr;
                    else if (idstr == "")
                        idstr = namestr;

                    SoapCompartmentType sCompartmentType = new SoapCompartmentType(meta, sbo, notes, anno, serverModel.ID, idstr, namestr);
                    ServerCompartmentType serCompartmentType = new ServerCompartmentType(sCompartmentType);
                    serCompartmentType.UpdateDatabase();
                    compartmentTypeIds.Add(serCompartmentType.SbmlId, serCompartmentType.ID);
                }

                //*********************************** Initiate Compartment Object *******************************

                LinkedList<ServerMapSbaseGO> goAnnotations = new LinkedList<ServerMapSbaseGO>();
                for (int i = 0; i < _model.getNumCompartments(); i++)
                {
                    Compartment _compartment = _model.getCompartment(i);

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_compartment.getMetaId() != null)
                        meta = _compartment.getMetaId();

                    if (_compartment.getSBOTerm() != -1)
                        sbo = _compartment.getSBOTerm().ToString();

                    if (_compartment.getNotesString() != null)
                        notes = _compartment.getNotesString();

                    if (_compartment.getAnnotationString() != null)
                        anno = _compartment.getAnnotationString();


                    if (_compartment.getCompartmentType() == "")
                        compartmentTypeId = Guid.Empty;
                    else
                        compartmentTypeId = compartmentTypeIds[_compartment.getCompartmentType()];

                    outsideId = Guid.Empty;

                    if (_compartment.getUnits() != "")
                    {
                        if (unitIds.ContainsKey(_compartment.getUnits()))
                            unitId = unitIds[_compartment.getUnits()];
                        else
                            unitId = UnitDefinitionManager.GetBaseUnitId(_compartment.getUnits());
                    }
                    else
                        unitId = Guid.Empty;

                    int SD = (int)_compartment.getSpatialDimensions();
                    if (SD < 0 || SD > 3)
                        SD = 3;

                    idstr = _compartment.getId().Trim();
                    namestr = _compartment.getName().Trim();
                    if (namestr == "")
                        namestr = idstr;
                    else if (idstr == "")
                        idstr = namestr;
                    Guid compartmentClassId = Guid.Empty;
                    /*
                     Since the compartment id and compartment names are used interchangeably
                     this section of the code will search for both alternatives
                     until it gives not found error. Program can not continue without compartment class.
                     The keys are not case sensitive!.
                     */
                    if (compartmentClasses.ContainsKey(namestr))
                    {
                        compartmentClassId = compartmentClasses[namestr];
                    }
                    else if (compartmentClasses.ContainsKey(idstr))
                    {
                        compartmentClassId = compartmentClasses[idstr];
                    }
                    //If compartment name is missing in dictionary it will remain as Guid.Empty
                    SoapCompartment sCompartment = new SoapCompartment(meta, sbo, notes, anno, serverModel.ID, idstr, namestr, compartmentTypeId,
                                                                          SD, (float)_compartment.getSize(), unitId, outsideId, compartmentClassId, _compartment.getConstant());

                    ServerCompartment serCompartment = new ServerCompartment(sCompartment);

                    //DO NOT insert compartment into the database yet -- needs prioritization, which is handled below.
                    //serCompartment.UpdateDatabase();
                    compartments.Add(serCompartment.ID, serCompartment);
                    compartmentIds.Add(serCompartment.SbmlId, serCompartment.ID);
                    compartmentOutsideIds.Add(serCompartment.ID, _compartment.getOutside());

                    /*** Parsing Annotations ********************************************
                     * 
                     * For more details, please see the commenting for 
                     * Model annotation parsing (above) as the same procedure repeated here.
                     * ******************************************************************/
                    goAnnotationsCache.Clear();
                    numCVTerms = (int)_compartment.getNumCVTerms();
                    for (int ti = 0; ti < numCVTerms; ti++)
                    {
                        CVTerm term = _compartment.getCVTerm(ti);
                        if (term.getQualifierType() != libsbml.BIOLOGICAL_QUALIFIER)
                            continue;
                        qualifierId = qualifierIds[term.getBiologicalQualifierType()];

                        XMLAttributes resources = term.getResources();
                        for (int j = 0; j < resources.getLength(); j++)
                        {
                            annotation = resources.getValue(j);
                            if (annotation.IndexOf("obo.go") > 0) // parsing GO annotation
                            {
                                annotation = annotation.Substring(annotation.Length - 7).Trim();
                                if (!GOIds.ContainsKey(annotation))
                                {
                                    if (!missedGOIds.ContainsKey(annotation))
                                        missedGOIds.Add(annotation, "");
                                    continue;
                                }
                                if (goAnnotationsCache.ContainsKey(annotation))
                                    continue;
                                goAnnotationsCache.Add(annotation, "");
                                smg = new ServerMapSbaseGO(serCompartment.ID, annotation, (short)qualifierId);
                                //smg.UpdateDatabase();
                                goAnnotations.AddFirst(smg);
                            }
                        }
                    }
                    //*** end of annotation parsing ***
                }

                //an inefficient but practical mechanism to make sure that parent compartments are always inserted before their children.
                while (compartmentOutsideIds.Count > 0)
                {
                    foreach (ServerCompartment sc in compartments.Values)
                    {
                        if (!compartmentOutsideIds.ContainsKey(sc.ID))
                            continue;
                        if (compartmentOutsideIds[sc.ID].Trim() != "")
                            sc.Outside = compartmentIds[compartmentOutsideIds[sc.ID]];
                        if (sc.Outside == Guid.Empty || !compartmentOutsideIds.ContainsKey(sc.Outside))
                        {
                            compartmentOutsideIds.Remove(sc.ID);
                            sc.UpdateDatabase();
                        }
                    }
                }

                // now insert annotations as well
                foreach (ServerMapSbaseGO goAnn in goAnnotations)
                    goAnn.UpdateDatabase();
                goAnnotations.Clear();

                //******************************************* Initiate SpeciesType Object ***********************************

                for (int i = 0; i < _model.getNumSpeciesTypes(); i++)
                {
                    SpeciesType _speciesType = _model.getSpeciesType(i);

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_speciesType.getMetaId() != null)
                        meta = _speciesType.getMetaId();

                    if (_speciesType.getSBOTerm() != -1)
                        sbo = _speciesType.getSBOTerm().ToString();

                    if (_speciesType.getNotesString() != null)
                        notes = _speciesType.getNotesString();

                    if (_speciesType.getAnnotationString() != null)
                        anno = _speciesType.getAnnotationString();


                    idstr = _speciesType.getId().Trim();
                    namestr = _speciesType.getName().Trim();
                    if (namestr == "")
                        namestr = idstr;
                    else if (idstr == "")
                        idstr = namestr;

                    SoapSpeciesType sSpeciesType = new SoapSpeciesType(meta, sbo, notes, anno, serverModel.ID, idstr, namestr);
                    ServerSpeciesType serSpeciesType = new ServerSpeciesType(sSpeciesType);
                    serSpeciesType.UpdateDatabase();

                    speciesTypeIds.Add(serSpeciesType.SbmlId, serSpeciesType.ID);
                }

                //******************************************** Initiate Species Object ***************************************

                for (int i = 0; i < _model.getNumSpecies(); i++)
                {
                    Species _species = _model.getSpecies(i);

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_species.getMetaId() != null)
                        meta = _species.getMetaId();

                    if (_species.getSBOTerm() != -1)
                        sbo = _species.getSBOTerm().ToString();

                    if (_species.getNotesString() != null)
                        notes = _species.getNotesString();

                    if (_species.getAnnotationString() != null)
                        anno = _species.getAnnotationString();


                    if (_species.getSpeciesType() == "")
                        speciesTypeId = Guid.Empty;
                    else
                        speciesTypeId = speciesTypeIds[_species.getSpeciesType()];

                    if (_species.getCompartment() == "")
                        compartmentId = Guid.Empty;
                    else
                        compartmentId = compartmentIds[_species.getCompartment()];

                    if (_species.getUnits() != "")
                    {
                        if (unitIds.ContainsKey(_species.getUnits()))
                            unitId = unitIds[_species.getUnits()];
                        else
                            unitId = UnitDefinitionManager.GetBaseUnitId(_species.getUnits());
                    }
                    else
                        unitId = Guid.Empty;


                    idstr = _species.getId().Trim();
                    namestr = _species.getName().Trim();
                    if (namestr == "")
                        namestr = idstr;
                    else if (idstr == "")
                        idstr = namestr;

                    SoapSpecies sSpecies = new SoapSpecies(meta, sbo, notes, anno, serverModel.ID, idstr, namestr, speciesTypeId, compartmentId, _species.getInitialAmount(),
                                                                        _species.getInitialConcentration(), unitId, _species.getHasOnlySubstanceUnits(), _species.getBoundaryCondition(),
                                                                        _species.getCharge(), _species.getConstant());

                    ServerSpecies serSpecies = new ServerSpecies(sSpecies);
                    serSpecies.UpdateDatabase();

                    speciesIds.Add(serSpecies.SbmlId, serSpecies.ID);

                    /*** Parsing KEGG (Species-Molecular Entity Mapping) Annotations ******
                     * 
                     * For more details, please see the commenting for 
                     * Model annotation parsing (above) as the same procedure repeated here.
                     * **********************************************************************/
                    numCVTerms = (int)_species.getNumCVTerms();
                    ServerMapSpeciesMolecularEntities msm;
                    SoapMapSpeciesMolecularEntities smsm;
                    for (int ti = 0; ti < numCVTerms; ti++)
                    {
                        CVTerm term = _species.getCVTerm(ti);
                        if (term.getQualifierType() != libsbml.BIOLOGICAL_QUALIFIER)
                            continue;
                        qualifierId = qualifierIds[term.getBiologicalQualifierType()];

                        XMLAttributes resources = term.getResources();
                        for (int j = 0; j < resources.getLength(); j++)
                        {
                            annotation = resources.getValue(j);
                            if (annotation.IndexOf("kegg") > 0)
                            {
                                annotation = annotation.Substring(annotation.LastIndexOf(":") + 1).Trim();
                                keggIds = AttributeManager.FindItems("CompoundID", annotation);
                                if (keggIds == null || keggIds.Length == 0)
                                    continue;
                                foreach (Guid pid in keggIds)
                                {
                                    if (!ServerMolecularEntity.Exists(pid))
                                        continue;
                                    smsm = new SoapMapSpeciesMolecularEntities(serSpecies.ID, pid, qualifierId);
                                    msm = new ServerMapSpeciesMolecularEntities(smsm);
                                    msm.UpdateDatabase();
                                }
                                keggIds = null;
                            }
                        }
                    }//end of annotation parsing
                }

                //****************************************** Initiate Parameter(Global) Object ***************************************************

                for (int i = 0; i < _model.getNumParameters(); i++)
                {
                    Parameter _parameter = _model.getParameter(i);

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_parameter.getMetaId() != null)
                        meta = _parameter.getMetaId();

                    if (_parameter.getSBOTerm() != -1)
                        sbo = _parameter.getSBOTerm().ToString();

                    if (_parameter.getNotesString() != null)
                        notes = _parameter.getNotesString();

                    if (_parameter.getAnnotationString() != null)
                        anno = _parameter.getAnnotationString();

                    if (_parameter.getUnits() != "")
                    {
                        if (unitIds.ContainsKey(_parameter.getUnits()))
                            unitId = unitIds[_parameter.getUnits()];
                        else
                            unitId = UnitDefinitionManager.GetBaseUnitId(_parameter.getUnits());
                    }
                    else
                        unitId = Guid.Empty;

                    idstr = _parameter.getId().Trim();
                    namestr = _parameter.getName().Trim();
                    if (namestr == "")
                        namestr = idstr;
                    else if (idstr == "")
                        idstr = namestr;

                    SoapParameter sParameter = new SoapParameter(meta, sbo, notes, anno, serverModel.ID, Guid.Empty, idstr, namestr,
                                                                    _parameter.getValue(), unitId, _parameter.getConstant());

                    ServerParameter serParameter = new ServerParameter(sParameter);
                    serParameter.UpdateDatabase();
                }

                //****************************** Initiate Reaction, Kinetic Law, Parameter (Local) Object *****************
                Guid kLawId;
                for (int i = 0; i < _model.getNumReactions(); i++)
                {
                    kLawId = Guid.Empty;
                    Reaction _reaction = _model.getReaction(i);
                    KineticLaw _kLaw = _reaction.getKineticLaw();

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";
                    math = "";

                    if (_kLaw != null)
                    {
                        if (_kLaw.getMetaId() != null)
                            meta = _kLaw.getMetaId();

                        if (_kLaw.getSBOTerm() != -1)
                            sbo = _kLaw.getSBOTerm().ToString();

                        if (_kLaw.getNotesString() != null)
                            notes = _kLaw.getNotesString();

                        if (_kLaw.getAnnotationString() != null)
                            anno = _kLaw.getAnnotationString();

                        if (libsbml.formulaToString(_kLaw.getMath()) != null)
                            math = libsbml.formulaToString(_kLaw.getMath());

                        SoapKineticLaw sKLaw = new SoapKineticLaw(meta, sbo, notes, anno, math);
                        ServerKineticLaw serKLaw = new ServerKineticLaw(sKLaw);
                        serKLaw.UpdateDatabase();
                        kLawId = serKLaw.ID;
                    }

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_reaction.getMetaId() != null)
                        meta = _reaction.getMetaId();

                    if (_reaction.getSBOTerm() != -1)
                        sbo = _reaction.getSBOTerm().ToString();

                    if (_reaction.getNotesString() != null)
                        notes = _reaction.getNotesString();

                    if (_reaction.getAnnotationString() != null)
                        anno = _reaction.getAnnotationString();


                    idstr = _reaction.getId().Trim();
                    namestr = _reaction.getName().Trim();
                    if (namestr == "")
                        namestr = idstr;
                    else if (idstr == "")
                        idstr = namestr;

                    SoapReaction sReaction = new SoapReaction(meta, sbo, notes, anno, serverModel.ID, idstr, namestr,
                                                             _reaction.getReversible(), _reaction.getFast(), kLawId);
                    ServerReaction serReaction = new ServerReaction(sReaction);
                    serReaction.UpdateDatabase();

                    /***** Parsing Annotations ***********************************************
                     * 
                     * For more details, please see the commenting for 
                     * Model annotation parsing (above) as the same procedure repeated here.
                     * **********************************************************************/
                    numCVTerms = (int)_reaction.getNumCVTerms();
                    ServerMapReactionsProcessEntities mrp;
                    SoapMapReactionsProcessEntities smrp;
                    ServerMapReactionECNumber smec;
                    goAnnotationsCache.Clear();
                    for (int ti = 0; ti < numCVTerms; ti++)
                    {
                        CVTerm term = _reaction.getCVTerm(ti);
                        if (term.getQualifierType() != libsbml.BIOLOGICAL_QUALIFIER)
                            continue;
                        qualifierId = qualifierIds[term.getBiologicalQualifierType()];

                        XMLAttributes resources = term.getResources();
                        for (int j = 0; j < resources.getLength(); j++)
                        {
                            annotation = resources.getValue(j);
                            if (annotation.IndexOf("kegg") > 0) // parsing KEGG annotation
                            {
                                annotation = annotation.Substring(annotation.LastIndexOf(":") + 1).Trim();
                                keggIds = AttributeManager.FindItems("ReactionID", annotation);
                                if (keggIds == null || keggIds.Length == 0)
                                    continue;
                                foreach (Guid pid in keggIds)
                                {
                                    if (!ServerProcess.Exists(pid))
                                        continue;
                                    smrp = new SoapMapReactionsProcessEntities(serReaction.ID, pid, qualifierId);
                                    mrp = new ServerMapReactionsProcessEntities(smrp);
                                    mrp.UpdateDatabase();
                                }
                                keggIds = null;
                            }
                            else if (annotation.IndexOf("ec-code") > 0) //parsing EC Number annotation -- ec-code
                            {
                                annotation = annotation.Substring(annotation.LastIndexOf(":") + 1).Trim();
                                if (annotation.StartsWith("EC+"))
                                    annotation = annotation.Substring(3);
                                else if (annotation.StartsWith("+"))
                                    annotation = annotation.Substring(1);
                                string[] ecsubs = annotation.Split('.');
                                for (int z = ecsubs.Length; z < 4; z++)
                                    annotation += ".-";
                                if (!ECNumbers.ContainsKey(annotation))
                                {
                                    if (!missedECNumbers.ContainsKey(annotation))
                                        missedECNumbers.Add(annotation, "");
                                    continue;
                                }
                                smec = new ServerMapReactionECNumber(serReaction.ID, annotation, qualifierId);
                                smec.UpdateDatabase();
                            }
                            else if (annotation.IndexOf("obo.go") > 0) // parsing GO annotation
                            {
                                annotation = annotation.Substring(annotation.Length - 7).Trim();
                                if (!GOIds.ContainsKey(annotation))
                                {
                                    if (!missedGOIds.ContainsKey(annotation))
                                        missedGOIds.Add(annotation, "");
                                    continue;
                                }
                                if (goAnnotationsCache.ContainsKey(annotation))
                                    continue;
                                goAnnotationsCache.Add(annotation, "");
                                smg = new ServerMapSbaseGO(serReaction.ID, annotation, (short)qualifierId);
                                smg.UpdateDatabase();
                            }
                        }
                    }
                    //*** end of annotation parsing ***

                    for (int j = 0; j < _kLaw.getNumParameters(); j++)
                    {
                        Parameter _parameter = _kLaw.getParameter(j);

                        meta = "";
                        sbo = "";
                        notes = "";
                        anno = "";
                        math = "";
                        id = "";
                        name = "";
                        value = 0;
                        constant = false;

                        if (_parameter != null)
                        {
                            if (_parameter.getMetaId() != null)
                                meta = _parameter.getMetaId();

                            if (_parameter.getSBOTerm() != -1)
                                sbo = _parameter.getSBOTerm().ToString();

                            if (_parameter.getNotesString() != null)
                                notes = _parameter.getNotesString();

                            if (_parameter.getAnnotationString() != null)
                                anno = _parameter.getAnnotationString().ToString();

                            if (_parameter.getId() != null)
                                id = _parameter.getId();

                            if (_parameter.getName() != null)
                                name = _parameter.getName();

                            if (_parameter.getValue() != 0)
                                value = _parameter.getValue();

                            if (_parameter.getConstant() != false)
                                constant = _parameter.getConstant();

                            if (_parameter.getUnits() != "")
                            {
                                if (unitIds.ContainsKey(_parameter.getUnits()))
                                    unitId = unitIds[_parameter.getUnits()];
                                else
                                    unitId = UnitDefinitionManager.GetBaseUnitId(_parameter.getUnits());
                            }
                            else
                                unitId = Guid.Empty;


                            if (name == "")
                                name = id;
                            else if (id == "")
                                id = name;

                            SoapParameter sParameter = new SoapParameter(meta, sbo, notes, anno, Guid.Empty, serReaction.ID, id, name, value,
                                                                         unitId, constant);
                            ServerParameter serParameter = new ServerParameter(sParameter);
                            serParameter.UpdateDatabase();
                        }
                    }

                    roleId = ReactionSpeciesRoleManager.GetRoleId("Reactant");
                    for (int j = 0; j < _reaction.getNumReactants(); j++)
                    {
                        Guid mathId = Guid.Empty;

                        SpeciesReference _sReference = _reaction.getReactant(j);
                        double _sStoichiometry = _sReference.getStoichiometry();
                        StoichiometryMath _sMath = _sReference.getStoichiometryMath();

                        if (_sMath == null && _sStoichiometry <= 0)
                            _sStoichiometry = 1;

                        if (_sMath != null)
                        {
                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";
                            math = "";

                            if (_sMath.getMetaId() != null)
                                meta = _sMath.getMetaId();

                            if (_sMath.getSBOTerm() != -1)
                                sbo = _sMath.getSBOTerm().ToString();

                            if (_sMath.getNotesString() != null)
                                notes = _sMath.getNotesString();

                            if (_sMath.getAnnotationString() != null)
                                anno = _sMath.getAnnotationString();

                            if (libsbml.formulaToString(_sMath.getMath()) != null)
                                math = libsbml.formulaToString(_sMath.getMath());

                            SoapStoichiometryMath sSMath = new SoapStoichiometryMath(meta, sbo, notes, anno, math);
                            ServerStoichiometryMath serSMath = new ServerStoichiometryMath(sSMath);
                            serSMath.UpdateDatabase();
                            mathId = serSMath.ID;
                        }

                        if (_sReference.getSpecies() == "")
                            speciesId = Guid.Empty;
                        else
                            speciesId = speciesIds[_sReference.getSpecies()];

                        meta = "";
                        sbo = "";
                        notes = "";
                        anno = "";

                        if (_sReference.getMetaId() != null)
                            meta = _sReference.getMetaId();

                        if (_sReference.getSBOTerm() != -1)
                            sbo = _sReference.getSBOTerm().ToString();

                        if (_sReference.getNotesString() != null)
                            notes = _sReference.getNotesString();

                        if (_sReference.getAnnotationString() != null)
                            anno = _sReference.getAnnotationString();


                        SoapReactionSpecies sRSpecies = new SoapReactionSpecies(meta, sbo, notes, anno, serReaction.ID, speciesId, (short)roleId, _sStoichiometry,
                                                                                mathId, _sReference.getId(), _sReference.getName());
                        ServerReactionSpecies serRSpecies = new ServerReactionSpecies(sRSpecies);
                        serRSpecies.UpdateDatabase();
                        _sReference = null;

                    }

                    roleId = ReactionSpeciesRoleManager.GetRoleId("Product");
                    for (int j = 0; j < _reaction.getNumProducts(); j++)
                    {
                        Guid mathId = Guid.Empty;

                        SpeciesReference _sReference = _reaction.getProduct(j);
                        double _sStoichiometry = _sReference.getStoichiometry();
                        StoichiometryMath _sMath = _sReference.getStoichiometryMath();

                        if (_sMath == null && _sStoichiometry <= 0)
                            _sStoichiometry = 1;

                        if (_sMath != null)
                        {
                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";
                            math = "";

                            if (_sMath.getMetaId() != null)
                                meta = _sMath.getMetaId();

                            if (_sMath.getSBOTerm() != -1)
                                sbo = _sMath.getSBOTerm().ToString();

                            if (_sMath.getNotesString() != null)
                                notes = _sMath.getNotesString();

                            if (_sMath.getAnnotationString() != null)
                                anno = _sMath.getAnnotationString();

                            if (libsbml.formulaToString(_sMath.getMath()) != null)
                                math = libsbml.formulaToString(_sMath.getMath());

                            SoapStoichiometryMath sSMath = new SoapStoichiometryMath(meta, sbo, notes, anno, math);
                            ServerStoichiometryMath serSMath = new ServerStoichiometryMath(sSMath);
                            serSMath.UpdateDatabase();
                            mathId = serSMath.ID;

                        }

                        if (_sReference.getSpecies() == "")
                            speciesId = Guid.Empty;
                        else
                            speciesId = speciesIds[_sReference.getSpecies()];

                        meta = "";
                        sbo = "";
                        notes = "";
                        anno = "";

                        if (_sReference.getMetaId() != null)
                            meta = _sReference.getMetaId();

                        if (_sReference.getSBOTerm() != -1)
                            sbo = _sReference.getSBOTerm().ToString();

                        if (_sReference.getNotesString() != null)
                            notes = _sReference.getNotesString();

                        if (_sReference.getAnnotationString() != null)
                            anno = _sReference.getAnnotationString();

                        SoapReactionSpecies sRSpecies = new SoapReactionSpecies(meta, sbo, notes, anno, serReaction.ID, speciesId, (short)roleId, _sStoichiometry,
                                                                                mathId, _sReference.getId(), _sReference.getName());
                        ServerReactionSpecies serRSpecies = new ServerReactionSpecies(sRSpecies);
                        serRSpecies.UpdateDatabase();

                    }

                    roleId = ReactionSpeciesRoleManager.GetRoleId("Modifier");
                    for (int j = 0; j < _reaction.getNumModifiers(); j++)
                    {
                        ModifierSpeciesReference _sReference = _reaction.getModifier(j);

                        if (_sReference.getSpecies() == "")
                            speciesId = Guid.Empty;
                        else
                            speciesId = speciesIds[_sReference.getSpecies()];

                        meta = "";
                        sbo = "";
                        notes = "";
                        anno = "";

                        if (_sReference.getMetaId() != null)
                            meta = _sReference.getMetaId();

                        if (_sReference.getSBOTerm() != -1)
                            sbo = _sReference.getSBOTerm().ToString();

                        if (_sReference.getNotesString() != null)
                            notes = _sReference.getNotesString();

                        if (_sReference.getAnnotationString() != null)
                            anno = _sReference.getAnnotationString();


                        SoapReactionSpecies sRSpecies = new SoapReactionSpecies(meta, sbo, notes, anno, serReaction.ID, speciesId, (short)roleId, 0, Guid.Empty,
                                                                                _sReference.getId(), _sReference.getName());
                        ServerReactionSpecies serRSpecies = new ServerReactionSpecies(sRSpecies);
                        serRSpecies.UpdateDatabase();
                    }
                }

                //********************************************* Initiate Constraint Object **********************************

                for (int i = 0; i < _model.getNumConstraints(); i++)
                {
                    Constraint _constraint = _model.getConstraint(i);

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_constraint.getMetaId() != null)
                        meta = _constraint.getMetaId();

                    if (_constraint.getSBOTerm() != -1)
                        sbo = _constraint.getSBOTerm().ToString();

                    if (_constraint.getNotesString() != null)
                        notes = _constraint.getNotesString();

                    if (_constraint.getAnnotationString() != null)
                        anno = _constraint.getAnnotationString();


                    SoapConstraint sConstraint = new SoapConstraint(meta, sbo, notes, anno, serverModel.ID, libsbml.formulaToString(_constraint.getMath()),
                                                                    _constraint.getMessage().toXMLString());

                    ServerConstraint serConstraint = new ServerConstraint(sConstraint);
                    serConstraint.UpdateDatabase();
                }

                //*************************************** Initiate Initial Assignment Object ******************************

                for (int i = 0; i < _model.getNumInitialAssignments(); i++)
                {
                    InitialAssignment _iAssignment = _model.getInitialAssignment(i);

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_iAssignment.getMetaId() != null)
                        meta = _iAssignment.getMetaId();

                    if (_iAssignment.getSBOTerm() != -1)
                        sbo = _iAssignment.getSBOTerm().ToString();

                    if (_iAssignment.getNotesString() != null)
                        notes = _iAssignment.getNotesString();

                    if (_iAssignment.getAnnotationString() != null)
                        anno = _iAssignment.getAnnotationString();


                    SoapInitialAssignment sIAssignment = new SoapInitialAssignment(meta, sbo, notes, anno, serverModel.ID, _iAssignment.getSymbol(),
                                                                                   libsbml.formulaToString(_iAssignment.getMath()));
                    ServerInitialAssignment serIAssignment = new ServerInitialAssignment(sIAssignment);
                    serIAssignment.UpdateDatabase();
                }


                //*************************************************** Initiate Rule Object *************************************

                //for (int i = 0; i < _model.getNumRules(); i++)
                //{
                //    Rule _rule = _model.getRule(i);

                //    meta = "";
                //    sbo = "";
                //    notes = "";
                //    anno = "";

                //    if (_rule.getMetaId() != null)
                //        meta = _rule.getMetaId();

                //    if (_rule.getSBOTerm() != -1)
                //        sbo = _rule.getSBOTerm().ToString();

                //    if (_rule.getNotesString() != null)
                //        notes = _rule.getNotesString();

                //    if (_rule.getAnnotationString() != null)
                //        anno = _rule.getAnnotationString();

                //    ruleTypeId = RuleTypeManager.GetTypeId(mapRuleTypes[_rule.getElementName()]);

                //    SoapRule sRule = new SoapRule(meta, sbo, notes, anno, serverModel.ID, _rule.getVariable(),
                //                                  libsbml.formulaToString(_rule.getMath()), (short)ruleTypeId);

                //    ServerRule serRule = new ServerRule(sRule);
                //    serRule.UpdateDatabase();
                //    _rule = null;

                //}

                //**************************** Initiate Event , Trigger, Delay , EventAssignment Object ********************
                Guid triggerId, delayId;
                for (int i = 0; i < _model.getNumEvents(); i++)
                {
                    triggerId = Guid.Empty;
                    delayId = Guid.Empty;
                    Event _event = _model.getEvent(i);

                    Trigger _trigger = _event.getTrigger();

                    Delay _delay = _event.getDelay();

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_delay != null)
                    {
                        if (_delay.getMetaId() != null)
                            meta = _delay.getMetaId();

                        if (_delay.getSBOTerm() != -1)
                            sbo = _delay.getSBOTerm().ToString();

                        if (_delay.getNotesString() != null)
                            notes = _delay.getNotesString();

                        if (_delay.getAnnotationString() != null)
                            anno = _delay.getAnnotationString();

                        if (libsbml.formulaToString(_delay.getMath()) != null)
                            math = libsbml.formulaToString(_delay.getMath());

                        SoapEventDelay sEventDelay = new SoapEventDelay(meta, sbo, notes, anno, math);
                        ServerEventDelay serEventDelay = new ServerEventDelay(sEventDelay);
                        serEventDelay.UpdateDatabase();
                        delayId = serEventDelay.ID;
                    }

                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_trigger != null)
                    {
                        if (_trigger.getMetaId() != null)
                            meta = _trigger.getMetaId();

                        if (_trigger.getSBOTerm() != -1)
                            sbo = _trigger.getSBOTerm().ToString();

                        if (_trigger.getNotesString() != null)
                            notes = _trigger.getNotesString();

                        if (_trigger.getAnnotationString() != null)
                            anno = _trigger.getAnnotationString();

                        if (libsbml.formulaToString(_trigger.getMath()) != null)
                            math = libsbml.formulaToString(_trigger.getMath());

                        SoapEventTrigger sEventTrigger = new SoapEventTrigger(meta, sbo, notes, anno, math);
                        ServerEventTrigger serEventTrigger = new ServerEventTrigger(sEventTrigger);
                        serEventTrigger.UpdateDatabase();
                        triggerId = serEventTrigger.ID;
                    }
                    meta = "";
                    sbo = "";
                    notes = "";
                    anno = "";

                    if (_event.getMetaId() != null)
                        meta = _event.getMetaId();

                    if (_event.getSBOTerm() != -1)
                        sbo = _event.getSBOTerm().ToString();

                    if (_event.getNotesString() != null)
                        notes = _event.getNotesString();

                    if (_event.getAnnotationString() != null)
                        anno = _event.getAnnotationString();

                    idstr = _event.getId().Trim();
                    namestr = _event.getName().Trim();
                    if (namestr == "")
                        namestr = idstr;
                    else if (idstr == "")
                        idstr = namestr;

                    SoapEvent sEvent = new SoapEvent(meta, sbo, notes, anno, serverModel.ID, idstr, namestr, triggerId, delayId);

                    ServerEvent serEvent = new ServerEvent(sEvent);
                    serEvent.UpdateDatabase();

                    for (int j = 0; j < _event.getNumEventAssignments(); j++)
                    {
                        EventAssignment _eventAssignment = _event.getEventAssignment(j);

                        meta = "";
                        sbo = "";
                        notes = "";
                        anno = "";
                        variable = "";
                        math = "";

                        if (_eventAssignment != null)
                        {

                            if (_eventAssignment.getMetaId() != null)
                                meta = _eventAssignment.getMetaId();

                            if (_eventAssignment.getSBOTerm() != -1)
                                sbo = _eventAssignment.getSBOTerm().ToString();

                            if (_eventAssignment.getNotesString() != null)
                                notes = _eventAssignment.getNotesString();

                            if (_eventAssignment.getAnnotationString() != null)
                                anno = _eventAssignment.getAnnotationString();

                            if (_eventAssignment.getVariable() != null)
                                variable = _eventAssignment.getVariable();

                            if (libsbml.formulaToString(_eventAssignment.getMath()) != null)
                                math = libsbml.formulaToString(_eventAssignment.getMath());

                            SoapEventAssignment sEAssignment = new SoapEventAssignment(meta, sbo, notes, anno, serEvent.ID, variable,
                                                                                       math);
                            ServerEventAssignment serEAssignment = new ServerEventAssignment(sEAssignment);
                            serEAssignment.UpdateDatabase();
                        }//end if event assignment is not null
                    }//end event assignment list                            
                }//end event insertion
            }//end if directory exists


            return insertedModelId;
        }

        public static void Main2(string[] args)
        {
            /********* To minimize accidental runs of this program, prompt user about the database cleanup **************/
            Console.WriteLine("DB Connection String: " + System.Configuration.ConfigurationManager.AppSettings["dbConnectString"] + "\n");
            Console.Write("You are about to delete everything in the above database, and start a re-population task. Do you want to continue? (y/n): ");
            //string input = Console.ReadLine();
            //if (input != "y")
            //    return;
            /********************** Counters ********************************************/
            int modelsWithPathwayMappings = 0;
            int reactionsWithProcessMappings = 0;
            int speciesWithMoleculeMappings = 0;

            int missedPathways = 0; // there is no such entry in the database
            int missedPathways2 = 0; // there is an entry but no corresponding pathway
            int missedProcesses = 0;
            int missedMolecules = 0;

            int modelsWithKegg = 0;
            int modelsWithReactome = 0;
            int modelsWithGO = 0;

            int ncbiDifKEGGOrgs = 0;
            int ncbiDifOrgDB = 0;
            int KEGGDifOrgDB = 0;

            LinkedList<string> missed1 = new LinkedList<string>();
            LinkedList<string> missed2 = new LinkedList<string>();

            Dictionary<Guid, string> modelsWithKeggAnnotations = new Dictionary<Guid, string>();
            Dictionary<string, string> pathwaysWithModelAnnotations = new Dictionary<string, string>();
            Dictionary<string, string> missedPathwayAnnotations = new Dictionary<string, string>();
            Dictionary<Guid, string> missedModelAnnotations = new Dictionary<Guid, string>();
            Dictionary<string, Guid> KeggNCBIMapping = new Dictionary<string, Guid>();

            Dictionary<string, string> GOIds = new Dictionary<string, string>();
            Dictionary<string, string> missedGOIds = new Dictionary<string, string>();
            Dictionary<string, string> goAnnotationsCache = new Dictionary<string, string>();

            Dictionary<string, string> ECNumbers = new Dictionary<string, string>();
            Dictionary<string, string> missedECNumbers = new Dictionary<string, string>();

            // we are retriving sql connection string from app.config file. 
            try
            {

                string strCon = System.Configuration.ConfigurationManager.AppSettings["dbConnectString"];
                string builtOnNewDatabase = System.Configuration.ConfigurationManager.AppSettings["builtOnNewDatabase"];
                DBWrapper.Instance = new DBWrapper(strCon);

                /*
                // Download up-to-date EC# Taxonomy
                Console.WriteLine("Downloading EC Number List from Expasy KEGG FTP site...");
                WebClient request = new WebClient();
                string ecNumberFileFTPAddress = System.Configuration.ConfigurationManager.AppSettings["ecNumberFileFTPAddress"];
                StreamReader sr = new StreamReader(request.OpenRead(ecNumberFileFTPAddress));
                string line, orgCode = "", taxId;
                Guid orgId = Guid.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                   // to be done
                }
                sr.Close();
                */

                // Refreshing Node Codes
                if(builtOnNewDatabase.CompareTo("true")==0){
                    Console.WriteLine("Refreshing Node Codes...");
                    ServerGOTerm.ComputeNodeCodes();
                    ServerECNumber.CompleteMissingParents();
                    ServerECNumber.ComputeNodeCodes();
                }
                
                
                // Download mapping between KEGG Organisms and NCBI Taxonomy
                Console.WriteLine("Parsing KEGG-NCBI organism mapping from local file...");
                TextReader sr = new StreamReader(ConfigurationManager.AppSettings["genome"]);
                string line, orgCode = "", taxId;
                Guid orgId = Guid.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("NAME "))
                    {
                        orgCode = line.Substring(4).Trim(); // Each name contains an abbrevations this is what we are looking for
                        orgCode = orgCode.Split(',')[0];
                        orgId = AttributeManager.FindSingleItem("OrganismID", orgCode);
                        if (orgId == Guid.Empty)
                            KEGGDifOrgDB++;
                    }
                    else if (line.StartsWith("TAXONOMY "))
                    {
                        line = line.Substring(line.IndexOf(":") + 1).Trim();
                        if (!KeggNCBIMapping.ContainsKey(line))
                            KeggNCBIMapping.Add(line, orgId);
                        orgId = Guid.Empty;
                    }
                }
                sr.Close();
                
                //fill the GOID cache
                ServerGOTerm[] gterms = ServerGOTerm.AllGOTerms();
                foreach (ServerGOTerm t in gterms)
                    GOIds.Add(t.ID, "");

                //fill the EC Number cache
                ServerECNumber[] ecs = ServerECNumber.AllECNumbers();
                foreach (ServerECNumber ec in ecs)
                    ECNumbers.Add(ec.ECNumber, "");
                
                DBreset D = new DBreset();
                //If you are using a blank database this code should be called.
                //Necessary old data (For frozen layouts) will be copied from existing database to the
                //new database
                if(builtOnNewDatabase.CompareTo("true")==0){
                    D.copyExistingData();
                }
                else{
                    Console.WriteLine("Cleaning the database...");
                    D.clearFKRelations();// Since some of the tables will be kept for ids (Model, reaction, species and compartment)
                    //foreign key relations should be cleared otherwise constraints will be voilated.
                    D.ClearDB(); //Only necessary when working on top of the existing database, instead of empty database
                }
                
                /************************* Initializing the Dictionaries ************************************************/
                Dictionary<string, Guid> compartmentClasses = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
                Dictionary<string, Guid> compartmentIds = new Dictionary<string, Guid>();
                Dictionary<Guid, string> compartmentOutsideIds = new Dictionary<Guid, string>();
                Dictionary<string, Guid> compartmentTypeIds = new Dictionary<string, Guid>();
                Dictionary<string, string> mapRuleTypes = new Dictionary<string, string>();
                Dictionary<string, int> roleIds = new Dictionary<string, int>();
                Dictionary<string, Guid> ruleTypeIds = new Dictionary<string, Guid>();
                Dictionary<string, Guid> speciesIds = new Dictionary<string, Guid>();
                Dictionary<string, Guid> speciesTypeIds = new Dictionary<string, Guid>();
                Dictionary<string, Guid> unitIds = new Dictionary<string, Guid>();
                Dictionary<Guid, ServerCompartment> compartments = new Dictionary<Guid, ServerCompartment>();


                Guid compartmentTypeId = Guid.Empty;
                Guid compartmentId = Guid.Empty;
                Guid outsideId = Guid.Empty;
                int roleId = -1;
                int ruleTypeId = -1;
                Guid speciesId = Guid.Empty;
                Guid speciesTypeId = Guid.Empty;
                Guid unitId = Guid.Empty;
                Guid mOrgId = Guid.Empty;

                /* 
                  * The compartment classes are matched with the compartment class id by using CompartmentClassDictionary
                  * table. In this table each row with a compartment name and class id
                  * will be added to a dictionary in order to process compartment classes during the creation of compartments.
                  * When new models are added to database, this table should be updated manually if new comparment names are proposed 
                  * by these new biological models (by using expert knowledge).
                */

                string query = "SELECT * FROM CompartmentClassDictionary";
                SqlCommand command = new SqlCommand(query);

                System.Data.DataSet[] ds = new System.Data.DataSet[0];
                DBWrapper.LoadMultiple(out ds, ref command);

                foreach (System.Data.DataSet d in ds)
                {
                    String compartmentName = (String)d.Tables[0].Rows[0]["compartmentName"];
                    Guid compartmentClassId = (Guid)d.Tables[0].Rows[0]["compartmentClassId"];
                    try
                    {
                        compartmentClasses.Add(compartmentName, compartmentClassId); // setting up dictionary 
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Uncessary compartment name " + compartmentName);
                    }


                }
                // For consistency frozen layout base ids are kept in the database
                // Other tables will be repopulated.
                query = "DELETE Sbase WHERE id NOT IN ((SELECT id FROM Model) UNION (SELECT id FROM CompartmentClass) " +
                "UNION (SELECT id FROM Species) UNION (SELECT id FROM Compartment) UNION (SELECT id FROM Reaction))";
                DBWrapper.Instance.ExecuteScalar(query);
                /*************************  Prefill UnitDefinition Table *************************************************/
                
                Console.WriteLine("Populating UnitDefinition Table...");

                ArrayList listOfBasics = new ArrayList();
                string[] basics ={ "ampere", "gram", "katal", "metre", "second", "watt", "becquerel", "gray", "kelvin", "mole", 
                                    "siemens", "weber", "candela", "henry", "kilogram", "newton", "sievert", "coulomb", "hertz", 
                                    "litre", "ohm", "steradian", "dimensionless", "item", "lumen", "pascal", "tesla", "farad", 
                                    "joule", "lux", "radian", "volt" };

                SoapUnitDefinition su;
                ServerUnitDefinition srvu;
                foreach (string unit in basics)
                {
                    su = new SoapUnitDefinition("", "", "", "", Guid.Empty, unit, unit, true);
                    srvu = new ServerUnitDefinition(su);
                    srvu.UpdateDatabase();
                }

                //substance: mole
                su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "substance", "substance", false);
                srvu = new ServerUnitDefinition(su);
                srvu.UpdateDatabase();
                srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("mole"), 1, 0, 1);

                //volume: litre
                su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "volume", "volume", false);
                srvu = new ServerUnitDefinition(su);
                srvu.UpdateDatabase();
                srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("litre"), 1, 0, 1);

                //area: square metre
                su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "area", "area", false);
                srvu = new ServerUnitDefinition(su);
                srvu.UpdateDatabase();
                srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("metre"), 2, 0, 1);

                //length: metre
                su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "length", "length", false);
                srvu = new ServerUnitDefinition(su);
                srvu.UpdateDatabase();
                srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("metre"), 1, 0, 1);

                //time: second
                su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "time", "time", false);
                srvu = new ServerUnitDefinition(su);
                srvu.UpdateDatabase();
                srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("second"), 1, 0, 1);

                //************************************** prefill AnnotationQualifier Table ************************
                Console.WriteLine("Populating AnnotationQualifier Table...");
                string[] qualifiers ={"is", "isDescribedBy", "encodes", "hasPart", "hasVersion", 
                                        "isEncodedBy", "isHomologTo", "isPartOf", "isVersionOf", "occursIn","hasProperty","isPropertyOf", "unknown"};

                foreach (string qualifier in qualifiers)
                {
                    AnnotationQualifierManager.GetQualifierId(qualifier, true);
                }
                Dictionary<int, int> qualifierIds = new Dictionary<int, int>();
                qualifierIds.Add(libsbml.BQB_ENCODES, AnnotationQualifierManager.GetQualifierId("encodes"));
                qualifierIds.Add(libsbml.BQB_HAS_PART, AnnotationQualifierManager.GetQualifierId("hasPart"));
                qualifierIds.Add(libsbml.BQB_HAS_VERSION, AnnotationQualifierManager.GetQualifierId("hasVersion"));
                qualifierIds.Add(libsbml.BQB_IS, AnnotationQualifierManager.GetQualifierId("is"));
                qualifierIds.Add(libsbml.BQB_IS_DESCRIBED_BY, AnnotationQualifierManager.GetQualifierId("isDescribedBy"));
                qualifierIds.Add(libsbml.BQB_IS_ENCODED_BY, AnnotationQualifierManager.GetQualifierId("isEncodedBy"));
                qualifierIds.Add(libsbml.BQB_IS_HOMOLOG_TO, AnnotationQualifierManager.GetQualifierId("isHomologTo"));
                qualifierIds.Add(libsbml.BQB_IS_PART_OF, AnnotationQualifierManager.GetQualifierId("isPartOf"));
                qualifierIds.Add(libsbml.BQB_IS_VERSION_OF, AnnotationQualifierManager.GetQualifierId("isVersionOf"));
                qualifierIds.Add(libsbml.BQB_OCCURS_IN, AnnotationQualifierManager.GetQualifierId("occursIn"));               
                qualifierIds.Add(libsbml.BQB_HAS_PROPERTY, AnnotationQualifierManager.GetQualifierId("hasProperty"));
                qualifierIds.Add(libsbml.BQB_IS_PROPERTY_OF, AnnotationQualifierManager.GetQualifierId("isPropertyOf"));
                qualifierIds.Add(libsbml.BQB_UNKNOWN, AnnotationQualifierManager.GetQualifierId("unknown"));

                //************************************** prefill ReactionSpeciesRole Table ************************
                Console.WriteLine("Populating ReactionSpeciesRole Table...");
                string[] roles = { "Reactant", "Product", "Modifier" };

                SoapReactionSpeciesRole rsr;
                ServerReactionSpeciesRole srvrsr;

                foreach (string role in roles)
                {
                    rsr = new SoapReactionSpeciesRole(role);
                    srvrsr = new ServerReactionSpeciesRole(rsr);
                    srvrsr.UpdateDatabase();
                }


                //************************************** prefill RuleType Table **********************************
                Console.WriteLine("Populating RuleType Table...");
                string[] ruleType = { "Algebraic Rule", "Assignment Rule", "Rate Rule" };

                SoapRuleType sRuleType;
                ServerRuleType serRuleType;

                foreach (string rule in ruleType)
                {
                    sRuleType = new SoapRuleType(rule);
                    serRuleType = new ServerRuleType(sRuleType);
                    serRuleType.UpdateDatabase();
                }

                mapRuleTypes.Add("rateRule", "Rate Rule");
                mapRuleTypes.Add("assignmentRule", "Assignment Rule");
                mapRuleTypes.Add("algebraicRule", "Algebraic Rule");

                //************************ Get the data source from the database or create it if it is not available ********

                ServerDataSource dsource = ServerDataSource.LoadByName("BioModels");
                if (dsource == null)
                {
                    SoapDataSource sds = new SoapDataSource("Biomodels", "http://www.ebi.ac.uk/biomodels/");
                    dsource = new ServerDataSource(sds);
                    dsource.UpdateDatabase();

                }


                //************************ Create an SBML reader to start parsing SBMl files *********************************

                string _directoryPath = System.Configuration.ConfigurationManager.AppSettings["modelDirectory"]; // The directory path to all the SBML files.

                //string _directoryPath = "C:\\case\\MS_Project\\ConsoleApplication1\\ConsoleApplication1\\ConsoleApplication1\\bin\\curated\\BIOMD0000000015.xml";
                //string _directoryPath = "C:\\case\\Biomodel_files\\release_03December2008_sbmls\\curated";
                SBMLReader _sbmlReader = new SBMLReader();



                //string _fileName;
                bool skip = false;

                SBMLDocument _sbmlDocument;
                ArrayList docs = new ArrayList(); // dummy container for SBMLDocument objects of libSBML. The goal is to prevent the garbage collector from disposing 
                // document objects, which otherwise leads to a generic exception with no known way to resolve.

                if (Directory.Exists(_directoryPath)) // check if the directory exists
                {
                    foreach (string _fileName in Directory.GetFiles(_directoryPath))  // get all the files in that directory
                    {
                        //if (_fileName.Contains("51.xml"))
                        //    skip = false;
                        //if (skip)
                        //    continue;

                        //_fileName = "C:\\case\\Biomodel_files\\release_03December2008_sbmls\\curated\\BIOMD0000000.xml";

                        Console.WriteLine("Processing model file: " + _fileName.Substring(_fileName.LastIndexOf("\\") + 1));

                        //******************************************  empty id caches *********************************************
                        mOrgId = Guid.Empty;
                        compartmentIds.Clear();
                        compartmentTypeIds.Clear();
                        roleIds.Clear();
                        ruleTypeIds.Clear();
                        speciesIds.Clear();
                        speciesTypeIds.Clear();
                        unitIds.Clear();
                        compartments.Clear();
                        compartmentOutsideIds.Clear();

                        String meta = "";
                        String sbo = "";
                        String notes = "";
                        String anno = "";
                        String math = "";
                        String id = "";
                        String name = "";
                        double value = 0;
                        bool constant = false;
                        String variable = "";

                        compartmentTypeId = Guid.Empty;
                        compartmentId = Guid.Empty;
                        outsideId = Guid.Empty;
                        roleId = -1;
                        ruleTypeId = -1;
                        speciesId = Guid.Empty;
                        speciesTypeId = Guid.Empty;
                        unitId = Guid.Empty;
                        //****************************************************************************************************
                        _sbmlDocument = _sbmlReader.readSBML(_fileName);
                        docs.Add(_sbmlDocument);



                        //**************************************** Initiate Model Object ***********************************************
                        Model _model = _sbmlDocument.getModel();

                        StreamReader reader = new StreamReader(_fileName);
                        string modelString = reader.ReadToEnd();
                        reader.Close();

                        if (_model.getMetaId() != null)
                            meta = _model.getMetaId();

                        if (_model.getSBOTerm() != -1)
                            sbo = _model.getSBOTerm().ToString();

                        if (_model.getNotesString() != null)
                            notes = _model.getNotesString();

                        try
                        {
                            if (_model.getAnnotationString() != null)
                                anno = _model.getAnnotationString();
                        }
                        catch (Exception e)
                        {
                            anno = "";
                        }

                        // create the model in the database
                        string idstr, namestr;
                        idstr = _model.getId().Trim();
                        namestr = _model.getName().Trim();
                        if (namestr == "")
                            namestr = idstr;
                        else if (idstr == "")
                            idstr = namestr;
                        SoapModel soapModel = new SoapModel(meta, sbo, notes, anno, idstr, namestr,
                                                            (int)_model.getLevel(), (int)_model.getVersion(), dsource.ID, modelString, _fileName.Substring(_fileName.LastIndexOf("\\") + 1));
                        /*
                         This section is necessary since the modelLayout's are kept by model ids
                         If a model is currently in the database we would like to keep the model
                         with same id, except the Model and SBase tables all other tables of the model
                         will be updated.
                         */

                        ServerModel[] existingModel = ServerModel.FindModelsByFileName(_fileName.Substring(_fileName.LastIndexOf("\\") + 1));
                        if (existingModel.Length == 1)// Model is in the database, so it is update
                        {
                            soapModel.ID = existingModel[0].ID; // Status changes no_change here 
                            soapModel.Status = ObjectStatus.NoChanges; // Without this can not set to update
                            soapModel.Status = ObjectStatus.Update; // setting it back

                        }
                        else if (existingModel.Length > 1)
                        {
                            Console.WriteLine("Model file name = " + soapModel.SbmlFileName + " matched more than one model in the database");
                            return;
                        }
                        ServerModel serverModel = new ServerModel(soapModel);
                        serverModel.UpdateDatabase();


                        //****************************** Parsing Annotations ******************************
                        int numCVTerms = (int)_model.getNumCVTerms();
                        int qualifierId;
                        Guid[] keggIds;
                        ServerMapModelsPathways mmp;
                        SoapMapModelsPathways smmp;
                        ServerModelOrganism smo;
                        ServerMapSbaseGO smg;
                        string annotation, anntext;
                        string organism;
                        Guid organismId;

                        bool isKegg = false, isGO = false, isReactome = false;
                        goAnnotationsCache.Clear();
                        for (int i = 0; i < numCVTerms; i++)
                        {
                            CVTerm term = _model.getCVTerm(i);
                            if (term.getQualifierType() != libsbml.BIOLOGICAL_QUALIFIER)
                                continue;
                            qualifierId = qualifierIds[term.getBiologicalQualifierType()];

                            XMLAttributes resources = term.getResources();
                            for (int j = 0; j < resources.getLength(); j++)
                            {
                                annotation = resources.getValue(j);
                                anntext = annotation;
                                if (annotation.IndexOf("kegg") > 0)
                                {
                                    if (!modelsWithKeggAnnotations.ContainsKey(serverModel.ID))
                                        modelsWithKeggAnnotations.Add(serverModel.ID, "");
                                }
                                else if (annotation.IndexOf("reactome") > 0 && !isReactome)
                                {
                                    isReactome = true;
                                    modelsWithReactome++;
                                }
                                else if (annotation.IndexOf("obo.go") > 0 && !isGO)
                                {
                                    isGO = true;
                                    modelsWithGO++;
                                }
                                //continue;

                                if (annotation.IndexOf("kegg") > 0)
                                {
                                    annotation = annotation.Substring(annotation.LastIndexOf(":") + 1);
                                    int k = 0;
                                    for (; k < annotation.Length; k++)
                                        if (Char.IsDigit(annotation[k]))
                                            break;
                                    if (k == annotation.Length)
                                        continue;
                                    organism = annotation.Substring(0, k);
                                    annotation = annotation.Substring(k).Trim();
                                    if (!pathwaysWithModelAnnotations.ContainsKey(annotation.Trim()))
                                        pathwaysWithModelAnnotations.Add(annotation.Trim(), "");
                                    keggIds = AttributeManager.FindItemsEndsWith("KEGGPathwayID", annotation);
                                    if (keggIds == null || keggIds.Length == 0)
                                    {
                                        if (!missedPathwayAnnotations.ContainsKey(annotation))
                                            missedPathwayAnnotations.Add(annotation, "");
                                        if (modelsWithKeggAnnotations.ContainsKey(serverModel.ID) && !missedModelAnnotations.ContainsKey(serverModel.ID))
                                            missedModelAnnotations.Add(serverModel.ID, "");

                                        continue;
                                    }
                                    organismId = AttributeManager.FindSingleItem("OrganismID", organism);
                                    foreach (Guid pid in keggIds)
                                    {
                                        if (!ServerPathway.Exists(pid))
                                            continue;

                                        smmp = new SoapMapModelsPathways(serverModel.ID, pid, qualifierId, organismId);
                                        mmp = new ServerMapModelsPathways(smmp);
                                        mmp.UpdateDatabase();
                                    }
                                    keggIds = null;
                                }
                                else if (annotation.IndexOf("taxonomy") > 0) // parsing organism annotation
                                {

                                    annotation = annotation.Substring(annotation.LastIndexOf(":") + 1).Trim();
                                    if (!KeggNCBIMapping.ContainsKey(annotation))
                                    {
                                        //throw new Exception("EXCEPTION: NCBI organism does not have a correspondance in the KEGG database: " + annotation);
                                        ncbiDifKEGGOrgs++;
                                        mOrgId = Guid.Empty;
                                        //continue;
                                    }
                                    else
                                        mOrgId = KeggNCBIMapping[annotation];
                                    if (mOrgId == Guid.Empty)
                                        ncbiDifOrgDB++;
                                    smo = new ServerModelOrganism(serverModel.ID, mOrgId, int.Parse(annotation), qualifierId);
                                    smo.UpdateDatabase();
                                }
                                else if (annotation.IndexOf("obo.go") > 0) // parsing GO annotation
                                {
                                    annotation = annotation.Substring(annotation.Length - 7).Trim();
                                    if (!GOIds.ContainsKey(annotation))
                                    {
                                        if (!missedGOIds.ContainsKey(annotation))
                                            missedGOIds.Add(annotation, "");
                                        continue;
                                    }
                                    if (goAnnotationsCache.ContainsKey(annotation))
                                        continue;
                                    goAnnotationsCache.Add(annotation, "");
                                    smg = new ServerMapSbaseGO(serverModel.ID, annotation, (short)qualifierId);
                                    smg.UpdateDatabase();
                                }
                            }
                        }

                        //Console.WriteLine("");
                        //Console.ReadLine();
                        ///continue;

                        //******************************************* Initiate FunctionDefinition Object ***********************************
                        for (int i = 0; i < _model.getNumFunctionDefinitions(); i++)
                        {
                            FunctionDefinition _functionDefinition = _model.getFunctionDefinition(i);

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_functionDefinition.getMetaId() != null)
                                meta = _functionDefinition.getMetaId();

                            if (_functionDefinition.getSBOTerm() != -1)
                                sbo = _functionDefinition.getSBOTerm().ToString();

                            if (_functionDefinition.getNotesString() != null)
                                notes = _functionDefinition.getNotesString();

                            if (_functionDefinition.getAnnotationString() != null)
                                anno = _functionDefinition.getAnnotationString();

                            idstr = _functionDefinition.getId().Trim();
                            namestr = _functionDefinition.getName().Trim();
                            if (namestr == "")
                                namestr = idstr;
                            else if (idstr == "")
                                idstr = namestr;
                            SoapFunctionDefinition sFunctionDefinition = new SoapFunctionDefinition(meta, sbo, notes, anno, serverModel.ID, idstr,
                                                                                                       namestr, libsbml.formulaToString(_functionDefinition.getMath()));
                            ServerFunctionDefinition serFunctionDefinition = new ServerFunctionDefinition(sFunctionDefinition);
                            serFunctionDefinition.UpdateDatabase();
                        }


                        //***************************************** Initiate UnitDefinition Object ***************************************

                        string unitKind;

                        for (int i = 0; i < _model.getNumUnitDefinitions(); i++)
                        {
                            UnitDefinition _unitDefinition = _model.getUnitDefinition(i);

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_unitDefinition.getMetaId() != null)
                                meta = _unitDefinition.getMetaId();

                            if (_unitDefinition.getSBOTerm() != -1)
                                sbo = _unitDefinition.getSBOTerm().ToString();

                            if (_unitDefinition.getNotesString() != null)
                                notes = _unitDefinition.getNotesString();

                            if (_unitDefinition.getAnnotationString() != null)
                                anno = _unitDefinition.getAnnotationString();


                            idstr = _unitDefinition.getId().Trim();
                            namestr = _unitDefinition.getName().Trim();
                            if (namestr == "")
                                namestr = idstr;
                            else if (idstr == "")
                                idstr = namestr;
                            SoapUnitDefinition sUnitDefinition = new SoapUnitDefinition(meta, sbo, notes, anno, serverModel.ID, idstr, namestr, false);
                            ServerUnitDefinition serUnitDefinition = new ServerUnitDefinition(sUnitDefinition);

                            serUnitDefinition.UpdateDatabase();

                            ListOfUnits _listOfUnits = _unitDefinition.getListOfUnits();

                            for (int j = 0; j < _listOfUnits.size(); j++)
                            {
                                Unit _unit = (Unit)_listOfUnits.get(j);
                                unitKind = libsbml.UnitKind_toString(_unit.getKind());
                                serUnitDefinition.AddUnit(UnitDefinitionManager.GetBaseUnitId(unitKind), _unit.getExponent(), _unit.getScale(), _unit.getMultiplier());
                            }

                            unitIds.Add(serUnitDefinition.SbmlId, serUnitDefinition.ID);
                        }

                        //********************************* Initiate CompartmentType Object *****************************************

                        for (int i = 0; i < _model.getNumCompartmentTypes(); i++)
                        {
                            CompartmentType _compartmentType = _model.getCompartmentType(i);

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_compartmentType.getMetaId() != null)
                                meta = _compartmentType.getMetaId();

                            if (_compartmentType.getSBOTerm() != -1)
                                sbo = _compartmentType.getSBOTerm().ToString();

                            if (_compartmentType.getNotesString() != null)
                                notes = _compartmentType.getNotesString();

                            if (_compartmentType.getAnnotationString() != null)
                                anno = _compartmentType.getAnnotationString();


                            idstr = _compartmentType.getId().Trim();
                            namestr = _compartmentType.getName().Trim();
                            if (namestr == "")
                                namestr = idstr;
                            else if (idstr == "")
                                idstr = namestr;

                            SoapCompartmentType sCompartmentType = new SoapCompartmentType(meta, sbo, notes, anno, serverModel.ID, idstr, namestr);
                            ServerCompartmentType serCompartmentType = new ServerCompartmentType(sCompartmentType);
                            serCompartmentType.UpdateDatabase();
                            compartmentTypeIds.Add(serCompartmentType.SbmlId, serCompartmentType.ID);
                        }

                        //*********************************** Initiate Compartment Object *******************************

                        LinkedList<ServerMapSbaseGO> goAnnotations = new LinkedList<ServerMapSbaseGO>();
                        for (int i = 0; i < _model.getNumCompartments(); i++)
                        {
                            Compartment _compartment = _model.getCompartment(i);

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_compartment.getMetaId() != null)
                                meta = _compartment.getMetaId();

                            if (_compartment.getSBOTerm() != -1)
                                sbo = _compartment.getSBOTerm().ToString();

                            if (_compartment.getNotesString() != null)
                                notes = _compartment.getNotesString();

                            if (_compartment.getAnnotationString() != null)
                                anno = _compartment.getAnnotationString();


                            if (_compartment.getCompartmentType() == "")
                                compartmentTypeId = Guid.Empty;
                            else
                                compartmentTypeId = compartmentTypeIds[_compartment.getCompartmentType()];

                            outsideId = Guid.Empty;

                            if (_compartment.getUnits() != "")
                            {
                                if (unitIds.ContainsKey(_compartment.getUnits()))
                                    unitId = unitIds[_compartment.getUnits()];
                                else
                                    unitId = UnitDefinitionManager.GetBaseUnitId(_compartment.getUnits());
                            }
                            else
                                unitId = Guid.Empty;

                            int SD = (int)_compartment.getSpatialDimensions();
                            if (SD < 0 || SD > 3)
                                SD = 3;

                            idstr = _compartment.getId().Trim();
                            namestr = _compartment.getName().Trim();
                            if (namestr == "")
                                namestr = idstr;
                            else if (idstr == "")
                                idstr = namestr;
                            Guid compartmentClassId = Guid.Empty;
                            /*
                             Since the compartment id and compartment names are used interchangeably
                             this section of the code will search for both alternatives
                             until it gives not found error. Program can not continue without compartment class.
                             The keys are not case sensitive!.
                             */
                            if (compartmentClasses.ContainsKey(namestr))
                            {
                                compartmentClassId = compartmentClasses[namestr];
                            }
                            else if (compartmentClasses.ContainsKey(idstr))
                            {
                                compartmentClassId = compartmentClasses[idstr];
                            }
                            else
                            {
                                Console.Write("Please add " + namestr + " compartment name to the database");
                                return;
                            }
                            SoapCompartment sCompartment = new SoapCompartment(meta, sbo, notes, anno, serverModel.ID, idstr, namestr, compartmentTypeId,
                                                                                  SD, (float)_compartment.getSize(), unitId, outsideId, compartmentClassId, _compartment.getConstant());
                            ServerCompartment[] existingCompartment = ServerCompartment.FindCompartmentByIds(serverModel.ID, idstr); // Search by model id + sbmlId superkey
                            if (existingCompartment.Length == 1)// Compartment is in the database, so update it 
                            {
                                sCompartment.ID = existingCompartment[0].ID; // Status changes no_change here 
                                sCompartment.Status = ObjectStatus.NoChanges; // Without this can not set to update
                                sCompartment.Status = ObjectStatus.Update; // setting it back

                            }
                            else if (existingCompartment.Length > 1)
                            {
                                Console.WriteLine("Compartment name = " + sCompartment.Name + ", id =" + sCompartment.ID + " sbmlId =" + sCompartment.SbmlId
                                    + " matched more than one compartment in the database");
                                return;
                            }

                            ServerCompartment serCompartment = new ServerCompartment(sCompartment);

                            //DO NOT insert compartment into the database yet -- needs prioritization, which is handled below.
                            //serCompartment.UpdateDatabase();
                            compartments.Add(serCompartment.ID, serCompartment);
                            compartmentIds.Add(serCompartment.SbmlId, serCompartment.ID);
                            compartmentOutsideIds.Add(serCompartment.ID, _compartment.getOutside());

                            //*** Parsing Annotations ***
                            goAnnotationsCache.Clear();
                            numCVTerms = (int)_compartment.getNumCVTerms();
                            for (int ti = 0; ti < numCVTerms; ti++)
                            {
                                CVTerm term = _compartment.getCVTerm(ti);
                                if (term.getQualifierType() != libsbml.BIOLOGICAL_QUALIFIER)
                                    continue;
                                qualifierId = qualifierIds[term.getBiologicalQualifierType()];

                                XMLAttributes resources = term.getResources();
                                for (int j = 0; j < resources.getLength(); j++)
                                {
                                    annotation = resources.getValue(j);
                                    if (annotation.IndexOf("obo.go") > 0) // parsing GO annotation
                                    {
                                        annotation = annotation.Substring(annotation.Length - 7).Trim();
                                        if (!GOIds.ContainsKey(annotation))
                                        {
                                            if (!missedGOIds.ContainsKey(annotation))
                                                missedGOIds.Add(annotation, "");
                                            continue;
                                        }
                                        if (goAnnotationsCache.ContainsKey(annotation))
                                            continue;
                                        goAnnotationsCache.Add(annotation, "");
                                        smg = new ServerMapSbaseGO(serCompartment.ID, annotation, (short)qualifierId);
                                        //smg.UpdateDatabase();
                                        goAnnotations.AddFirst(smg);
                                    }
                                }
                            }
                            //*** end of annotation parsing ***
                        }

                        //an inefficient but practical mechanism to make sure that parent compartments are always inserted before their children.
                        while (compartmentOutsideIds.Count > 0)
                        {
                            foreach (ServerCompartment sc in compartments.Values)
                            {
                                if (!compartmentOutsideIds.ContainsKey(sc.ID)) // If compartment has not an outside
                                    continue;
                                if (compartmentOutsideIds[sc.ID].Trim() != "") // If outside id is not empty
                                    sc.Outside = compartmentIds[compartmentOutsideIds[sc.ID]]; // Get outside by sbmlId
                                if (sc.Outside == Guid.Empty || !compartmentOutsideIds.ContainsKey(sc.Outside)) //Current compartment has no outside or outside has no outsides
                                {
                                    compartmentOutsideIds.Remove(sc.ID);
                                    sc.UpdateDatabase();
                                }
                            }
                        }

                        // now insert annotations as well
                        foreach (ServerMapSbaseGO goAnn in goAnnotations)
                            goAnn.UpdateDatabase();
                        goAnnotations.Clear();

                        //******************************************* Initiate SpeciesType Object ***********************************

                        for (int i = 0; i < _model.getNumSpeciesTypes(); i++)
                        {
                            SpeciesType _speciesType = _model.getSpeciesType(i);

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_speciesType.getMetaId() != null)
                                meta = _speciesType.getMetaId();

                            if (_speciesType.getSBOTerm() != -1)
                                sbo = _speciesType.getSBOTerm().ToString();

                            if (_speciesType.getNotesString() != null)
                                notes = _speciesType.getNotesString();

                            if (_speciesType.getAnnotationString() != null)
                                anno = _speciesType.getAnnotationString();


                            idstr = _speciesType.getId().Trim();
                            namestr = _speciesType.getName().Trim();
                            if (namestr == "")
                                namestr = idstr;
                            else if (idstr == "")
                                idstr = namestr;

                            SoapSpeciesType sSpeciesType = new SoapSpeciesType(meta, sbo, notes, anno, serverModel.ID, idstr, namestr);
                            ServerSpeciesType serSpeciesType = new ServerSpeciesType(sSpeciesType);
                            serSpeciesType.UpdateDatabase();

                            speciesTypeIds.Add(serSpeciesType.SbmlId, serSpeciesType.ID);
                        }

                        //******************************************** Initiate Species Object ***************************************

                        for (int i = 0; i < _model.getNumSpecies(); i++)
                        {
                            Species _species = _model.getSpecies(i);

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_species.getMetaId() != null)
                                meta = _species.getMetaId();

                            if (_species.getSBOTerm() != -1)
                                sbo = _species.getSBOTerm().ToString();

                            if (_species.getNotesString() != null)
                                notes = _species.getNotesString();

                            if (_species.getAnnotationString() != null)
                                anno = _species.getAnnotationString();


                            if (_species.getSpeciesType() == "")
                                speciesTypeId = Guid.Empty;
                            else
                                speciesTypeId = speciesTypeIds[_species.getSpeciesType()];

                            if (_species.getCompartment() == "")
                                compartmentId = Guid.Empty;
                            else
                                compartmentId = compartmentIds[_species.getCompartment()];

                            if (_species.getUnits() != "")
                            {
                                if (unitIds.ContainsKey(_species.getUnits()))
                                    unitId = unitIds[_species.getUnits()];
                                else
                                    unitId = UnitDefinitionManager.GetBaseUnitId(_species.getUnits());
                            }
                            else
                                unitId = Guid.Empty;


                            idstr = _species.getId().Trim();
                            namestr = _species.getName().Trim();
                            if (namestr == "")
                                namestr = idstr;
                            else if (idstr == "")
                                idstr = namestr;

                            SoapSpecies sSpecies = new SoapSpecies(meta, sbo, notes, anno, serverModel.ID, idstr, namestr, speciesTypeId, compartmentId, _species.getInitialAmount(),
                                                                                _species.getInitialConcentration(), unitId, _species.getHasOnlySubstanceUnits(), _species.getBoundaryCondition(),
                                                                                _species.getCharge(), _species.getConstant());
                            ServerSpecies[] existingSpecies = ServerSpecies.FindSpeciesByIds(serverModel.ID, idstr); // Search by model id + sbmlId superkey
                            if (existingSpecies.Length == 1)// Compartment is in the database, so update it 
                            {
                                sSpecies.ID = existingSpecies[0].ID; // Status changes no_change here 
                                sSpecies.Status = ObjectStatus.NoChanges; // Without this can not set to update
                                sSpecies.Status = ObjectStatus.Update; // setting it back
                            }
                            else if (existingSpecies.Length > 1)
                            {
                                Console.WriteLine("Species name = " + sSpecies.Name + ", id =" + sSpecies.ID + " sbmlId =" + sSpecies.SbmlId
                                    + " matched more than one species in the database");
                                return;
                            }
                            ServerSpecies serSpecies = new ServerSpecies(sSpecies);
                            serSpecies.UpdateDatabase();

                            speciesIds.Add(serSpecies.SbmlId, serSpecies.ID);

                            //*** Parsing KEGG (Species-Molecular Entity Mapping) Annotations ***
                            numCVTerms = (int)_species.getNumCVTerms();
                            ServerMapSpeciesMolecularEntities msm;
                            SoapMapSpeciesMolecularEntities smsm;
                            for (int ti = 0; ti < numCVTerms; ti++)
                            {
                                CVTerm term = _species.getCVTerm(ti);
                                if (term.getQualifierType() != libsbml.BIOLOGICAL_QUALIFIER)
                                    continue;

                                qualifierId = qualifierIds[term.getBiologicalQualifierType()];

                                XMLAttributes resources = term.getResources();
                                for (int j = 0; j < resources.getLength(); j++)
                                {
                                    annotation = resources.getValue(j);
                                    if (annotation.IndexOf("kegg") > 0)
                                    {
                                        annotation = annotation.Substring(annotation.LastIndexOf(":") + 1).Trim();
                                        keggIds = AttributeManager.FindItems("CompoundID", annotation);
                                        if (keggIds == null || keggIds.Length == 0)
                                            continue;
                                        foreach (Guid pid in keggIds)
                                        {
                                            if (!ServerMolecularEntity.Exists(pid))
                                                continue;
                                            smsm = new SoapMapSpeciesMolecularEntities(serSpecies.ID, pid, qualifierId);
                                            msm = new ServerMapSpeciesMolecularEntities(smsm);
                                            msm.UpdateDatabase();
                                        }
                                        keggIds = null;
                                    }
                                }
                            }//end of annotation parsing
                        }

                        //****************************************** Initiate Parameter(Global) Object ***************************************************

                        for (int i = 0; i < _model.getNumParameters(); i++)
                        {
                            Parameter _parameter = _model.getParameter(i);

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_parameter.getMetaId() != null)
                                meta = _parameter.getMetaId();

                            if (_parameter.getSBOTerm() != -1)
                                sbo = _parameter.getSBOTerm().ToString();

                            if (_parameter.getNotesString() != null)
                                notes = _parameter.getNotesString();

                            if (_parameter.getAnnotationString() != null)
                                anno = _parameter.getAnnotationString();

                            if (_parameter.getUnits() != "")
                            {
                                if (unitIds.ContainsKey(_parameter.getUnits()))
                                    unitId = unitIds[_parameter.getUnits()];
                                else
                                    unitId = UnitDefinitionManager.GetBaseUnitId(_parameter.getUnits());
                            }
                            else
                                unitId = Guid.Empty;

                            idstr = _parameter.getId().Trim();
                            namestr = _parameter.getName().Trim();
                            if (namestr == "")
                                namestr = idstr;
                            else if (idstr == "")
                                idstr = namestr;

                            SoapParameter sParameter = new SoapParameter(meta, sbo, notes, anno, serverModel.ID, Guid.Empty, idstr, namestr,
                                                                            _parameter.getValue(), unitId, _parameter.getConstant());

                            ServerParameter serParameter = new ServerParameter(sParameter);
                            serParameter.UpdateDatabase();
                        }

                        //****************************** Initiate Reaction, Kinetic Law, Parameter (Local) Object *****************
                        Guid kLawId;
                        for (int i = 0; i < _model.getNumReactions(); i++)
                        {
                            kLawId = Guid.Empty;
                            Reaction _reaction = _model.getReaction(i);
                            KineticLaw _kLaw = _reaction.getKineticLaw();

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";
                            math = "";

                            if (_kLaw != null)
                            {
                                if (_kLaw.getMetaId() != null)
                                    meta = _kLaw.getMetaId();

                                if (_kLaw.getSBOTerm() != -1)
                                    sbo = _kLaw.getSBOTerm().ToString();

                                if (_kLaw.getNotesString() != null)
                                    notes = _kLaw.getNotesString();

                                if (_kLaw.getAnnotationString() != null)
                                    anno = _kLaw.getAnnotationString();

                                if (libsbml.formulaToString(_kLaw.getMath()) != null)
                                    math = libsbml.formulaToString(_kLaw.getMath());

                                SoapKineticLaw sKLaw = new SoapKineticLaw(meta, sbo, notes, anno, math);
                                ServerKineticLaw serKLaw = new ServerKineticLaw(sKLaw);
                                serKLaw.UpdateDatabase();
                                kLawId = serKLaw.ID;
                            }

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_reaction.getMetaId() != null)
                                meta = _reaction.getMetaId();

                            if (_reaction.getSBOTerm() != -1)
                                sbo = _reaction.getSBOTerm().ToString();

                            if (_reaction.getNotesString() != null)
                                notes = _reaction.getNotesString();

                            if (_reaction.getAnnotationString() != null)
                                anno = _reaction.getAnnotationString();


                            idstr = _reaction.getId().Trim();
                            namestr = _reaction.getName().Trim();
                            if (namestr == "")
                                namestr = idstr;
                            else if (idstr == "")
                                idstr = namestr;

                            SoapReaction sReaction = new SoapReaction(meta, sbo, notes, anno, serverModel.ID, idstr, namestr,
                                                                     _reaction.getReversible(), _reaction.getFast(), kLawId);
                            ServerReaction[] existingReactions = ServerReaction.FindReactionsByIds(serverModel.ID, idstr); // Search by model id + sbmlId superkey
                            if (existingReactions.Length == 1)// Compartment is in the database, so update it 
                            {
                                sReaction.ID = existingReactions[0].ID; // Status changes no_change here 
                                sReaction.Status = ObjectStatus.NoChanges; // Without this can not set to update
                                sReaction.Status = ObjectStatus.Update; // setting it back
                            }
                            else if (existingReactions.Length > 1)
                            {
                                Console.WriteLine("Reaction name = " + sReaction.Name + ", id =" + sReaction.ID + " sbmlId =" + sReaction.SbmlId
                                    + " matched more than one reaction in the database");
                                return;
                            }
                            ServerReaction serReaction = new ServerReaction(sReaction);
                            serReaction.UpdateDatabase();

                            //***** Parsing Annotations ******
                            numCVTerms = (int)_reaction.getNumCVTerms();
                            ServerMapReactionsProcessEntities mrp;
                            SoapMapReactionsProcessEntities smrp;
                            ServerMapReactionECNumber smec;
                            goAnnotationsCache.Clear();
                            for (int ti = 0; ti < numCVTerms; ti++)
                            {

                                CVTerm term = _reaction.getCVTerm(ti);

                                if (term.getQualifierType() != libsbml.BIOLOGICAL_QUALIFIER)
                                    continue;
                                qualifierId = qualifierIds[term.getBiologicalQualifierType()];

                                XMLAttributes resources = term.getResources();
                                for (int j = 0; j < resources.getLength(); j++)
                                {
                                    annotation = resources.getValue(j);
                                    if (annotation.IndexOf("kegg") > 0) // parsing KEGG annotation
                                    {
                                        annotation = annotation.Substring(annotation.LastIndexOf(":") + 1).Trim();
                                        keggIds = AttributeManager.FindItems("ReactionID", annotation);
                                        if (keggIds == null || keggIds.Length == 0)
                                            continue;
                                        foreach (Guid pid in keggIds)
                                        {
                                            if (!ServerProcess.Exists(pid))
                                                continue;
                                            smrp = new SoapMapReactionsProcessEntities(serReaction.ID, pid, qualifierId);
                                            mrp = new ServerMapReactionsProcessEntities(smrp);
                                            mrp.UpdateDatabase();
                                        }
                                        keggIds = null;
                                    }
                                    else if (annotation.IndexOf("ec-code") > 0) //parsing EC Number annotation -- ec-code
                                    {
                                        annotation = annotation.Substring(annotation.LastIndexOf(":") + 1).Trim();
                                        if (annotation.StartsWith("EC+"))
                                            annotation = annotation.Substring(3);
                                        else if (annotation.StartsWith("+"))
                                            annotation = annotation.Substring(1);
                                        string[] ecsubs = annotation.Split('.');
                                        for (int z = ecsubs.Length; z < 4; z++)
                                            annotation += ".-";
                                        if (!ECNumbers.ContainsKey(annotation))
                                        {
                                            if (!missedECNumbers.ContainsKey(annotation))
                                                missedECNumbers.Add(annotation, "");
                                            continue;
                                        }
                                        smec = new ServerMapReactionECNumber(serReaction.ID, annotation, qualifierId);
                                        smec.UpdateDatabase();
                                    }
                                    else if (annotation.IndexOf("obo.go") > 0) // parsing GO annotation
                                    {
                                        annotation = annotation.Substring(annotation.Length - 7).Trim();
                                        if (!GOIds.ContainsKey(annotation))
                                        {
                                            if (!missedGOIds.ContainsKey(annotation))
                                                missedGOIds.Add(annotation, "");
                                            continue;
                                        }
                                        if (goAnnotationsCache.ContainsKey(annotation))
                                            continue;
                                        goAnnotationsCache.Add(annotation, "");
                                        smg = new ServerMapSbaseGO(serReaction.ID, annotation, (short)qualifierId);
                                        smg.UpdateDatabase();
                                    }
                                }
                            }
                            //*** end of annotation parsing ***

                            for (int j = 0; j < _kLaw.getNumParameters(); j++)
                            {
                                Parameter _parameter = _kLaw.getParameter(j);

                                meta = "";
                                sbo = "";
                                notes = "";
                                anno = "";
                                math = "";
                                id = "";
                                name = "";
                                value = 0;
                                constant = false;

                                if (_parameter != null)
                                {
                                    if (_parameter.getMetaId() != null)
                                        meta = _parameter.getMetaId();

                                    if (_parameter.getSBOTerm() != -1)
                                        sbo = _parameter.getSBOTerm().ToString();

                                    if (_parameter.getNotesString() != null)
                                        notes = _parameter.getNotesString();

                                    if (_parameter.getAnnotationString() != null)
                                        anno = _parameter.getAnnotationString().ToString();

                                    if (_parameter.getId() != null)
                                        id = _parameter.getId();

                                    if (_parameter.getName() != null)
                                        name = _parameter.getName();

                                    if (_parameter.getValue() != 0)
                                        value = _parameter.getValue();

                                    if (_parameter.getConstant() != false)
                                        constant = _parameter.getConstant();

                                    if (_parameter.getUnits() != "")
                                    {
                                        if (unitIds.ContainsKey(_parameter.getUnits()))
                                            unitId = unitIds[_parameter.getUnits()];
                                        else
                                            unitId = UnitDefinitionManager.GetBaseUnitId(_parameter.getUnits());
                                    }
                                    else
                                        unitId = Guid.Empty;


                                    if (name == "")
                                        name = id;
                                    else if (id == "")
                                        id = name;

                                    SoapParameter sParameter = new SoapParameter(meta, sbo, notes, anno, Guid.Empty, serReaction.ID, id, name, value,
                                                                                 unitId, constant);
                                    ServerParameter serParameter = new ServerParameter(sParameter);
                                    serParameter.UpdateDatabase();
                                }
                            }

                            roleId = ReactionSpeciesRoleManager.GetRoleId("Reactant");
                            for (int j = 0; j < _reaction.getNumReactants(); j++)
                            {
                                Guid mathId = Guid.Empty;

                                SpeciesReference _sReference = _reaction.getReactant(j);
                                double _sStoichiometry = _sReference.getStoichiometry();
                                StoichiometryMath _sMath = _sReference.getStoichiometryMath();

                                if (_sMath == null && _sStoichiometry <= 0)
                                    _sStoichiometry = 1;

                                if (_sMath != null)
                                {
                                    meta = "";
                                    sbo = "";
                                    notes = "";
                                    anno = "";
                                    math = "";

                                    if (_sMath.getMetaId() != null)
                                        meta = _sMath.getMetaId();

                                    if (_sMath.getSBOTerm() != -1)
                                        sbo = _sMath.getSBOTerm().ToString();

                                    if (_sMath.getNotesString() != null)
                                        notes = _sMath.getNotesString();

                                    if (_sMath.getAnnotationString() != null)
                                        anno = _sMath.getAnnotationString();

                                    if (libsbml.formulaToString(_sMath.getMath()) != null)
                                        math = libsbml.formulaToString(_sMath.getMath());

                                    SoapStoichiometryMath sSMath = new SoapStoichiometryMath(meta, sbo, notes, anno, math);
                                    ServerStoichiometryMath serSMath = new ServerStoichiometryMath(sSMath);
                                    serSMath.UpdateDatabase();
                                    mathId = serSMath.ID;
                                }

                                if (_sReference.getSpecies() == "")
                                    speciesId = Guid.Empty;
                                else
                                    speciesId = speciesIds[_sReference.getSpecies()];

                                meta = "";
                                sbo = "";
                                notes = "";
                                anno = "";

                                if (_sReference.getMetaId() != null)
                                    meta = _sReference.getMetaId();

                                if (_sReference.getSBOTerm() != -1)
                                    sbo = _sReference.getSBOTerm().ToString();

                                if (_sReference.getNotesString() != null)
                                    notes = _sReference.getNotesString();

                                if (_sReference.getAnnotationString() != null)
                                    anno = _sReference.getAnnotationString();


                                SoapReactionSpecies sRSpecies = new SoapReactionSpecies(meta, sbo, notes, anno, serReaction.ID, speciesId, (short)roleId, _sStoichiometry,
                                                                                        mathId, _sReference.getId(), _sReference.getName());
                                ServerReactionSpecies serRSpecies = new ServerReactionSpecies(sRSpecies);
                                serRSpecies.UpdateDatabase();
                                _sReference = null;

                            }

                            roleId = ReactionSpeciesRoleManager.GetRoleId("Product");
                            for (int j = 0; j < _reaction.getNumProducts(); j++)
                            {
                                Guid mathId = Guid.Empty;

                                SpeciesReference _sReference = _reaction.getProduct(j);
                                double _sStoichiometry = _sReference.getStoichiometry();
                                StoichiometryMath _sMath = _sReference.getStoichiometryMath();

                                if (_sMath == null && _sStoichiometry <= 0)
                                    _sStoichiometry = 1;

                                if (_sMath != null)
                                {
                                    meta = "";
                                    sbo = "";
                                    notes = "";
                                    anno = "";
                                    math = "";

                                    if (_sMath.getMetaId() != null)
                                        meta = _sMath.getMetaId();

                                    if (_sMath.getSBOTerm() != -1)
                                        sbo = _sMath.getSBOTerm().ToString();

                                    if (_sMath.getNotesString() != null)
                                        notes = _sMath.getNotesString();

                                    if (_sMath.getAnnotationString() != null)
                                        anno = _sMath.getAnnotationString();

                                    if (libsbml.formulaToString(_sMath.getMath()) != null)
                                        math = libsbml.formulaToString(_sMath.getMath());

                                    SoapStoichiometryMath sSMath = new SoapStoichiometryMath(meta, sbo, notes, anno, math);
                                    ServerStoichiometryMath serSMath = new ServerStoichiometryMath(sSMath);
                                    serSMath.UpdateDatabase();
                                    mathId = serSMath.ID;

                                }

                                if (_sReference.getSpecies() == "")
                                    speciesId = Guid.Empty;
                                else
                                    speciesId = speciesIds[_sReference.getSpecies()];

                                meta = "";
                                sbo = "";
                                notes = "";
                                anno = "";

                                if (_sReference.getMetaId() != null)
                                    meta = _sReference.getMetaId();

                                if (_sReference.getSBOTerm() != -1)
                                    sbo = _sReference.getSBOTerm().ToString();

                                if (_sReference.getNotesString() != null)
                                    notes = _sReference.getNotesString();

                                if (_sReference.getAnnotationString() != null)
                                    anno = _sReference.getAnnotationString();

                                SoapReactionSpecies sRSpecies = new SoapReactionSpecies(meta, sbo, notes, anno, serReaction.ID, speciesId, (short)roleId, _sStoichiometry,
                                                                                        mathId, _sReference.getId(), _sReference.getName());
                                ServerReactionSpecies serRSpecies = new ServerReactionSpecies(sRSpecies);
                                serRSpecies.UpdateDatabase();

                            }

                            roleId = ReactionSpeciesRoleManager.GetRoleId("Modifier");
                            for (int j = 0; j < _reaction.getNumModifiers(); j++)
                            {
                                ModifierSpeciesReference _sReference = _reaction.getModifier(j);

                                if (_sReference.getSpecies() == "")
                                    speciesId = Guid.Empty;
                                else
                                    speciesId = speciesIds[_sReference.getSpecies()];

                                meta = "";
                                sbo = "";
                                notes = "";
                                anno = "";

                                if (_sReference.getMetaId() != null)
                                    meta = _sReference.getMetaId();

                                if (_sReference.getSBOTerm() != -1)
                                    sbo = _sReference.getSBOTerm().ToString();

                                if (_sReference.getNotesString() != null)
                                    notes = _sReference.getNotesString();

                                if (_sReference.getAnnotationString() != null)
                                    anno = _sReference.getAnnotationString();


                                SoapReactionSpecies sRSpecies = new SoapReactionSpecies(meta, sbo, notes, anno, serReaction.ID, speciesId, (short)roleId, 0, Guid.Empty,
                                                                                        _sReference.getId(), _sReference.getName());
                                ServerReactionSpecies serRSpecies = new ServerReactionSpecies(sRSpecies);
                                serRSpecies.UpdateDatabase();
                            }
                        }

                        //********************************************* Initiate Constraint Object **********************************

                        for (int i = 0; i < _model.getNumConstraints(); i++)
                        {
                            Constraint _constraint = _model.getConstraint(i);

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_constraint.getMetaId() != null)
                                meta = _constraint.getMetaId();

                            if (_constraint.getSBOTerm() != -1)
                                sbo = _constraint.getSBOTerm().ToString();

                            if (_constraint.getNotesString() != null)
                                notes = _constraint.getNotesString();

                            if (_constraint.getAnnotationString() != null)
                                anno = _constraint.getAnnotationString();


                            SoapConstraint sConstraint = new SoapConstraint(meta, sbo, notes, anno, serverModel.ID, libsbml.formulaToString(_constraint.getMath()),
                                                                            (_constraint.getMessage().toXMLString()));

                            ServerConstraint serConstraint = new ServerConstraint(sConstraint);
                            serConstraint.UpdateDatabase();
                        }

                        //*************************************** Initiate Initial Assignment Object ******************************

                        for (int i = 0; i < _model.getNumInitialAssignments(); i++)
                        {
                            InitialAssignment _iAssignment = _model.getInitialAssignment(i);

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_iAssignment.getMetaId() != null)
                                meta = _iAssignment.getMetaId();

                            if (_iAssignment.getSBOTerm() != -1)
                                sbo = _iAssignment.getSBOTerm().ToString();

                            if (_iAssignment.getNotesString() != null)
                                notes = _iAssignment.getNotesString();

                            if (_iAssignment.getAnnotationString() != null)
                                anno = _iAssignment.getAnnotationString();


                            SoapInitialAssignment sIAssignment = new SoapInitialAssignment(meta, sbo, notes, anno, serverModel.ID, _iAssignment.getSymbol(),
                                                                                           libsbml.formulaToString(_iAssignment.getMath()));
                            ServerInitialAssignment serIAssignment = new ServerInitialAssignment(sIAssignment);
                            serIAssignment.UpdateDatabase();
                        }


                        //*************************************************** Initiate Rule Object *************************************

                        for (int i = 0; i < _model.getNumRules(); i++)
                        {
                            Rule _rule = _model.getRule(i);

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_rule.getMetaId() != null)
                                meta = _rule.getMetaId();

                            if (_rule.getSBOTerm() != -1)
                                sbo = _rule.getSBOTerm().ToString();

                            if (_rule.getNotesString() != null)
                                notes = _rule.getNotesString();

                            if (_rule.getAnnotationString() != null)
                                anno = _rule.getAnnotationString();

                            ruleTypeId = RuleTypeManager.GetTypeId(mapRuleTypes[_rule.getElementName()]);

                            SoapRule sRule = new SoapRule(meta, sbo, notes, anno, serverModel.ID, _rule.getVariable(),
                                                          libsbml.formulaToString(_rule.getMath()), (short)ruleTypeId);

                            ServerRule serRule = new ServerRule(sRule);
                            serRule.UpdateDatabase();
                            _rule = null;

                        }

                        //**************************** Initiate Event , Trigger, Delay , EventAssignment Object ********************
                        Guid triggerId, delayId;
                        for (int i = 0; i < _model.getNumEvents(); i++)
                        {
                            triggerId = Guid.Empty;
                            delayId = Guid.Empty;
                            Event _event = _model.getEvent(i);

                            Trigger _trigger = _event.getTrigger();

                            Delay _delay = _event.getDelay();

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_delay != null)
                            {
                                if (_delay.getMetaId() != null)
                                    meta = _delay.getMetaId();

                                if (_delay.getSBOTerm() != -1)
                                    sbo = _delay.getSBOTerm().ToString();

                                if (_delay.getNotesString() != null)
                                    notes = _delay.getNotesString();

                                if (_delay.getAnnotationString() != null)
                                    anno = _delay.getAnnotationString();

                                if (libsbml.formulaToString(_delay.getMath()) != null)
                                    math = libsbml.formulaToString(_delay.getMath());

                                SoapEventDelay sEventDelay = new SoapEventDelay(meta, sbo, notes, anno, math);
                                ServerEventDelay serEventDelay = new ServerEventDelay(sEventDelay);
                                serEventDelay.UpdateDatabase();
                                delayId = serEventDelay.ID;
                            }

                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_trigger != null)
                            {
                                if (_trigger.getMetaId() != null)
                                    meta = _trigger.getMetaId();

                                if (_trigger.getSBOTerm() != -1)
                                    sbo = _trigger.getSBOTerm().ToString();

                                if (_trigger.getNotesString() != null)
                                    notes = _trigger.getNotesString();

                                if (_trigger.getAnnotationString() != null)
                                    anno = _trigger.getAnnotationString();

                                if (libsbml.formulaToString(_trigger.getMath()) != null)
                                    math = libsbml.formulaToString(_trigger.getMath());

                                SoapEventTrigger sEventTrigger = new SoapEventTrigger(meta, sbo, notes, anno, math);
                                ServerEventTrigger serEventTrigger = new ServerEventTrigger(sEventTrigger);
                                serEventTrigger.UpdateDatabase();
                                triggerId = serEventTrigger.ID;
                            }
                            meta = "";
                            sbo = "";
                            notes = "";
                            anno = "";

                            if (_event.getMetaId() != null)
                                meta = _event.getMetaId();

                            if (_event.getSBOTerm() != -1)
                                sbo = _event.getSBOTerm().ToString();

                            if (_event.getNotesString() != null)
                                notes = _event.getNotesString();

                            if (_event.getAnnotationString() != null)
                                anno = _event.getAnnotationString();

                            idstr = _event.getId().Trim();
                            namestr = _event.getName().Trim();
                            if (namestr == "")
                                namestr = idstr;
                            else if (idstr == "")
                                idstr = namestr;

                            SoapEvent sEvent = new SoapEvent(meta, sbo, notes, anno, serverModel.ID, idstr, namestr, triggerId, delayId);

                            ServerEvent serEvent = new ServerEvent(sEvent);
                            serEvent.UpdateDatabase();

                            for (int j = 0; j < _event.getNumEventAssignments(); j++)
                            {
                                EventAssignment _eventAssignment = _event.getEventAssignment(j);

                                meta = "";
                                sbo = "";
                                notes = "";
                                anno = "";
                                variable = "";
                                math = "";

                                if (_eventAssignment != null)
                                {

                                    if (_eventAssignment.getMetaId() != null)
                                        meta = _eventAssignment.getMetaId();

                                    if (_eventAssignment.getSBOTerm() != -1)
                                        sbo = _eventAssignment.getSBOTerm().ToString();

                                    if (_eventAssignment.getNotesString() != null)
                                        notes = _eventAssignment.getNotesString();

                                    if (_eventAssignment.getAnnotationString() != null)
                                        anno = _eventAssignment.getAnnotationString();

                                    if (_eventAssignment.getVariable() != null)
                                        variable = _eventAssignment.getVariable();

                                    if (libsbml.formulaToString(_eventAssignment.getMath()) != null)
                                        math = libsbml.formulaToString(_eventAssignment.getMath());

                                    SoapEventAssignment sEAssignment = new SoapEventAssignment(meta, sbo, notes, anno, serEvent.ID, variable,
                                                                                               math);
                                    ServerEventAssignment serEAssignment = new ServerEventAssignment(sEAssignment);
                                    serEAssignment.UpdateDatabase();
                                }//end if event assignment is not null
                            }//end event assignment list                            
                        }//end event insertion
                    }//end foreach model
                    Console.WriteLine("Finished parsing files...");
                }//end if directory exists

                AnnotationParser.Parse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.Read();
            }
            finally
            {


                Console.WriteLine("Models with KEGG: " + modelsWithKeggAnnotations.Count);
                Console.WriteLine("Models without KEGG: " + missedModelAnnotations.Count);
                Console.WriteLine("Reactome: " + modelsWithReactome);
                Console.WriteLine("GO: " + modelsWithGO);
                Console.WriteLine("Total Referenced KEGG Pathways: " + pathwaysWithModelAnnotations.Count);
                Console.WriteLine("Missed Pathways2: " + missedPathways2);
                Console.WriteLine("MissedPathwayAnnotations: " + missedPathwayAnnotations.Count);
                foreach (string s in missedPathwayAnnotations.Keys)
                    Console.WriteLine(s);

                Console.WriteLine();

                Console.WriteLine("ncbiDifKEGGOrgs: " + ncbiDifKEGGOrgs);
                Console.WriteLine("ncbiDifOrgDB: " + ncbiDifOrgDB);
                Console.WriteLine("KEGGDifOrgDB: " + KEGGDifOrgDB);
                Console.WriteLine("MissedGOIds: " + missedGOIds.Count);
                foreach (string t in missedGOIds.Keys)
                    Console.WriteLine("\t" + t);

                Console.WriteLine("MissedECNumbers: " + missedECNumbers.Count);
                foreach (string t in missedECNumbers.Keys)
                    Console.WriteLine("\t" + t);

                Console.Write("Hit Enter to Exit!");
                Console.Read();
            }

        }//main
    }//class
}//namespace
