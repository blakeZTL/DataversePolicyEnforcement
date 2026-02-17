import type { AttributePolicyDecision } from "../Models/AttributePolicyDecision";

export interface IPolicyService {
  getDecisionsAsync(
    entityLogicalName: string,
    attributeLogicalName: string,
  ): Promise<AttributePolicyDecision[]>;

  getAttributeDecisionsAsync(
    entityLogicalName: string,
    triggerAttributeLogicalName: string,
    currentValue: any,
    attributeLogicalNames: string[],
  ): Promise<AttributePolicyDecision[]>;
}
