using DataversePolicyEnforcement.Core.Model;
using Microsoft.Xrm.Sdk;

namespace DataversePolicyEnforcement.Core.Evaluation
{
    public interface IPolicyEvaluator
    {
        PolicyDecision EvaluateAttribute(
            string entityLogicalName,
            string attributeLogicalName,
            Entity target, // values being written (may or may not contain attribute)
            Entity preImage // may be null; or may not contain attribute
        );
    }
}
