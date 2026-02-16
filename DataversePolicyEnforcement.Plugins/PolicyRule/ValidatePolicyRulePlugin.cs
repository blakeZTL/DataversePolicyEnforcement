namespace DataversePolicyEnforcement.Plugins.PolicyRule
{
    public class ValidatePolicyRulePlugin : PluginBase
    {
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
        }
    }
}
