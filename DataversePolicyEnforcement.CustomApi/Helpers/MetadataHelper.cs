using DataversePolicyEnforcement.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;

namespace DataversePolicyEnforcement.CustomApi.Helpers
{
    public class MetadataHelper
    {
        private readonly dpe_GetAttributeDecisionsRequest _request;
        private readonly IOrganizationService _service;
        private readonly ITracingService _tracer;

        public MetadataHelper(
            dpe_GetAttributeDecisionsRequest req,
            IOrganizationService service,
            ITracingService tracer
        )
        {
            _request = req;
            _service = service;
            _tracer = tracer;
        }

        public AttributeMetadata GetAttributeMetadata()
        {
            var request = new RetrieveAttributeRequest
            {
                EntityLogicalName = _request.dpe_gad_entitylogicalname,
                LogicalName = _request.dpe_gad_triggerattributelogicalname
            };
            var response = (RetrieveAttributeResponse)_service.Execute(request);
            return response.AttributeMetadata;
        }

        public object ParseAttributeValue()
        {
            object value = null;
            if (string.IsNullOrWhiteSpace(_request.dpe_gad_trigger_currentvalue))
            {
                _tracer.Trace(
                    "Trigger current value is null or empty. Cannot parse attribute value."
                );
                return value;
            }

            var attributeMetadata = GetAttributeMetadata();
            _tracer.Trace(
                $"Parsing attribute value for attribute type: {attributeMetadata.AttributeType}"
            );
            switch (attributeMetadata.AttributeType)
            {
                case AttributeTypeCode.Lookup:
                    value = ParseLookupValue();
                    break;
                case AttributeTypeCode.String:
                    value = _request.dpe_gad_trigger_currentvalue;
                    break;
                case AttributeTypeCode.Integer:
                    value = ParseIntValue();
                    break;
                case AttributeTypeCode.Picklist:
                    value = ParseOptionValue();
                    break;
                default:
                    _tracer.Trace(
                        $"Attribute type {attributeMetadata.AttributeType} not supported for parsing."
                    );
                    throw new NotSupportedException(
                        $"Parsing for attribute type {attributeMetadata.AttributeType} is not supported."
                    );
            }

            return value;
        }

        private EntityReference ParseLookupValue()
        {
            EntityReference response = null;

            if (string.IsNullOrWhiteSpace(_request.dpe_gad_triggercurrentvalue_lookup_logicalname))
            {
                _tracer.Trace("Lookup logical name is null or empty. Cannot parse lookup value.");
                return response;
            }

            try
            {
                response = new EntityReference(
                    _request.dpe_gad_triggercurrentvalue_lookup_logicalname,
                    Guid.Parse(_request.dpe_gad_trigger_currentvalue)
                );
            }
            catch (FormatException ex)
            {
                _tracer.Trace($"Failed to parse lookup value: {ex.Message}");
            }

            return response;
        }

        private int? ParseIntValue()
        {
            if (!int.TryParse(_request.dpe_gad_trigger_currentvalue, out int response))
            {
                _tracer.Trace(
                    $"Failed to parse integer value from trigger current value: {_request.dpe_gad_trigger_currentvalue}"
                );
                throw new FormatException(
                    $"Trigger current value '{_request.dpe_gad_trigger_currentvalue}' is not a valid integer."
                );
            }

            return response;
        }

        private int ParseOptionValue()
        {
            if (!int.TryParse(_request.dpe_gad_trigger_currentvalue, out int optionValue))
            {
                _tracer.Trace(
                    $"Failed to parse option set value from trigger current value: {_request.dpe_gad_trigger_currentvalue}"
                );
                throw new FormatException(
                    $"Trigger current value '{_request.dpe_gad_trigger_currentvalue}' is not a valid integer for option set."
                );
            }
            _tracer.Trace($"Parsed option set value: {optionValue}");
            return optionValue;
        }
    }
}
