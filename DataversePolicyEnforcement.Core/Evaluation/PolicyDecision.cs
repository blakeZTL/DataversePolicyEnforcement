namespace DataversePolicyEnforcement.Core.Evaluation
{
    public sealed class PolicyDecision
    {
        public ServerPolicyDetails ServerDetails { get; set; } = new ServerPolicyDetails();
        public ClientPolicyDetails ClientDetails { get; set; } = new ClientPolicyDetails();
    }
}
