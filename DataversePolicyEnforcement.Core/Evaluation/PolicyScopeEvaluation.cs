using DataversePolicyEnforcement.Core.Data;
using DataversePolicyEnforcement.Core.Model;
using DataversePolicyEnforcement.Models.Entities;
using DataversePolicyEnforcement.Models.OptionSets;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataversePolicyEnforcement.Core.Evaluation
{
    public class PolicyScopeEvaluation : IPolicyScopeEvaluator
    {
        private readonly IPolicyCollection _policyCollection;

        public PolicyScopeEvaluation(IPolicyCollection policyCollection)
        {
            _policyCollection =
                policyCollection ?? throw new ArgumentNullException(nameof(policyCollection));
        }

        public ClientPolicyDetails EvaluateClientScope(
            IOrganizationService service,
            List<dpe_PolicyRule> rules,
            Entity target,
            Entity preImage,
            ClientPolicyDetails initial
        )
        {
            if (initial == null)
                initial = new ClientPolicyDetails();
            if (service == null || rules == null || rules.Count == 0 || target == null)
                return initial;

            var visibleSet = false;
            var requiredSet = false;
            var notAllowedSet = false;

            foreach (var rule in rules.OrderBy(r => r.dpe_Sequence))
            {
                if (visibleSet && requiredSet && notAllowedSet)
                    break;

                var conditions = _policyCollection.GetConditions(service, rule.Id);
                if (!Comparison.Condition.AllConditionsMatch(rule, conditions, target, preImage))
                    continue;

                switch (rule.dpe_PolicyType)
                {
                    case dpe_policytype.Visible:
                        if (!visibleSet)
                        {
                            initial.Visible = rule.dpe_Result ?? initial.Visible;
                            visibleSet = true;
                        }
                        break;

                    case dpe_policytype.Required:
                        if (!requiredSet)
                        {
                            initial.Required = rule.dpe_Result ?? initial.Required;
                            requiredSet = true;
                        }
                        break;

                    case dpe_policytype.NotAllowed:
                        var notAllowedResult = rule.dpe_Result ?? initial.NotAllowed;
                        if (notAllowedResult)
                        {
                            initial.NotAllowed = true;
                            notAllowedSet = true;
                            return initial;
                        }
                        else
                        {
                            initial.NotAllowed = false;
                        }
                        break;
                }
            }

            return initial;
        }

        public ServerPolicyDetails EvaluateServerScope(
            IOrganizationService service,
            List<dpe_PolicyRule> rules,
            Entity target,
            Entity preImage,
            ServerPolicyDetails initial
        )
        {
            if (initial == null)
                initial = new ServerPolicyDetails();
            if (service == null || rules == null || rules.Count == 0 || target == null)
                return initial;

            var requiredSet = false;
            var notAllowedSet = false;

            foreach (var rule in rules.OrderBy(r => r.dpe_Sequence))
            {
                if (requiredSet && notAllowedSet)
                    break;

                var conditions = _policyCollection.GetConditions(service, rule.Id);
                if (!Comparison.Condition.AllConditionsMatch(rule, conditions, target, preImage))
                    continue;

                switch (rule.dpe_PolicyType)
                {
                    case dpe_policytype.Visible:
                        // Server scope ignores Visible rules
                        break;

                    case dpe_policytype.Required:
                        if (!requiredSet)
                        {
                            initial.Required = rule.dpe_Result ?? initial.Required;
                            requiredSet = true;
                        }
                        break;

                    case dpe_policytype.NotAllowed:
                        var notAllowedResult = rule.dpe_Result ?? initial.NotAllowed;
                        if (notAllowedResult)
                        {
                            initial.NotAllowed = true;
                            notAllowedSet = true;
                            return initial;
                        }
                        else
                        {
                            initial.NotAllowed = false;
                        }
                        break;
                }
            }

            return initial;
        }
    }
}
