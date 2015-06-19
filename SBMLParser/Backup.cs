#region "declaration region"
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using PathwaysLib.ServerObjects;
using PathwaysLib.SoapObjects;
using libsbml;
#endregion


/***********************************************************************************************************************************************************************************
 *     SBML Parser Instructions
 * *********************************************************************************************************************************************************************************
 * The parser can either be run within Visual Studio, or SBMLParser.exe file in ./SBMLParser/bin/debug folder can be invoked from the console screen.
 * 
 * It makes use of libSBML library to parse SBML files. Related DLL files that need to be added into the project references are located in
 *     ./SBMLParser/DLLs folder.
 * 
 * This program requires the existence and proper setting of the following parameters in the App.config file:  
 *  (1) dbConnectString: database connection string
 *      e.g.: <add key="dbConnectString" value="Persist Security Info=False;User ID=pathcase;Password=dblab;Initial Catalog=PathCase_SystemBiology_Test2;Data Source=dblab.case.edu;"/>
 *  (2) modelDirectory: full path to the directory that contains SBML files to be parsed 
 *      (this folder should not contain any other files as the parser does not check extension or type of input files)
 *      e.g.: <add key="modelDirectory" value="C:\Ali\BioModels\release_03December2008_sbmls\curated"/>
 * 
 * NOTE: After parsing the last file, the program may throw an "Access Violation" exception. This exception is generated within libSBML Dlls, and
 * the cause of the exception has not been resolved yet. However, this exception is thrown after everything finishes. Thus, it does not seem
 * that this exception has any effect on the parsing process -- but still annoying.
 * ********************************************************************************************************************************************************************************/
    

namespace PathwaysLib.SBMLParser
{
    class Program
    {

