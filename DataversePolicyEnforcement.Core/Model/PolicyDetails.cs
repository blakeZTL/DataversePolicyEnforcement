namespace DataversePolicyEnforcement.Core.Model
{
    public sealed class ClientPolicyDetails
    {
        public bool Visible { get; set; } = true;
        public bool Required { get; set; } = false;
        public bool NotAllowed { get; set; } = false;
    }

    public sealed class ServerPolicyDetails
    {
        public bool Required { get; set; } = false;
        public bool NotAllowed { get; set; } = false;
    }
}
