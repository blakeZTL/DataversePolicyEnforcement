using DataversePolicyEnforcement.Core.Data;
using DataversePolicyEnforcement.Models;
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

            #region Plugin Execution Context
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
            #endregion

            #region Plugin Resources
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

            #endregion

            dpe_PolicyRule policyRule = new dpe_PolicyRule();

            #region Required Column Validation
            if (context.MessageName == "Create")
            {
                tracer.Trace("Validating Create");
                foreach (var attr in RequiredAttributes)
                {
                    tracer.Trace($"Validating {attr}");
                    if (
                        !target.Attributes.ContainsKey(attr)
                        || target[attr] == null
                        || (target[attr] is string s && string.IsNullOrWhiteSpace(s))
                    )
                    {
                        throw new InvalidPluginExecutionException($"{attr} is required");
                    }
                    policyRule[attr] = target[attr];
                    tracer.Trace($"{attr} valid");
                }
            }
            else
            {
                tracer.Trace("Validating Update");
                foreach (var attr in RequiredAttributes)
                {
                    tracer.Trace($"Validating {attr}");
                    if (!target.TryGetAttributeValue(attr, out object value))
                    {
                        tracer.Trace($"{attr} not in target. Checking pre-image");
                        preImage?.TryGetAttributeValue(attr, out value);
                    }
                    tracer.Trace($"{attr}: {value}");
                    if (value == null || (value is string s && string.IsNullOrWhiteSpace(s)))
                    {
                        throw new InvalidPluginExecutionException($"{attr} is required");
                    }
                    policyRule[attr] = value;
                    tracer.Trace($"{attr} valid");
                }
            }
            #endregion

            #region Metadata Validation

            var metaValidator = new MetadataValidator(systemService);

            tracer.Trace(
                $"Validating {dpe_PolicyRule.Fields.dpe_TargetEntityLogicalName} with value of {policyRule.dpe_TargetEntityLogicalName}..."
            );
            if (!metaValidator.ValidateEntity(policyRule.dpe_TargetEntityLogicalName))
            {
                throw new InvalidPluginExecutionException(
                    $"{policyRule.dpe_TargetEntityLogicalName} is not a valid entity"
                );
            }
            tracer.Trace(
                $"Validating {dpe_PolicyRule.Fields.dpe_TargetAttributeLogicalName} with value of {policyRule.dpe_TargetAttributeLogicalName}..."
            );
            if (
                !metaValidator.ValidateAttribute(
                    policyRule.dpe_TargetEntityLogicalName,
                    policyRule.dpe_TargetAttributeLogicalName
                )
            )
            {
                throw new InvalidPluginExecutionException(
                    $"{policyRule.dpe_TargetAttributeLogicalName} is not a valid attribute of {policyRule.dpe_TargetEntityLogicalName}"
                );
            }
            tracer.Trace(
                $"Validating {dpe_PolicyRule.Fields.dpe_TriggerAttributeLogicalName} with value of {policyRule.dpe_TriggerAttributeLogicalName}..."
            );
            if (
                !metaValidator.ValidateAttribute(
                    policyRule.dpe_TargetEntityLogicalName,
                    policyRule.dpe_TriggerAttributeLogicalName
                )
            )
            {
                throw new InvalidPluginExecutionException(
                    $"{policyRule.dpe_TriggerAttributeLogicalName} is not a valid attribute of {policyRule.dpe_TargetEntityLogicalName}"
                );
            }

            #endregion

            #region Validate Scope

            if (
                policyRule.dpe_PolicyType == dpe_policytype.Visible
                && policyRule.dpe_Scope == dpe_policyscope.ServerOnly
            )
            {
                throw new InvalidPluginExecutionException(
                    "A visibility rule can only be applied to the form or both"
                );
            }

            #endregion
        }
    }
}
