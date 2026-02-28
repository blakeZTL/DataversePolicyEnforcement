using DataversePolicyEnforcement.Models;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace DataversePolicyEnforcement.Core.Data
{
    public interface IPolicyCollection
    {
        List<dpe_PolicyRule> GetRules(
            IOrganizationService service,
            string entityLogicalName,
            string attributeLogicalName
        );
        List<dpe_PolicyCondition> GetConditions(IOrganizationService service, Guid ruleId);
    }
}
