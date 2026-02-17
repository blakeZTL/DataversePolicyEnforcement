using System.Collections.Generic;

namespace DataversePolicyEnforcement.Core.Data
{
    public interface IMetadataValidator
    {
        bool ValidateEntity(string logicalName);

        bool ValidateAttribute(string entityLogicalName, string attributeLogicalName);

        bool ValidateAllAttributes(string entityLogicalName, List<string> attributeLogicalNames);
    }
}
