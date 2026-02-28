using DataversePolicyEnforcement.Models;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataversePolicyEnforcement.Core.Comparison
{
    public static class Condition
    {
        public static bool ConditionValueEquals(dpe_PolicyCondition condition, object value)
        {
            if (condition == null || value == null)
                return false;
            Debug.WriteLine(
                $"{nameof(ConditionValueEquals)}: Condition Value Type of {condition.dpe_ValueType.Value} comparing against value of {value}"
            );
            try
            {
                switch (condition.dpe_ValueType)
                {
                    case dpe_policyconditionvaluetype.String:
                        return string.Equals(
                            condition.dpe_ValueString,
                            value.ToString(),
                            StringComparison.OrdinalIgnoreCase
                        );
                    case dpe_policyconditionvaluetype.WholeNumber:
                        return condition.dpe_ValueWholeNumber == Convert.ToInt32(value);
                    case dpe_policyconditionvaluetype.Decimal:
                        return condition.dpe_ValueDecimal == Convert.ToDecimal(value);
                    case dpe_policyconditionvaluetype.Boolean:
                        return condition.dpe_ValueBoolean == Convert.ToBoolean(value);
                    case dpe_policyconditionvaluetype.DateTime:
                        return condition.dpe_ValueDateTime == Convert.ToDateTime(value);
                    case dpe_policyconditionvaluetype.Guid:
                        return string.Equals(
                            condition.dpe_ValueGuid,
                            value.ToString(),
                            StringComparison.OrdinalIgnoreCase
                        );
                    case dpe_policyconditionvaluetype.Money:
                        return condition.dpe_ValueMoney == (Money)value;
                    case dpe_policyconditionvaluetype.OptionSet:
                        return condition.dpe_ValueOptionSetValue == Convert.ToInt32(value);
                    case dpe_policyconditionvaluetype.Lookup:
                        return string.Equals(
                                condition.dpe_ValueLookupLogicalName,
                                ((EntityReference)value).LogicalName,
                                StringComparison.OrdinalIgnoreCase
                            )
                            && condition.dpe_ValueLookupId
                                == ((EntityReference)value).Id.ToString();
                    default:
                        return false;
                }
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public static bool ConditionMatches(
            dpe_PolicyRule rule,
            dpe_PolicyCondition condition,
            Entity target,
            Entity preImage
        )
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (
                !target.TryGetAttributeValue(
                    rule.dpe_TriggerAttributeLogicalName,
                    out object targetValue
                )
            )
            {
                if (
                    preImage == null
                    || !preImage.TryGetAttributeValue(
                        rule.dpe_TriggerAttributeLogicalName,
                        out targetValue
                    )
                )
                {
                    return false;
                }
            }

            switch (condition.dpe_Operator)
            {
                case dpe_policyconditionoperator.Equals:
                    return ConditionValueEquals(condition, targetValue);
                case dpe_policyconditionoperator.NotEquals:
                    return !ConditionValueEquals(condition, targetValue);
                case dpe_policyconditionoperator.IsNull:
                    return targetValue == null;
                case dpe_policyconditionoperator.IsNotNull:
                    return targetValue != null;
                default:
                    return false;
            }
        }

        public static bool AllConditionsMatch(
            dpe_PolicyRule rule,
            List<dpe_PolicyCondition> conditions,
            Entity target,
            Entity preImage
        )
        {
            if (
                rule == null
                || conditions == null
                || conditions.Count == 0
                || target == null
                || rule.dpe_TriggerAttributeLogicalName == null
                || string.IsNullOrEmpty(rule.dpe_TriggerAttributeLogicalName)
            )
                return true;

            foreach (var condition in conditions.OrderBy(r => r.dpe_Sequence))
            {
                if (!ConditionMatches(rule, condition, target, preImage))
                    return false;
            }

            return true;
        }
    }
}
