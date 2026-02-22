using DataversePolicyEnforcement.Core.Data;
using DataversePolicyEnforcement.Core.Evaluation;
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
                    req.Parameters[kvp.Key] = kvp.Value;

                if (!RequestIsValid(req, tracer, out Entity validEntity) || validEntity == null)
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
            dpe_GetAttributeDecisionsRequest request,
            ITracingService tracer,
            out Entity validEntity
        )
        {
            validEntity = null;
            if (request == null)
                return false;

            double providedCount = 0;
            validEntity = new Entity(request.dpe_gad_entitylogicalname);
            EntityReference lookupValue = null;

            foreach (var kvp in request.Parameters)
            {
                if (kvp.Key.StartsWith("dpe_gad_triggercurrentvalue_lookup") && kvp.Value != null)
                {
                    providedCount += .5;
                    if (lookupValue == null)
                    {
                        lookupValue = new EntityReference();
                    }
                }
                else if (kvp.Key.StartsWith("dpe_gad_triggercurrentvalue") && kvp.Value != null)
                {
                    providedCount++;
                    validEntity[request.dpe_gad_triggerattributelogicalname] = kvp.Value;
                }
                else
                    continue;

                tracer.Trace("Found trigger current value: {0} = {1}", kvp.Key, kvp.Value);
            }

            if (lookupValue != null)
            {
                lookupValue.LogicalName = request.dpe_gad_triggercurrentvalue_lookup_logicalname;
                lookupValue.Id = Guid.Parse(request.dpe_gad_triggercurrentvalue_lookupid);
                validEntity[request.dpe_gad_triggerattributelogicalname] = lookupValue;
                tracer.Trace(
                    "Constructed lookup value for trigger current value: {0} ({1})",
                    lookupValue.Id,
                    lookupValue.LogicalName
                );
            }

            var isValid = providedCount == 1;

            if (!isValid)
            {
                validEntity = null;
            }

            return isValid;
        }
    }
}
