using DataversePolicyEnforcement.Models.Entities;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace DataversePolicyEnforcement.Core.Evaluation
{
    public interface IPolicyScopeEvaluator
    {
        ClientPolicyDetails EvaluateClientScope(
            IOrganizationService service,
            List<dpe_PolicyRule> rules,
            Entity target,
            Entity preImage,
            ClientPolicyDetails initial
        );

        ServerPolicyDetails EvaluateServerScope(
            IOrganizationService service,
            List<dpe_PolicyRule> rules,
            Entity target,
            Entity preImage,
            ServerPolicyDetails initial
        );
    }
}
