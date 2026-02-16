using DataversePolicyEnforcement.Models.Entities;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace DataversePolicyEnforcement.Plugins.PolicyRule
{
    public class ValidatePolicyRulePlugin : PluginBase
    {
        public static List<string> RequiredAttributes = new List<string>
        {
            dpe_PolicyRule.Fields.dpe_TargetEntityLogicalName,
            dpe_PolicyRule.Fields.dpe_TargetAttributeLogicalName,
            dpe_PolicyRule.Fields.dpe_TriggerAttributeLogicalName,
            dpe_PolicyRule.Fields.dpe_PolicyType,
            dpe_PolicyRule.Fields.dpe_Scope,
            dpe_PolicyRule.Fields.dpe_Result
        };

        private const string PreImageName = "PreImage";

        public ValidatePolicyRulePlugin()
            : base(typeof(ValidatePolicyRulePlugin))
        {
            // Not Implemented
        }

        protected override void ExecuteCdsPlugin(ILocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var systemService = localPluginContext.SystemUserService;
            var tracer = localPluginContext.TracingService;

            if (context.MessageName != "Create" && context.MessageName != "Update")
            {
                tracer.Trace("Message is not Create or Update, skipping plugin execution.");
                return;
            }

            if (context.Stage != 10)
            {
                tracer.Trace("Stage is not PreValidation, skipping plugin execution.");
                return;
            }

            if (!context.InputParameters.TryGetValue("Target", out Entity target))
            {
                throw new InvalidPluginExecutionException("Target not found in input parameters");
            }

            Entity preImage = null;
            if (
                context.MessageName == "Update"
                && !context.PreEntityImages.TryGetValue(PreImageName, out preImage)
                && preImage == null
            )
            {
                throw new InvalidPluginExecutionException(
                    $"Pre-image named {PreImageName} required on update"
                );
            }

            if (context.MessageName == "Create")
            {
                foreach (var attr in RequiredAttributes)
                {
                    if (
                        !target.Attributes.ContainsKey(attr)
                        || target[attr] == null
                        || (
                            target[attr].GetType() == typeof(string)
                            && string.IsNullOrWhiteSpace(target[attr].ToString())
                        )
                    )
                    {
                        throw new InvalidPluginExecutionException($"{attr} is required on create");
                    }
                }
            }
            else
            {
                foreach (var attr in RequiredAttributes)
                {
                    if (!target.TryGetAttributeValue(attr, out object value))
                    {
                        preImage.TryGetAttributeValue(attr, out value);
                    }
                    if (
                        value == null
                        || (
                            value.GetType() == typeof(string)
                            && string.IsNullOrWhiteSpace(target[attr].ToString())
                        )
                    )
                    {
                        throw new InvalidPluginExecutionException($"{attr} is required on create");
                    }
                }
            }
        }
    }
}
