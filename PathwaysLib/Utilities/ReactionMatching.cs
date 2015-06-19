using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.Utilities
{
    public class ReactionMatching
    {
        public ReactionMatching(List<String> e, List<ServerModel> m)
        {
            Equations = e;
            Models = m;
        }
        public List<String> Equations { get; set; }
        public List<ServerModel> Models { get; set; }
    }

    public static class ReactionMatchingFunctions
    {
        public static void GetReactionMatchings(ServerModel[] models, List<ReactionMatching> reactionModelMatchings)
        {
            foreach (var model in models)
            {
                var reactions = model.GetAllReactions();
                var equations = new List<String>();

                foreach (var reaction in reactions)
                {
                    equations.Add(GetEquation(reaction));
                }

                ReactionMatching reactionModel = GetMatchingEquations(equations, reactionModelMatchings);

                if (reactionModel == null)
                {
                    // Add model with its reactions as a new group entry
                    var modelsList = new List<ServerModel> { model };
                    reactionModelMatchings.Add(new ReactionMatching(equations, modelsList));
                }
                else
                {
                    reactionModel.Models.Add(model);
                }
            }

        }

        private static ReactionMatching GetMatchingEquations(List<string> equation, List<ReactionMatching> reactionModelMatchings)
        {
            foreach (var reactionMatch in reactionModelMatchings)
            {
                var isMatchingFound = equation.All(y => reactionMatch.Equations.Contains(y));
                if (isMatchingFound)
                {
                    return reactionMatch;
                }
            }
            return null;
        }

        private static string GetEquation(ServerReaction reaction)
        {
            var species = reaction.GetAllSpeciesIdWithName();
            var products = species.Where(x => x.RoleId == (int)ReactionSpeciesRoleEnum.product).OrderBy(x => x.Name).ToList();
            var substrates = species.Where(x => x.RoleId == (int)ReactionSpeciesRoleEnum.substrate).OrderBy(x => x.Name).ToList();
            var modifiers = species.Where(x => x.RoleId == (int)ReactionSpeciesRoleEnum.modifier).OrderBy(x => x.Name).ToList();

            StringBuilder equation = new StringBuilder();
            substrates.ForEach(x => equation.Append((x.Stoichiometry == 1 ? "" : x.Stoichiometry.ToString()) + x.Name + "+"));
            if (equation.Length > 0)
            {
                if (equation[equation.Length - 1] == '+') { equation.Remove(equation.Length - 1, 1); /*delete the last + sign*/}
            }
            if (reaction.Reversible)
            {
                equation.Append("<->");
            }
            else
            {
                equation.Append("->");
            }

            products.ForEach(x => equation.Append((x.Stoichiometry == 1 ? "" : x.Stoichiometry.ToString()) + x.Name + "+"));
            if (equation.Length > 0)
            {
                if (equation[equation.Length - 1] == '+') { equation.Remove(equation.Length - 1, 1); /*delete the last + sign*/}
            }
            return equation.ToString();
        }

    }
}
