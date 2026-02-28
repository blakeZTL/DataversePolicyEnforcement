using DataversePolicyEnforcement.Core.Model;

namespace DataversePolicyEnforcement.CustomApi.Models
{
    public class AttributeDecisionResult
    {
        public string EntityLogicalName { get; set; }

        public string AttributeLogicalName { get; set; }

        public string TriggerAttributeLogicalName { get; set; }

        public ClientPolicyDetails ClientPolicyDetails { get; set; }
    }
}
