using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;

namespace DataversePolicyEnforcement.CustomApi.Models
{
    public class AttributeDecisionInputParameters
    {
        public string EntityLogicalName { get; set; }
        public string TriggerAttributeLogicalName { get; set; }
        public object CurrentValue { get; set; }
        public List<string> TargetAttributeLogicalNames { get; set; }

        const string _EntityLogicalName = "dpe_gad_entitylogicalname";
        const string _TriggerAttributeLogicalName = "dpe_gad_triggerattributelogicalname";
        const string _CurrentValue = "dpe_gad_triggercurrentvalue";
        const string _TargetAttributeLogicalNames = "dpe_gad_targetattributelogicalnames";

        public AttributeDecisionInputParameters(ParameterCollection collection)
        {
            if (collection == null)
            {
                throw new System.ArgumentNullException("Collection is required and cannot be null");
            }

            if (!collection.Contains(_EntityLogicalName))
            {
                throw new System.ArgumentException(
                    $"Missing required parameter: {_EntityLogicalName}"
                );
            }

            if (
                !(collection[_EntityLogicalName] is string entityLogicalName)
                || string.IsNullOrEmpty(entityLogicalName.Trim())
            )
            {
                throw new System.ArgumentException("Entity logical name cannot be null or empty.");
            }

            EntityLogicalName = entityLogicalName;

            if (!collection.Contains(_TriggerAttributeLogicalName))
            {
                throw new System.ArgumentException(
                    $"Missing required parameter: {_TriggerAttributeLogicalName}"
                );
            }

            if (
                !(collection[_TriggerAttributeLogicalName] is string triggerAttributeLogicalName)
                || string.IsNullOrEmpty(triggerAttributeLogicalName.Trim())
            )
            {
                throw new System.ArgumentException(
                    "Trigger attribute logical name cannot be null or empty."
                );
            }

            TriggerAttributeLogicalName = triggerAttributeLogicalName;

            if (!collection.Contains(_CurrentValue))
            {
                throw new System.ArgumentException($"Missing required parameter: {_CurrentValue}");
            }
            object currentValue = collection[_CurrentValue];
            CurrentValue = currentValue;

            if (!collection.Contains(_TargetAttributeLogicalNames))
            {
                TargetAttributeLogicalNames = new List<string>();
            }
            else
            {
                object targetAttributeLogicalNames = collection[_TargetAttributeLogicalNames];
                TargetAttributeLogicalNames = NormalizeTargetAttributeLogicalNames(
                    targetAttributeLogicalNames
                );
            }
        }

        private List<string> NormalizeTargetAttributeLogicalNames(object input)
        {
            if (input == null)
                return new List<string>();

            if (input is List<string> list)
                return new List<string>(list);

            if (input is string[] sArr)
                return sArr.ToList();

            if (input is object[] oArr)
                return oArr.OfType<string>().ToList();

            if (input is IEnumerable<string> e)
                return e.ToList();

            if (input is string s)
                return new List<string> { s };

            throw new System.ArgumentException(
                $"Unexpected type for targetAttributeLogicalNames: {input.GetType().FullName}",
                nameof(input)
            );
        }
    }
}
