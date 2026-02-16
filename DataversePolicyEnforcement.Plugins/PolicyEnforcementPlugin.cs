using Microsoft.Xrm.Sdk;

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

            if (
                context.MessageName == "Update"
                && !context.PreEntityImages.TryGetValue(PreImageName, out Entity preImage)
            )
            {
                throw new InvalidPluginExecutionException(
                    $"Pre-Image named {PreImageName} required on update"
                );
            }

            #endregion
        }
    }
}
