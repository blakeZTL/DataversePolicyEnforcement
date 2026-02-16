using DataversePolicyEnforcement.Core.Data;
using DataversePolicyEnforcement.Core.Evaluation;
using DataversePolicyEnforcement.Plugins.Helpers;
using Microsoft.Xrm.Sdk;
using System;

namespace DataversePolicyEnforcement.Plugins
{
    public class PolicyEnforcementPlugin : PluginBase
    {
        private const string PreImageName = "PreImage";

        public PolicyEnforcementPlugin()
            : base(typeof(PolicyEnforcementPlugin))
        {
            // Not Implemented
        }

        protected override void ExecuteCdsPlugin(ILocalPluginContext localPluginContext)
        {
            #region Plugin Execution Context
            var context = localPluginContext.PluginExecutionContext;
            var systemService = localPluginContext.SystemUserService;
            var tracer = localPluginContext.TracingService;

            if (context.MessageName != "Create" && context.MessageName != "Update")
            {
                tracer.Trace($"Message {context.MessageName} is not supported.");
                return;
            }

            if (context.Stage != 20) // PreOperation
            {
                tracer.Trace($"Stage {context.Stage} is not supported.");
                return;
            }
            #endregion

            #region Plugin Resources

            if (!context.InputParameters.TryGetValue("Target", out Entity target))
            {
                throw new InvalidPluginExecutionException(
                    "Target entity is missing in the input parameters."
                );
            }

            Entity preImage = null;
            if (
                context.MessageName == "Update"
                && !context.PreEntityImages.TryGetValue(PreImageName, out preImage)
                && preImage == null
            )
            {
                throw new InvalidPluginExecutionException(
                    $"Pre-Image named {PreImageName} required on update"
                );
            }
            tracer.Trace("Plugin Resources validated");
            #endregion

            try
            {
                var policyCollection = new PolicyCollection();

                var governedAttributes = policyCollection.GetGovernedAttributes(
                    systemService,
                    target.LogicalName
                );

                if (governedAttributes.Count == 0)
                {
                    tracer.Trace("No attributes to govern. Exiting.");
                    return;
                }
                tracer.Trace($"Governed attributes: {governedAttributes.Count}");

                var evaluator = new PolicyEvaluator(policyCollection);

                foreach (var attr in governedAttributes)
                {
                    tracer.Trace($"Evaluating {attr}");
                    var isCreate = context.MessageName == "Create";
                    var isUpdate = context.MessageName == "Update";

                    var inTarget = target.Attributes.ContainsKey(attr);

                    #region Update
                    if (isUpdate && inTarget)
                    {
                        tracer.Trace("Evaluating in context of an update message");
                        var decision = evaluator.EvaluateAttribute(
                            systemService,
                            target.LogicalName,
                            attr,
                            target,
                            preImage
                        );

                        if (decision.ServerDetails.NotAllowed)
                        {
                            tracer.Trace("Server: Not allowed");
                            var newValue = target[attr];
                            var oldValue = preImage.Attributes.ContainsKey(attr)
                                ? preImage[attr]
                                : null;

                            if (!ValueEquality.AreEqual(newValue, oldValue))
                            {
                                throw new InvalidPluginExecutionException(
                                    $"Change blocked by policy: {target.LogicalName}.{attr} is not allowed to change."
                                );
                            }
                        }

                        if (decision.ServerDetails.Required)
                        {
                            tracer.Trace("Server: required");
                            var newValue =
                                target[attr]
                                ?? throw new InvalidPluginExecutionException(
                                    $"Policy violation: {target.LogicalName}.{attr} is required."
                                );
                        }
                    }
                    #endregion


                    #region Create
                    if (isCreate)
                    {
                        tracer.Trace("Evaluating in context of a create message");
                        var decision = evaluator.EvaluateAttribute(
                            systemService,
                            target.LogicalName,
                            attr,
                            target,
                            null
                        );

                        if (decision.ServerDetails.Required)
                        {
                            if (!target.Attributes.ContainsKey(attr) || target[attr] == null)
                            {
                                throw new InvalidPluginExecutionException(
                                    $"Policy violation: {target.LogicalName}.{attr} is required."
                                );
                            }
                        }

                        if (
                            decision.ServerDetails.NotAllowed
                            && target.Attributes.ContainsKey(attr)
                            && target[attr] != null
                        )
                        {
                            throw new InvalidPluginExecutionException(
                                $"Creation blocked by policy: {target.LogicalName}.{attr} is not allowed to be set."
                            );
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message, ex);
            }
        }
    }
}
