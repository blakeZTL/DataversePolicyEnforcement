using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;

namespace DataversePolicyEnforcement.Core.Data
{
    public class MetadataValidator : IMetadataValidator
    {
        private readonly IOrganizationService _service;

        public MetadataValidator(IOrganizationService service)
        {
            _service = service;
        }

        public bool ValidateAttribute(string entityLogicalName, string attributeLogicalName)
        {
            if (
                _service == null
                || string.IsNullOrWhiteSpace(entityLogicalName)
                || string.IsNullOrWhiteSpace(attributeLogicalName)
            )
                return false;

            var attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityLogicalName,
                LogicalName = attributeLogicalName
            };

            try
            {
                var response = (RetrieveAttributeResponse)_service.Execute(attributeRequest);
                return response.AttributeMetadata != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ValidateAllAttributes(
            string entityLogicalName,
            List<string> attributeLogicalNames
        )
        {
            if (_service == null || string.IsNullOrWhiteSpace(entityLogicalName))
                return false;

            if (attributeLogicalNames == null || attributeLogicalNames.Count == 0)
                return true;

            var metadataRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = entityLogicalName
            };

            try
            {
                var response = (RetrieveEntityResponse)_service.Execute(metadataRequest);
                var entityMetadata = response.EntityMetadata;
                if (entityMetadata == null || entityMetadata.Attributes == null)
                    return false;
                var existingAttributeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var attr in entityMetadata.Attributes)
                {
                    existingAttributeNames.Add(attr.LogicalName);
                }
                foreach (var attributeName in attributeLogicalNames)
                {
                    if (!existingAttributeNames.Contains(attributeName))
                        return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ValidateEntity(string logicalName)
        {
            if (_service == null || string.IsNullOrWhiteSpace(logicalName))
                return false;

            var metadataRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Entity,
                LogicalName = logicalName
            };

            try
            {
                var response = (RetrieveEntityResponse)_service.Execute(metadataRequest);
                return response.EntityMetadata != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
