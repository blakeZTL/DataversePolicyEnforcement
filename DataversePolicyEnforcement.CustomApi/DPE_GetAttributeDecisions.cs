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

                var entity = new Entity(req.dpe_gad_entitylogicalname)
                {
                    [req.dpe_gad_triggerattributelogicalname] =
                        req.dpe_gad_triggercurrentvalue_string
                };

                var evaluator = new PolicyEvaluator(systemService, new PolicyCollection());

                var results = new List<AttributeDecisionResult>();

                foreach (var targetAttribute in req.dpe_gad_targetattributelogicalnames)
                {
                    var decision = evaluator.EvaluateAttribute(
                        req.dpe_gad_entitylogicalname,
                        targetAttribute,
                        entity,
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
    }
}
