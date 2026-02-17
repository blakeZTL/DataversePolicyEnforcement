using DataversePolicyEnforcement.Core.Data;
using DataversePolicyEnforcement.Core.Model;
using DataversePolicyEnforcement.Models;
using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Linq;

namespace DataversePolicyEnforcement.Core.Evaluation
{
    public class PolicyEvaluator : IPolicyEvaluator
    {
        private readonly IPolicyCollection _policyCollection;
        private readonly IOrganizationService _service;

        public PolicyEvaluator(IOrganizationService service, IPolicyCollection policyCollection)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _policyCollection =
                policyCollection ?? throw new ArgumentNullException(nameof(policyCollection));
        }

        public PolicyDecision EvaluateAttribute(
            string entityLogicalName,
            string attributeLogicalName,
            Entity target,
            Entity preImage
        )
        {
            var decision = new PolicyDecision();

            if (target == null)
                return decision;

            var rules = _policyCollection.GetRules(
                _service,
                entityLogicalName,
                attributeLogicalName
            );
            Debug.WriteLine(
                $"Evaluating {rules.Count} rules for {entityLogicalName}.{attributeLogicalName}"
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
            Debug.WriteLine($"Server rules count: {serverRules.Count}");
            var clientRules = rules
                .Where(
                    r =>
                        r.dpe_Scope == dpe_policyscope.FormOnly
                        || r.dpe_Scope == dpe_policyscope.Both
                )
                .OrderBy(r => r.dpe_Sequence)
                .ToList();
            Debug.WriteLine($"Client rules count: {clientRules.Count}");
            var scopeEvaluator = new PolicyScopeEvaluation(_policyCollection);

            decision.ServerDetails = scopeEvaluator.EvaluateServerScope(
                _service,
                serverRules,
                target,
                preImage,
                decision.ServerDetails
            );
            decision.ClientDetails = scopeEvaluator.EvaluateClientScope(
                _service,
                clientRules,
                target,
                preImage,
                decision.ClientDetails
            );
            Debug.WriteLine(
                $"Final decision for {entityLogicalName}.{attributeLogicalName}"
                    + Environment.NewLine
                    + "Server:"
                    + $" Required={decision.ServerDetails.Required}, NotAllowed={decision.ServerDetails.NotAllowed};"
                    + Environment.NewLine
                    + "Client:"
                    + $" Required={decision.ClientDetails.Required}, Visible={decision.ClientDetails.Visible}, NotAllowed={decision.ClientDetails.NotAllowed}"
            );
            return decision;
        }
    }
}
