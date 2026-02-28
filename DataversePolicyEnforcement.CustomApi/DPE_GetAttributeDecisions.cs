using DataversePolicyEnforcement.Core.Data;
using DataversePolicyEnforcement.Core.Evaluation;
using DataversePolicyEnforcement.CustomApi.Helpers;
using DataversePolicyEnforcement.CustomApi.Models;
using DataversePolicyEnforcement.Models;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DataversePolicyEnforcement.CustomApi
{
    public class DPE_GetAttributeDecisions : PluginBase
    {
        const string OutputParameterName = "dpe_gad_attributedecision_results";

        public DPE_GetAttributeDecisions()
            : base(typeof(DPE_GetAttributeDecisions))
        {
            // not implemented
        }

        protected override void ExecuteCdsPlugin(ILocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var systemService = localPluginContext.SystemUserService;
            var tracer = localPluginContext.TracingService;

            try
            {
                var req = new dpe_GetAttributeDecisionsRequest();

                foreach (var kvp in context.InputParameters)
                {
                    req[kvp.Key] = kvp.Value;
                }

                if (
                    !RequestIsValid(req, systemService, tracer, out Entity validEntity)
                    || validEntity == null
                )
                {
                    throw new InvalidPluginExecutionException(
                        "Invalid request: exactly one trigger current value must be provided."
                    );
                }

                var evaluator = new PolicyEvaluator(systemService, new PolicyCollection());

                var results = new List<AttributeDecisionResult>();

                foreach (var targetAttribute in req.dpe_gad_targetattributelogicalnames)
                {
                    var decision = evaluator.EvaluateAttribute(
                        req.dpe_gad_entitylogicalname,
                        targetAttribute,
                        validEntity,
                        null
                    );
                    results.Add(
                        new AttributeDecisionResult
                        {
                            EntityLogicalName = req.dpe_gad_entitylogicalname,
                            AttributeLogicalName = targetAttribute,
                            TriggerAttributeLogicalName = req.dpe_gad_triggerattributelogicalname,
                            ClientPolicyDetails = decision.ClientDetails,
                        }
                    );
                }

                string json = JsonSerializer.Serialize(results);
                tracer.Trace("Returning {0} attribute decision(s) as JSON", results.Count);
                tracer.Trace(json);

                context.OutputParameters[OutputParameterName] = json;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message, ex);
            }
        }

        private bool RequestIsValid(
            dpe_GetAttributeDecisionsRequest req,
            IOrganizationService service,
            ITracingService tracer,
            out Entity validEntity
        )
        {
            validEntity = null;
            if (req == null)
                return false;

            var metadataHelper = new MetadataHelper(req, service, tracer);
            object value;
            try
            {
                value = metadataHelper.ParseAttributeValue();
            }
            catch (Exception ex)
            {
                tracer.Trace($"Request invalid: {ex.Message}", ex);
                return false;
            }
            validEntity = new Entity(req.dpe_gad_entitylogicalname);

            tracer.Trace(
                $"Request valid. Setting {req.dpe_gad_triggerattributelogicalname} to {value}"
            );
            validEntity[req.dpe_gad_triggerattributelogicalname] = value;

            return true;
        }
    }
}
