using DataversePolicyEnforcement.Core.Data;
using DataversePolicyEnforcement.Core.Model;
using DataversePolicyEnforcement.Models.OptionSets;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace DataversePolicyEnforcement.Core.Evaluation
{
    public class PolicyEvaluator : IPolicyEvaluator
    {
        private readonly IPolicyCollection _policyCollection;

        public PolicyEvaluator(IPolicyCollection policyCollection)
        {
            _policyCollection =
                policyCollection ?? throw new ArgumentNullException(nameof(policyCollection));
        }

        public PolicyDecision EvaluateAttribute(
            IOrganizationService service,
            string entityLogicalName,
            string attributeLogicalName,
            Entity target,
            Entity preImage
        )
        {
            var decision = new PolicyDecision();

            if (service == null || target == null)
                return decision;

            var rules = _policyCollection.GetRules(
                service,
                entityLogicalName,
                attributeLogicalName
            );

            if (rules == null || rules.Count == 0)
                return decision;

            var serverRules = rules
                .Where(
                    r =>
                        r.dpe_Scope == dpe_policyscope.ServerOnly
                        || r.dpe_Scope == dpe_policyscope.Both
                )
                .OrderBy(r => r.dpe_Sequence)
                .ToList();
            var clientRules = rules
                .Where(
                    r =>
                        r.dpe_Scope == dpe_policyscope.FormOnly
                        || r.dpe_Scope == dpe_policyscope.Both
                )
                .OrderBy(r => r.dpe_Sequence)
                .ToList();

            var scopeEvaluator = new PolicyScopeEvaluation(_policyCollection);

            decision.ServerDetails = scopeEvaluator.EvaluateServerScope(
                service,
                serverRules,
                target,
                preImage,
                decision.ServerDetails
            );
            decision.ClientDetails = scopeEvaluator.EvaluateClientScope(
                service,
                clientRules,
                target,
                preImage,
                decision.ClientDetails
            );

            return decision;
        }
    }
}
