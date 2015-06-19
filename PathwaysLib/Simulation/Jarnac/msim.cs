using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using SBW;
namespace Jarnac
{
    public class msim
    {
        public msim() { }
        private static int _nModuleID = new Module("Jarnac").ID;
        private static int _nServiceID = SBWLowLevel.moduleFindServiceByName(_nModuleID, "msim");
        private static int _nMethod0 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "int createModel ()");

        ///<summary>
        ///Start a new model instance, returns model handle
        ///</summary>
        public static int createModel()
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                return (int)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod0, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod1 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "void freeModel (int modelHandle)");

        ///<summary>
        ///Destroy a model given it's model handle
        ///</summary>
        public static void freeModel(int modelHandle)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(modelHandle);
                SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod1, oArguments);
                return;
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod2 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "void loadSBML(int, string)");

        ///<summary>
        ///Load the sbml model contained in the string argument
        ///</summary>
        public static void loadSBML(int modelHandle, string var1)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(modelHandle);
                oArguments.add(var1);
                SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod2, oArguments);
                return;
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod3 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "void loadJarnac(int, string)");

        ///<summary>
        ///Load the Jarnac script model contained in the string argument
        ///</summary>
        public static void loadJarnac(int var0, string var1)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod3, oArguments);
                return;
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod4 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "void changeInitialConditions (int, double[])");

        ///<summary>
        ///Load the initial conditions vector with a new vector
        ///</summary>
        public static void changeInitialConditions(int var0, double[] var1)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod4, oArguments);
                return;
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod5 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "void setTimeStart (int, double)");

        ///<summary>
        ///Set the time start for the simulation
        ///</summary>
        public static void setTimeStart(int var0, double var1)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod5, oArguments);
                return;
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod6 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "void setTimeEnd (int, double)");

        ///<summary>
        ///Set the time end for the simulation
        ///</summary>
        public static void setTimeEnd(int var0, double var1)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod6, oArguments);
                return;
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod7 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "void setNumPoints (int, int)");

        ///<summary>
        ///Set the number of output data points
        ///</summary>
        public static void setNumPoints(int var0, int var1)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod7, oArguments);
                return;
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod8 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "void setSelectionList (int, {})");

        ///<summary>
        ///Sets the list of variables to return when calling simulate(), eg setSelectionList(handle, {'Time', 'S1', 'J0'
        ///</summary>
        public static void setSelectionList(int var0, ArrayList var1)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod8, oArguments);
                return;
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod9 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "void reset (int)");

        ///<summary>
        ///Reset the model to it's initial conditions
        ///</summary>
        public static void reset(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod9, oArguments);
                return;
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod10 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double oneStep (int, double, double)");

        ///<summary>
        ///Carry out one step in the simulation. The method takes two arguments, the current time and the step size to use in the next integration step. The method returns the time at the new point. Usage: t = oneStep (modelHandle, t, 0.1)
        ///</summary>
        public static double oneStep(int var0, double var1, double var2)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                oArguments.add(var2);
                return (double)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod10, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod11 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[][] simulate (int)");

        ///<summary>
        ///Carry out a simulation using a stiff ODE itegrator
        ///</summary>
        public static double[][] simulate(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double[][])HighLevel.convertArray(SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod11, oArguments).getObject());
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod12 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double steadyState (int)");

        ///<summary>
        ///Compute the steady state, returns the sum fo squares indicating the quality of the solution
        ///</summary>
        public static double steadyState(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod12, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod13 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[][] getReducedJacobian (int)");

        ///<summary>
        ///Get the current reduced jacobian matrix. This call will return the reduced matrix, that is dependent species have been removed
        ///</summary>
        public static double[][] getReducedJacobian(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double[][])HighLevel.convertArray(SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod13, oArguments).getObject());
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod14 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[][] getFullJacobian (int)");

        ///<summary>
        ///Get the current full jacobian matrix.
        ///</summary>
        public static double[][] getFullJacobian(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double[][])HighLevel.convertArray(SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod14, oArguments).getObject());
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod15 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[][] evalModel (double, double, double[], double[])");

        ///<summary>
        ///Get the current state of the model, arg1 = time, arg2 = variable array, arg3 = parameter array
        ///</summary>
        public static double[][] evalModel(double var0, double var1, double[] var2, double[] var3)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                oArguments.add(var2);
                oArguments.add(var3);
                return (double[][])HighLevel.convertArray(SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod15, oArguments).getObject());
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod16 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double getuCC (int, string, string)");

        ///<summary>
        ///Returns the unscaled control coefficient
        ///</summary>
        public static double getuCC(int var0, string var1, string var2)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                oArguments.add(var2);
                return (double)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod16, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod17 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double getCC (int, string, string)");

        ///<summary>
        ///Returns the scaled control coefficient
        ///</summary>
        public static double getCC(int var0, string var1, string var2)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                oArguments.add(var2);
                return (double)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod17, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod18 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double getuEE (int, string, string)");

        ///<summary>
        ///Returns the unscaled elasticity coefficient
        ///</summary>
        public static double getuEE(int var0, string var1, string var2)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                oArguments.add(var2);
                return (double)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod18, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod19 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double getEE (int, string, string)");

        ///<summary>
        ///Returns the scaled elasticity coefficient
        ///</summary>
        public static double getEE(int var0, string var1, string var2)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                oArguments.add(var2);
                return (double)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod19, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod20 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[][] gillespie (int)");

        ///<summary>
        ///Carry out a simulation using the Gillespie method
        ///</summary>
        public static double[][] gillespie(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double[][])HighLevel.convertArray(SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod20, oArguments).getObject());
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod21 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "void setValue (int, string, double)");

        ///<summary>
        ///Set the value of the named symbol, eg setValue (id, \"ATP\", 1.2\")
        ///</summary>
        public static void setValue(int var0, string var1, double var2)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                oArguments.add(var2);
                SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod21, oArguments);
                return;
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod22 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double getValue (int, string)");

        ///<summary>
        ///Get the value of the named symbol, eg x = getValue (id, \"ATP\")
        ///</summary>
        public static double getValue(int var0, string var1)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                return (double)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod22, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod23 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[] getReactionRates (int)");

        ///<summary>
        ///Get the reaction rates as a vector
        ///</summary>
        public static double[] getReactionRates(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double[])SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod23, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod24 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double getReactionRate (int, int)");

        ///<summary>
        ///Get a particular reaction rate (id, ithReaction)
        ///</summary>
        public static double getReactionRate(int var0, int var1)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                return (double)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod24, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod25 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[] getRatesOfChange (int)");

        ///<summary>
        ///Get the rates of change as a vector
        ///</summary>
        public static double[] getRatesOfChange(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double[])SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod25, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod26 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "{} getFloatingSpeciesNames(int)");

        ///<summary>
        ///Get the list of floating species names
        ///</summary>
        public static ArrayList getFloatingSpeciesNames(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (ArrayList)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod26, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod27 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[] getFloatingSpeciesValues(int)");

        ///<summary>
        ///Get an array of floating species values
        ///</summary>
        public static double[] getFloatingSpeciesValues(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double[])SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod27, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod28 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "{} getReactionNames(int)");

        ///<summary>
        ///Get the list of reaction names
        ///</summary>
        public static ArrayList getReactionNames(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (ArrayList)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod28, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod29 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[] getBoundarySpeciesValues ()");

        ///<summary>
        ///Get the list of boundary species values
        ///</summary>
        public static double[] getBoundarySpeciesValues()
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                return (double[])SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod29, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod30 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "{} getBoundarySpeciesNames ()");

        ///<summary>
        ///Get the list of boundary species names
        ///</summary>
        public static ArrayList getBoundarySpeciesNames()
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                return (ArrayList)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod30, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod31 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "int getNumLocalParameters(int)");

        ///<summary>
        ///Get the number of local parameters in the model
        ///</summary>
        public static int getNumLocalParameters(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (int)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod31, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod32 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[] getAllLocalParameterValues(int)");

        ///<summary>
        ///Get the list of local parameter values in the model
        ///</summary>
        public static double[] getAllLocalParameterValues(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double[])SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod32, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod33 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "{} getAllLocalParameterNames(int)");

        ///<summary>
        ///Get the list of parameters in the model
        ///</summary>
        public static ArrayList getAllLocalParameterNames(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (ArrayList)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod33, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod34 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "int getNumFloatingSpecies(int)");

        ///<summary>
        ///Get the number of floating species
        ///</summary>
        public static int getNumFloatingSpecies(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (int)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod34, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod35 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "int getNumBoundarySpecies(int)");

        ///<summary>
        ///Get the number of boundary species
        ///</summary>
        public static int getNumBoundarySpecies(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (int)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod35, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod36 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "string getFloatingSpeciesName(int, int)");

        ///<summary>
        ///Get the ith floating species name
        ///</summary>
        public static string getFloatingSpeciesName(int var0, int var1)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                return (string)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod36, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod37 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "string getBoundarySpeciesName(int, int)");

        ///<summary>
        ///Get the ith boundary species name
        ///</summary>
        public static string getBoundarySpeciesName(int var0, int var1)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                return (string)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod37, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod38 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "{} getODEList (int)");

        ///<summary>
        ///Returns the model as a list of ODE equation strings
        ///</summary>
        public static ArrayList getODEList(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (ArrayList)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod38, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod39 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "{} getRateLawList (int)");

        ///<summary>
        ///Returns the models' list of rate laws
        ///</summary>
        public static ArrayList getRateLawList(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (ArrayList)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod39, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod40 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[][] getConservationLawArray (int)");

        ///<summary>
        ///Returns the conservation law array
        ///</summary>
        public static double[][] getConservationLawArray(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double[][])HighLevel.convertArray(SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod40, oArguments).getObject());
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod41 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[][] getStoichiometryMatrix ()");

        ///<summary>
        ///Returns the stoichiometry matrix
        ///</summary>
        public static double[][] getStoichiometryMatrix()
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                return (double[][])HighLevel.convertArray(SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod41, oArguments).getObject());
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod42 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[][] getNrMatrix (int)");

        ///<summary>
        ///Returns the reduced stoichiometry matrix
        ///</summary>
        public static double[][] getNrMatrix(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double[][])HighLevel.convertArray(SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod42, oArguments).getObject());
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod43 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[][] getLinkMatrix (int)");

        ///<summary>
        ///Returns the link matrix
        ///</summary>
        public static double[][] getLinkMatrix(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double[][])HighLevel.convertArray(SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod43, oArguments).getObject());
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod44 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "double[][] getUnScaledElasticityMatrix (int)");

        ///<summary>
        ///Returns the matrix of unscaled elasticities
        ///</summary>
        public static double[][] getUnScaledElasticityMatrix(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (double[][])HighLevel.convertArray(SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod44, oArguments).getObject());
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod45 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "int getNumReactions (int)");

        ///<summary>
        ///Returns the number of reaction in the current model
        ///</summary>
        public static int getNumReactions(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (int)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod45, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod46 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "string getCapabilities (int)");

        ///<summary>
        ///Returns the simulators' capabilities
        ///</summary>
        public static string getCapabilities(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (string)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod46, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod47 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "{} getIntegrationMethodList (int)");

        ///<summary>
        ///Returns the list simulator names
        ///</summary>
        public static ArrayList getIntegrationMethodList(int var0)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                return (ArrayList)SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod47, oArguments).getObject();
            }
            catch (SBWException e)
            {
                throw e;
            }
        }
        private static int _nMethod48 = SBWLowLevel.serviceGetMethod(_nModuleID, _nServiceID, "void setIntegrationMethod (int, string)");

        ///<summary>
        ///Sets the integration method
        ///</summary>
        public static void setIntegrationMethod(int var0, string var1)
        {
            try
            {
                DataBlockWriter oArguments = new DataBlockWriter();
                oArguments.add(var0);
                oArguments.add(var1);
                SBWLowLevel.methodCall(_nModuleID, _nServiceID, _nMethod48, oArguments);
                return;
            }
            catch (SBWException e)
            {
                throw e;
            }
        }

    }
}