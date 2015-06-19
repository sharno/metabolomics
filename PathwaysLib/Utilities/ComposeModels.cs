using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libsbmlcs;

namespace PathwaysLib.Utilities
{
    /// <summary>
    /// Functions necessary for Compose Model functionality.
    /// </summary>
    public static class ComposeModelFunctions
    {

        /// <summary>
        /// This method is used to test whether two files can be composed via AutoMerge.
        /// </summary>
        /// <param name="model1">First model in string format.</param>
        /// <param name="model2">Second model in string format.</param>
        /// <returns></returns>
        public static bool ComposeForTest(string model1, string model2)
        {
            SBMLReader reader = new SBMLReader();
            var sbml1 = reader.readSBMLFromString(model1).getModel();
            var sbml2 = reader.readSBMLFromString(model2).getModel();

            return Compose(sbml1, sbml2, new Model(sbml1), new List<KeyValuePair<string, AttributeTypeEnum>>(), new List<string>());
        }

        /// <summary>
        /// Composes two models (model1 and model2) into modelComposed.
        /// </summary>
        /// <param name="model1">First model to feed composition method.</param>
        /// <param name="model2">Second model to feed composition method.</param>
        /// <param name="modelComposed">Two models will be composed into this model. By default send a new model contains model1. e.g. var modelComposed = new Model(model1)</param>
        /// <param name="DiffOfMergedElements">Stores the modifications on the merged elements.</param>
        /// <param name="ModifiedIds">Ids of the modified elements.</param>
        /// <returns>True if there is no exception, false otherwise.</returns>
        public static bool Compose(Model model1, Model model2, Model modelComposed, List<KeyValuePair<string, AttributeTypeEnum>> DiffOfMergedElements, List<string> ModifiedIds)
        {
            try
            {
                #region Compartment matching
                var tmpCompartments = DiffOfMergedElements;
                tmpCompartments.Add(new KeyValuePair<string, AttributeTypeEnum>("Compartment:", AttributeTypeEnum.Separator));

                // Find the compartments which have the same NAME and ID in both model1 and model2.
                for (int i2 = 0; i2 < model2.getNumCompartments(); i2++)
                {
                    bool isCompartmentFound = false;
                    for (int i1 = 0; i1 < model1.getNumCompartments(); i1++)
                    {
                        if (model1.getCompartment(i1).getId().Equals(model2.getCompartment(i2).getId()))
                        {
                            // If name exists in model1 do not add it to composed model.
                            isCompartmentFound = true;

                            if (!model1.getCompartment(i1).getUnits().Equals(model2.getCompartment(i2).getUnits()))
                            {   // if units are different add to warnings
                                tmpCompartments.Add(new KeyValuePair<string, AttributeTypeEnum>("Compartment " + model1.getCompartment(i1).getId(), AttributeTypeEnum.CompartmentUnits));
                            }
                            if (!model1.getCompartment(i1).getSize().Equals(model2.getCompartment(i2).getSize()))
                            {   // if sizes are different add to warnings
                                tmpCompartments.Add(new KeyValuePair<string, AttributeTypeEnum>("Compartment " + model1.getCompartment(i1).getId(), AttributeTypeEnum.CompartmentSize));
                            }

                            break;
                        }
                    }
                    if (!isCompartmentFound)
                    {
                        // If name does not exist in model 1 then add it to the composed model.
                        modelComposed.addCompartment(model2.getCompartment(i2));
                    }
                }
                DiffOfMergedElements = tmpCompartments;
                tmpCompartments = null;
                #endregion

                #region Rule matching

                // Find the rules which have the same name in both model1 and model2.
                for (int i2 = 0; i2 < model2.getNumRules(); i2++)
                {
                    bool isRuleFound = false;
                    for (int i1 = 0; i1 < model1.getNumRules(); i1++)
                    {
                        if (model1.getRule(i1).getName().Equals(model2.getRule(i2).getName()))
                        {
                            // If name exists in model1 do not add it to composed model.
                            isRuleFound = true;
                            break;
                        }
                    }
                    if (!isRuleFound)
                    {
                        // If name does not exist in model 1 then add it to the composed model.
                        modelComposed.addRule(model2.getRule(i2));
                    }
                }
                #endregion

                #region Initial assignments matching
                // Find the initial assignments which have the same name in both model1 and model2.
                for (int i2 = 0; i2 < model2.getNumInitialAssignments(); i2++)
                {
                    bool isInitialAssignmentFound = false;
                    for (int i1 = 0; i1 < model1.getNumRules(); i1++)
                    {
                        if (model1.getInitialAssignment(i1).getSymbol().Equals(model2.getInitialAssignment(i2).getSymbol()))
                        {
                            // If name exists in model1 do not add it to composed model.
                            isInitialAssignmentFound = true;
                            break;
                        }
                    }
                    if (!isInitialAssignmentFound)
                    {
                        // If name does not exist in model 1 then add it to the composed model.
                        modelComposed.addInitialAssignment(model2.getInitialAssignment(i2));
                    }
                }
                #endregion

                #region Species matching
                var tmpSpecies = DiffOfMergedElements;
                tmpSpecies.Add(new KeyValuePair<string, AttributeTypeEnum>("Species:", AttributeTypeEnum.Separator));

                // Find the species which have the same name in both model1 and model2.
                for (int i2 = 0; i2 < model2.getNumSpecies(); i2++)
                {
                    bool isSpeciesFound = false;
                    for (int i1 = 0; i1 < model1.getNumSpecies(); i1++)
                    {
                        if (model1.getSpecies(i1).getName().Equals(model2.getSpecies(i2).getName()) &&
                            model1.getSpecies(i1).getCompartment().Equals(model2.getSpecies(i2).getCompartment()))
                        {
                            // If name exists in model1 do not add it to composed model.
                            isSpeciesFound = true;
                            if (!model1.getSpecies(i1).getUnits().Equals(model2.getSpecies(i2).getUnits()))
                            {   // if substanceUnits are different add to warnings
                                tmpSpecies.Add(new KeyValuePair<string, AttributeTypeEnum>("Species " + model1.getSpecies(i1).getId(), AttributeTypeEnum.SubstanceUnits));
                            }
                            if (!model1.getSpecies(i1).getInitialAmount().Equals(model2.getSpecies(i2).getInitialAmount()))
                            {// if initialAmounts are different add to warnings
                                tmpSpecies.Add(new KeyValuePair<string, AttributeTypeEnum>("Species " + model1.getSpecies(i1).getId(), AttributeTypeEnum.InitialAmount));
                            }
                            break;
                        }
                    }
                    if (!isSpeciesFound)
                    {
                        var speciesToAdd = model2.getSpecies(i2);

                        // if same id is used in model 1, then prefix the id with model2_
                        if (modelComposed.getSpecies(speciesToAdd.getId()) != null)
                        {
                            var id = modelComposed.getSpecies(speciesToAdd.getId()).getId();
                            var name = modelComposed.getSpecies(speciesToAdd.getId()).getName();

                            speciesToAdd.setId(AddToModifiedIds(id, ModifiedIds));
                            speciesToAdd.setName("model2_" + name);
                        }

                        // If name does not exist in model 1 then add it to the composed model.
                        modelComposed.addSpecies(speciesToAdd);
                    }
                }
                DiffOfMergedElements = tmpSpecies;
                tmpSpecies = null;
                #endregion

                #region Parameter matching
                var tmpParams = DiffOfMergedElements;
                tmpParams.Add(new KeyValuePair<string, AttributeTypeEnum>("Parameter:", AttributeTypeEnum.Separator));
                // Find the parameters which have the same name in both model1 and model2.
                for (int i2 = 0; i2 < model2.getNumParameters(); i2++)
                {
                    bool isParameterFound = false;
                    for (int i1 = 0; i1 < model1.getNumParameters(); i1++)
                    {
                        if (model1.getParameter(i1).getName().Equals(model2.getParameter(i2).getName()))
                        {
                            // If name exists in model1 do not add it to composed model.
                            isParameterFound = true;

                            if (!model1.getParameter(i1).getValue().Equals(model2.getParameter(i2).getValue()))
                            {// if parameter values are different add to warnings
                                tmpParams.Add(new KeyValuePair<string, AttributeTypeEnum>("Parameter " + model1.getParameter(i1).getId(), AttributeTypeEnum.Value));
                            }

                            break;
                        }
                    }
                    if (!isParameterFound)
                    {
                        // If name does not exist in model 1 then add it to the composed model.
                        modelComposed.addParameter(model2.getParameter(i2));
                    }
                    else
                    {
                        var parameterToAdd = model2.getParameter(i2);

                        // if same id is used in model 1, then prefix the id with model2_
                        if (modelComposed.getParameter(parameterToAdd.getId()) != null)
                        {
                            var id = modelComposed.getParameter(parameterToAdd.getId()).getId();
                            var name = modelComposed.getParameter(parameterToAdd.getId()).getName();

                            parameterToAdd.setId(AddToModifiedIds(id, ModifiedIds));
                            parameterToAdd.setName("model2_" + name);

                            // Update Kinetic Law
                            for (int r = 0; r < model2.getNumReactions(); r++)
                            {
                                var currentMath = model2.getReaction(r).getKineticLaw().getMath();
                                var newNode = new ASTNode();
                                newNode.setName(parameterToAdd.getId());
                                currentMath.replaceArgument(name, newNode);
                            }
                        }

                        modelComposed.addParameter(parameterToAdd);
                    }
                }

                DiffOfMergedElements = tmpParams;
                tmpParams = null;
                #endregion

                #region Reaction matching
                var tmpReact = DiffOfMergedElements;
                tmpReact.Add(new KeyValuePair<string, AttributeTypeEnum>("Reaction:", AttributeTypeEnum.Separator));
                // Find the reactions which have the same name in both model1 and model2.
                for (int i2 = 0; i2 < model2.getNumReactions(); i2++)
                {
                    bool isReactionFound = false;
                    for (int i1 = 0; i1 < model1.getNumReactions(); i1++)
                    {
                        if (model1.getReaction(i1).getId().Equals(model2.getReaction(i2).getId()) &&
                            model1.getReaction(i1).getCompartment().Equals(model2.getReaction(i2).getCompartment()))
                        {
                            // Assume that the reactions are same before checking reactants and products.
                            isReactionFound = true;

                            // Temporary list to store different attributes of reactants, modifiers and products until we are sure both reactions are same(will be merged) in the end.
                            var tmpDiffOfMergedReactionElements = new List<KeyValuePair<string, AttributeTypeEnum>>();

                            var model2Reaction = model2.getReaction(i2);
                            var model1Reaction = model1.getReaction(i1);

                            // Sync reactants in the reactions which have the same name.
                            for (int i3 = 0; i3 < model2Reaction.getNumReactants(); i3++)
                            {
                                bool isReactantFound = false;
                                for (int i4 = 0; i4 < model1Reaction.getNumReactants(); i4++)
                                {
                                    if (model1Reaction.getReactant(i4).getSpecies().Equals(FindModifiedId(model2Reaction.getReactant(i3).getSpecies(), ModifiedIds)) &&
                                        model1.getSpecies(model1Reaction.getReactant(i4).getSpecies()).getCompartment().Equals(model2.getSpecies(FindModifiedId(model2Reaction.getReactant(i3).getSpecies(), ModifiedIds)).getCompartment()))
                                    {
                                        isReactantFound = true;

                                        if (!model1Reaction.getReactant(i4).getStoichiometry().Equals(model2Reaction.getReactant(i3).getStoichiometry()))
                                        {
                                            tmpDiffOfMergedReactionElements.Add(new KeyValuePair<string, AttributeTypeEnum>("Reactant " + model1Reaction.getReactant(i4).getSpecies(), AttributeTypeEnum.Stoichiometry));
                                        }

                                        break;
                                    }
                                }
                                if (!isReactantFound)
                                {
                                    isReactionFound = false;
                                    break;
                                }
                            }

                            // If reactants or products of the reaction are not matching
                            if (!isReactionFound) { break; }

                            // Sync products in the reactions which have the same name.
                            for (int i3 = 0; i3 < model2Reaction.getNumProducts(); i3++)
                            {
                                bool isProductFound = false;
                                for (int i4 = 0; i4 < model1Reaction.getNumProducts(); i4++)
                                {
                                    if (model1Reaction.getProduct(i4).getSpecies().Equals(FindModifiedId(model2Reaction.getProduct(i3).getSpecies(), ModifiedIds)) &&
                                        model1.getSpecies(model1Reaction.getProduct(i4).getSpecies()).getCompartment().Equals(model2.getSpecies(FindModifiedId(model2Reaction.getProduct(i3).getSpecies(), ModifiedIds)).getCompartment()))
                                    {
                                        isProductFound = true;
                                        if (!model1Reaction.getProduct(i4).getStoichiometry().Equals(model2Reaction.getProduct(i3).getStoichiometry()))
                                        {
                                            tmpDiffOfMergedReactionElements.Add(new KeyValuePair<string, AttributeTypeEnum>("Product " + model1Reaction.getProduct(i4).getSpecies(), AttributeTypeEnum.Stoichiometry));
                                        }
                                        break;
                                    }
                                }
                                if (!isProductFound)
                                {
                                    isReactionFound = false;
                                    break;
                                }
                            }

                            // If reactants or products of the reaction are not matching
                            if (!isReactionFound) { break; }

                            // Sync modifiers in the reactions which have the same name.
                            for (int i3 = 0; i3 < model2Reaction.getNumModifiers(); i3++)
                            {
                                bool isModifierFound = false;
                                for (int i4 = 0; i4 < model1Reaction.getNumModifiers(); i4++)
                                {
                                    if (model1Reaction.getModifier(i4).getSpecies().Equals(FindModifiedId(model2Reaction.getModifier(i3).getSpecies(), ModifiedIds)) &&
                                        model1.getSpecies(model1Reaction.getModifier(i4).getSpecies()).getCompartment().Equals(model2.getSpecies(FindModifiedId(model2Reaction.getModifier(i3).getSpecies(), ModifiedIds)).getCompartment()))
                                    {
                                        isModifierFound = true;
                                        break;
                                    }
                                }
                                if (!isModifierFound)
                                {
                                    isReactionFound = false;
                                    break;
                                }
                            }

                            // If reactants or products of the reaction are not matching
                            if (!isReactionFound) { break; }

                            // Reactions will be merged, therefore add elements with different attributes to display in the warning list.
                            tmpReact.AddRange(tmpDiffOfMergedReactionElements);
                        }
                    }
                    if (!isReactionFound)
                    {
                        var reactionToAdd = model2.getReaction(i2);
                        // if same id is used in model 1, then prefix the id with model2_
                        if (modelComposed.getReaction(reactionToAdd.getId()) != null)
                        {
                            var id = modelComposed.getReaction(reactionToAdd.getId()).getId();
                            var name = modelComposed.getReaction(reactionToAdd.getId()).getName();

                            reactionToAdd.setId(AddToModifiedIds(id, ModifiedIds));
                            reactionToAdd.setName("model2_" + name);

                            tmpReact.Add(new KeyValuePair<string, AttributeTypeEnum>(String.Format("Reaction \"{0}\" of {1} and {2} are not merged. In the composed model, {0} is the same of that of {1} while model2_{0} is the {0} of the {2}", name, model1.getName(), model2.getName()), AttributeTypeEnum.Rename));
                        }

                        for (int i = 0; i < reactionToAdd.getNumReactants(); i++)
                        {
                            var reactant = reactionToAdd.getReactant(i);
                            reactant.setSpecies(FindModifiedId(reactant.getSpecies(), ModifiedIds));
                        }
                        for (int i = 0; i < reactionToAdd.getNumProducts(); i++)
                        {
                            var product = reactionToAdd.getProduct(i);
                            product.setSpecies(FindModifiedId(product.getSpecies(), ModifiedIds));
                        }
                        for (int i = 0; i < reactionToAdd.getNumModifiers(); i++)
                        {
                            var modifier = reactionToAdd.getModifier(i);
                            modifier.setSpecies(FindModifiedId(modifier.getSpecies(), ModifiedIds));
                        }

                        modelComposed.addReaction(reactionToAdd);
                    }
                }
                DiffOfMergedElements = tmpReact;
                tmpReact = null;
                #endregion

                #region Event matching
                // Add events in model2 to composedModel.
                for (int i2 = 0; i2 < model2.getNumEvents(); i2++)
                {
                    var eventToAdd = model2.getEvent(i2);
                    // Id of the event should start with a letter and cannot contain "-" dash character.
                    var uniqueId = String.Format("model2_{0}", eventToAdd.getId());
                    var uniqueName = String.Format("model2_{0}", eventToAdd.getName());
                    eventToAdd.setId(uniqueId);
                    eventToAdd.setName(uniqueName);
                    modelComposed.addEvent(eventToAdd);
                }
                #endregion

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Add the id to ModifiedIds session with correct prefix.
        /// </summary>
        /// <param name="idForComposedModel">Original id which will be prefixed by the method.</param>
        /// <param name="ModifiedIds">List of modified ids.</param>
        /// <returns>Prefixed id.</returns>
        private static string AddToModifiedIds(string idForComposedModel, List<string> ModifiedIds)
        {
            var newId = "model2_" + idForComposedModel;
            var modifiedIds = ModifiedIds;
            modifiedIds.Add(newId);
            ModifiedIds = modifiedIds;
            return newId;
        }

        /// <summary>
        /// Checks the id(Sid) of the SBML element whether it is modified during composition.
        /// </summary>
        /// <param name="idToCheck">String Sid of the element.</param>
        /// <param name="ModifiedIds">List of modified ids.</param>
        /// <returns>If id found returns new id, otherwise returns idToCheck</returns>
        private static string FindModifiedId(string idToCheck, List<string> ModifiedIds)
        {
            var idFound = ModifiedIds.FirstOrDefault(x => x.Equals("model2_" + idToCheck));
            return idFound ?? idToCheck;
        }
    }
}
