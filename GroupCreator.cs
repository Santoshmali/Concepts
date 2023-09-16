using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treetraverse
{
    public class GroupCreator
    {
        public List<ConditionGroup> Evaluate(ConditionGroup conditionGroup)
        {
            List<ConditionGroup> conditionGroups = new List<ConditionGroup>();

            // Group can either have conditions or condion groups


            // If there is OR operator then its separate condition else condition group will be merged together
            if (conditionGroup.Operator == "OR")
            {
                // All conditions will be in separate condition group
                // Condition groups
                if (conditionGroup.ConditionGroups != null && conditionGroup.ConditionGroups.Any())
                {
                    foreach (var cGroup in conditionGroup.ConditionGroups)
                    {
                        var result = Evaluate(cGroup);
                        conditionGroups.AddRange(result);
                    }
                }

                // Conditions
                if (conditionGroup.Conditions != null && conditionGroup.Conditions.Any())
                {
                    foreach(var condition in conditionGroup.Conditions)
                    {
                        var newConditionGroup = new ConditionGroup()
                        {
                            Conditions = new List<Condition>()
                        };
                        newConditionGroup.Conditions.Add(condition);
                        conditionGroups.Add(newConditionGroup);
                    }
                }
            }
            else if (conditionGroup.Operator == "AND")
            {
                var newConditionGroups = new List<ConditionGroup>();
                // All conditions will be merged into single group
                // Condition groups
                if (conditionGroup.ConditionGroups != null && conditionGroup.ConditionGroups.Any())
                {
                    foreach (var cGroup in conditionGroup.ConditionGroups)
                    {
                        var result = Evaluate(cGroup);
                        newConditionGroups.AddRange(result);
                    }                    
                }

                var newConditionGroup = new ConditionGroup()
                {
                    Conditions = new List<Condition>()
                };
                // Conditions
                if (conditionGroup.Conditions != null && conditionGroup.Conditions.Any())
                {
                    foreach (var condition in conditionGroup.Conditions)
                    {
                        newConditionGroup.Conditions.Add(condition);
                    }

                    newConditionGroups.Add(newConditionGroup);
                }

                // Merge all condition groups as its AND
                // Permutation 
                var groupedList = newConditionGroups.GroupBy(t => t.ConditionGroupId).Combine();
                foreach(var groups in groupedList)
                {
                    var groupedConditionGroup = new ConditionGroup()
                    {
                        ConditionGroupId = Guid.NewGuid().ToString(),
                        Conditions = new List<Condition>()
                    };
                    foreach(var group in groups)
                    {
                        // Add all conditions in same condition group
                        groupedConditionGroup.Conditions.AddRange(group.Conditions);
                    }

                    conditionGroups.Add(groupedConditionGroup);
                }
            }

            return conditionGroups;
        }
    }

    public static class Combination
    {
        public static IEnumerable<IEnumerable<t>> Combine<t>(this IEnumerable<IEnumerable<t>> sequences)
        {
            IEnumerable<IEnumerable<t>> emptyproduct = new[] { Enumerable.Empty<t>() };
            return sequences.Aggregate(
              emptyproduct,
              (accumulator, sequence) =>
                from accseq in accumulator
                from item in sequence
                select accseq.Concat(new[] { item }));
        }
    }

    public class ConditionGroup
    {
        public string ConditionGroupId { get; set; }
        public List<ConditionGroup> ConditionGroups { get; set; }
        public string Operator { get; set; }
        public List<Condition> Conditions { get; set; }
    }

    public class Condition
    {
        public List<ConditionGroup> ConditionGroups { get; set; }
        public List<string> Names { get; set; }
        public string Operator { get; set; }
    }


}
