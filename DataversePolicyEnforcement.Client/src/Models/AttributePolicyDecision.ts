import type { ClientPolicyDetails } from "./ClientPolicyDetails";

export interface AttributePolicyDecision {
  attributeLogicalName: string;
  triggerAttributeLogicalName: string;
  policyDetails: ClientPolicyDetails;
}
