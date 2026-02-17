import { AttributePolicyDecision } from "../Models";
import type { IPolicyService } from "./IPolicyService";

export class PolicyService implements IPolicyService {
  private _xrmWebApi: Xrm.WebApi;

  constructor(xrmWebApi: Xrm.WebApi) {
    this._xrmWebApi = xrmWebApi;
  }
  getAttributeDecisionsAsync(
    entityLogicalName: string,
    triggerAttributeLogicalName: string,
    currentValue: any,
    attributeLogicalNames: string[],
  ): Promise<AttributePolicyDecision[]> {
    return new Promise((resolve) => {
      setTimeout(() => {
        resolve([
          {
            attributeLogicalName: attributeLogicalNames[0],
            triggerAttributeLogicalName: triggerAttributeLogicalName,
            policyDetails: {
              visible: true,
              required: false,
              notAllowed: false,
            },
          },
        ]);
      }, 250);
    });
  }

  async getDecisionsAsync(
    entityLogicalName: string,
    attributeLogicalName: string,
  ): Promise<AttributePolicyDecision[]> {
    await new Promise((resolve) => setTimeout(resolve, 1000));

    return [
      {
        attributeLogicalName: attributeLogicalName,
        triggerAttributeLogicalName: "triggerAttribute",
        policyDetails: {
          visible: true,
          required: false,
          notAllowed: false,
        },
      },
    ];
  }
}