        static void Main(string[] args)
        {
            // we are retriving sql connection string from app.config file. 
            try
            {
                string strCon = System.Configuration.ConfigurationSettings.AppSettings["dbConnectString"];
                DBWrapper.Instance = new DBWrapper(strCon);

                // Delete all data from all tables

                Console.WriteLine("Cleaning the database...");
                DBreset D = new DBreset();
                D.ClearDB();

                /************************* Initializing the Dictionaries ************************************************/

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
                string[] roles ={ "Reactant", "Product", "Modifier" };

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
                string[] ruleType ={ "Algebraic Rule", "Assignment Rule", "Rate Rule" };

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

                string _directoryPath = System.Configuration.ConfigurationSettings.AppSettings["modelDirectory"]; // The directory path to all the SBML files.

                //string _directoryPath = "C:\\case\\MS_Project\\ConsoleApplication1\\ConsoleApplication1\\ConsoleApplication1\\bin\\curated\\BIOMD0000000015.xml";
                //string _directoryPath = "C:\\case\\Biomodel_files\\release_03December2008_sbmls\\curated";

                SBMLReader _sbmlReader = new SBMLReader();
                
                //string _fileName;
                //bool skip = false;

                /**** Model Files with Problems (always fail)
                 * BIOMD0000000064.xml -- getAnnotationString -- libSBML PINVOKE Exception
                 * BIOMD0000000150.xml -- getAnnotationString -- libSBML PINVOKE Exception
                 * BIOMD0000000183.xml -- getAnnotationString -- libSBML PINVOKE Exception
                 * BIOMD0000000184.xml -- getAnnotationString -- libSBML PINVOKE Exception
                 * BIOMD0000000185.xml -- getAnnotationString -- libSBML PINVOKE Exception
                 * BIOMD0000000188.xml -- getAnnotationString -- libSBML PINVOKE Exception
                 * BIOMD0000000189.xml -- getAnnotationString -- libSBML PINVOKE Exception
                 * 
                 * *** Model Files with (Maybe) Problems (sometimes fail)
                 * BIOMD0000000057.xml
                 * BIOMD0000000067.xml
                 * BIOMD0000000074.xml
                 * BIOMD0000000075.xml
                 * BIOMD0000000084.xml
                 * BIOMD0000000086.xml
                 */

                SBMLDocument _sbmlDocument;
                ArrayList docs = new ArrayList(); // dummy container for SBMLDocument objects of libSBML. The goal is to prevent garbage collector from disposing 
                                                  // document objects, which leads to a generic exception with no known way to resolve.

                if (Directory.Exists(_directoryPath)) // check if the directory exists
                {
                    foreach (string _fileName in Directory.GetFiles(_directoryPath))  // get all the files in that directory
                    {
                        //if (_fileName.Contains("57.xml"))
                        //    skip = false;
                        //if (skip)
                        //    continue;

                        //_fileName = "C:\\case\\Biomodel_files\\release_03December2008_sbmls\\curated\\BIOMD0000000.xml";

                        Console.WriteLine("Processing model file: " + _fileName.Substring(_fileName.LastIndexOf("\\") + 1));

                        //******************************************  empty id caches *********************************************

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

                        SoapModel soapModel = new SoapModel(meta, sbo, notes, anno, _model.getId(), _model.getName(),
                                                            (int)_model.getLevel(), (int)_model.getVersion(), dsource.ID, modelString);

                        ServerModel serverModel = new ServerModel(soapModel);
                        serverModel.UpdateDatabase();

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

                            SoapFunctionDefinition sFunctionDefinition = new SoapFunctionDefinition(meta, sbo, notes, anno, serverModel.ID, _functionDefinition.getId(),
                                                                                                       _functionDefinition.getName(), libsbml.libsbml.formulaToString(_functionDefinition.getMath()));
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

                            SoapUnitDefinition sUnitDefinition = new SoapUnitDefinition(meta, sbo, notes, anno, serverModel.ID, _unitDefinition.getId(), _unitDefinition.getName(), false);
                            ServerUnitDefinition serUnitDefinition = new ServerUnitDefinition(sUnitDefinition);

                            serUnitDefinition.UpdateDatabase();

                            ListOfUnits _listOfUnits = _unitDefinition.getListOfUnits();

                            for (int j = 0; j < _listOfUnits.size(); j++)
                            {
                                Unit _unit = (Unit)_listOfUnits.get(j);
                                unitKind = libsbml.libsbml.UnitKind_toString(_unit.getKind());
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

                            SoapCompartmentType sCompartmentType = new SoapCompartmentType(meta, sbo, notes, anno, serverModel.ID, _compartmentType.getId(), _compartmentType.getName());

                            ServerCompartmentType serCompartmentType = new ServerCompartmentType(sCompartmentType);
                            serCompartmentType.UpdateDatabase();
                            compartmentTypeIds.Add(serCompartmentType.SbmlId, serCompartmentType.ID);
                        }

                        //*********************************** Initiate Compartment Object *******************************


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

                            int SD = _compartment.getSpatialDimensions();
                            if (SD < 0 || SD > 3)
                                SD = 3;
                            
                            SoapCompartment sCompartment = new SoapCompartment(meta, sbo, notes, anno, serverModel.ID, _compartment.getId(), _compartment.getName(), compartmentTypeId,
                                                                                  SD, (float)_compartment.getSize(), unitId, outsideId, _compartment.getConstant());

                            ServerCompartment serCompartment = new ServerCompartment(sCompartment);

                            //DO NOT insert compartment into the database yet -- needs prioritization, which is handled below.
                            //serCompartment.UpdateDatabase();
                            compartments.Add(serCompartment.ID, serCompartment);
                            compartmentIds.Add(serCompartment.SbmlId, serCompartment.ID);
                            compartmentOutsideIds.Add(serCompartment.ID, _compartment.getOutside());                            
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


                            SoapSpeciesType sSpeciesType = new SoapSpeciesType(meta, sbo, notes, anno, serverModel.ID, _speciesType.getId(), _speciesType.getName());
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

                            SoapSpecies sSpecies = new SoapSpecies(meta, sbo, notes, anno, serverModel.ID, _species.getId(), _species.getName(), speciesTypeId, compartmentId, _species.getInitialAmount(),
                                                                                _species.getInitialConcentration(), unitId, _species.getHasOnlySubstanceUnits(), _species.getBoundaryCondition(),
                                                                                _species.getCharge(), _species.getConstant());

                            ServerSpecies serSpecies = new ServerSpecies(sSpecies);
                            serSpecies.UpdateDatabase();

                            speciesIds.Add(serSpecies.SbmlId, serSpecies.ID);
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

                            SoapParameter sParameter = new SoapParameter(meta, sbo, notes, anno, serverModel.ID, Guid.Empty, _parameter.getId(), _parameter.getName(),
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

                                if (libsbml.libsbml.formulaToString(_kLaw.getMath()) != null)
                                    math = libsbml.libsbml.formulaToString(_kLaw.getMath());

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


                            SoapReaction sReaction = new SoapReaction(meta, sbo, notes, anno, serverModel.ID, _reaction.getId(), _reaction.getName(),
                                                                     _reaction.getReversible(), _reaction.getFast(), kLawId);
                            ServerReaction serReaction = new ServerReaction(sReaction);
                            serReaction.UpdateDatabase();

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

                                    if (libsbml.libsbml.formulaToString(_sMath.getMath()) != null)
                                        math = libsbml.libsbml.formulaToString(_sMath.getMath());

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

                                    if (libsbml.libsbml.formulaToString(_sMath.getMath()) != null)
                                        math = libsbml.libsbml.formulaToString(_sMath.getMath());
                                
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


                            SoapConstraint sConstraint = new SoapConstraint(meta, sbo, notes, anno, serverModel.ID, libsbml.libsbml.formulaToString(_constraint.getMath()),
                                                                            libsbml.XMLNode.convertXMLNodeToString(_constraint.getMessage()));

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
                                                                                           libsbml.libsbml.formulaToString(_iAssignment.getMath()));
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
                                                          libsbml.libsbml.formulaToString(_rule.getMath()), (short)ruleTypeId);

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

                                if (libsbml.libsbml.formulaToString(_delay.getMath()) != null)
                                    math = libsbml.libsbml.formulaToString(_delay.getMath());

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

                                if (libsbml.libsbml.formulaToString(_trigger.getMath()) != null)
                                    math = libsbml.libsbml.formulaToString(_trigger.getMath());

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

                            SoapEvent sEvent = new SoapEvent(meta, sbo, notes, anno, serverModel.ID, _event.getId(), _event.getName(), triggerId, delayId);

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

                                    if (libsbml.libsbml.formulaToString(_eventAssignment.getMath()) != null)
                                        math = libsbml.libsbml.formulaToString(_eventAssignment.getMath());

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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.Read();
            }
            finally
            {
                Console.Write("Hit Enter to Exit!");
                Console.Read();
            }
        }//main
    }//class
}//namespace
