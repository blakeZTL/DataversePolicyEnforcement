using DataversePolicyEnforcement.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace DataversePolicyEnforcement.Core.Data
{
    public class PolicyCollection : IPolicyCollection
    {
        public List<dpe_PolicyCondition> GetConditions(IOrganizationService service, Guid ruleId)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (ruleId == Guid.Empty)
                throw new ArgumentNullException(nameof(ruleId));

            var query = new QueryExpression(dpe_PolicyCondition.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(
                            dpe_PolicyCondition.Fields.dpe_PolicyRuleId,
                            ConditionOperator.Equal,
                            ruleId
                        ),
                        new ConditionExpression(
                            dpe_PolicyCondition.Fields.statecode,
                            ConditionOperator.Equal,
                            (int)dpe_policycondition_statecode.Active
                        )
                    }
                },
                Orders =
                {
                    new OrderExpression(
                        dpe_PolicyCondition.Fields.dpe_Sequence,
                        OrderType.Ascending
                    )
                }
            };

            var results = service.RetrieveMultiple(query);

            var conditions = new List<dpe_PolicyCondition>();

            foreach (var entity in results.Entities)
            {
                conditions.Add(entity.ToEntity<dpe_PolicyCondition>());
            }
            return conditions;
        }

        public List<dpe_PolicyRule> GetRules(
            IOrganizationService service,
            string entityLogicalName,
            string attributeLogicalName
        )
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (entityLogicalName == null)
                throw new ArgumentNullException(nameof(entityLogicalName));
            if (attributeLogicalName == null)
                throw new ArgumentNullException(nameof(attributeLogicalName));

            var query = new QueryExpression(dpe_PolicyRule.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(
                            dpe_PolicyRule.Fields.dpe_TargetEntityLogicalName,
                            ConditionOperator.Equal,
                            entityLogicalName
                        ),
                        new ConditionExpression(
                            dpe_PolicyRule.Fields.dpe_TargetAttributeLogicalName,
                            ConditionOperator.Equal,
                            attributeLogicalName
                        ),
                        new ConditionExpression(
                            dpe_PolicyRule.Fields.statecode,
                            ConditionOperator.Equal,
                            (int)dpe_policyrule_statecode.Active
                        )
                    }
                },
                Orders =
                {
                    new OrderExpression(dpe_PolicyRule.Fields.dpe_Sequence, OrderType.Ascending)
                }
            };

            var results = service.RetrieveMultiple(query);

            var rules = new List<dpe_PolicyRule>();

            foreach (var entity in results.Entities)
            {
                rules.Add(entity.ToEntity<dpe_PolicyRule>());
            }
            return rules;
        }

        public HashSet<string> GetGovernedAttributes(
            IOrganizationService service,
            string entityLogicalName
        )
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            var governedAttributes = new HashSet<string>();

            if (entityLogicalName == null || string.IsNullOrWhiteSpace(entityLogicalName))
                return governedAttributes;

            var query = new QueryExpression(dpe_PolicyRule.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(dpe_PolicyRule.Fields.dpe_TargetAttributeLogicalName),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(
                            dpe_PolicyRule.Fields.dpe_TargetEntityLogicalName,
                            ConditionOperator.Equal,
                            entityLogicalName
                        ),
                        new ConditionExpression(
                            dpe_PolicyRule.Fields.statecode,
                            ConditionOperator.Equal,
                            (int)dpe_policyrule_statecode.Active
                        )
                    }
                },
                Distinct = true
            };
            var results = service.RetrieveMultiple(query);

            foreach (var entity in results.Entities)
            {
                var rule = entity.ToEntity<dpe_PolicyRule>();
                if (!string.IsNullOrEmpty(rule.dpe_TargetAttributeLogicalName))
                    governedAttributes.Add(rule.dpe_TargetAttributeLogicalName);
            }
            return governedAttributes;
        }
    }
}
