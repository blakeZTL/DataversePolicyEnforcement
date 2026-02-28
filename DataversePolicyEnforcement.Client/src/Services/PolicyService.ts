import { AttributePolicyDecision } from "../Models";
import type { IPolicyService } from "./IPolicyService";

export class PolicyService implements IPolicyService {
  private _xrmWebApi: Xrm.WebApi;

  constructor(xrmWebApi: Xrm.WebApi) {
    this._xrmWebApi = xrmWebApi;
  }

  async getAttributeDecisionsAsync(
    entityLogicalName: string,
    triggerAttributeLogicalName: string,
    currentValue: any,
    attributeLogicalNames: string[],
  ): Promise<AttributePolicyDecision[]> {
    var execute_dpe_GetAttributeDecisions_Request = {
      // Parameters
      dpe_gad_entitylogicalname: entityLogicalName, // Edm.String
      dpe_gad_targetattributelogicalnames: attributeLogicalNames, // Collection(Edm.String)
      dpe_gad_triggerattributelogicalname: triggerAttributeLogicalName, // Edm.String
      dpe_gad_trigger_currentvalue: String(currentValue), // Edm.String

      getMetadata: function () {
        return {
          parameterTypes: {
            dpe_gad_entitylogicalname: {
              typeName: "Edm.String",
              structuralProperty: 1,
            },
            dpe_gad_targetattributelogicalnames: {
              typeName: "Collection(Edm.String)",
              structuralProperty: 4,
            },
            dpe_gad_triggerattributelogicalname: {
              typeName: "Edm.String",
              structuralProperty: 1,
            },
            dpe_gad_trigger_currentvalue: {
              typeName: "Edm.String",
              structuralProperty: 1,
            },
          },
          operationType: 0, // Action
          operationName: "dpe_GetAttributeDecisions",
        };
      },
    };

    var response = await this._xrmWebApi.online.execute(
      execute_dpe_GetAttributeDecisions_Request,
    );

    if (response.ok) {
      var result = await response.json();
      console.debug("Policy Service Result:", result);
      return result[
        "dpe_gad_attributedecision_results"
      ] as AttributePolicyDecision[];
    }
    // return new Promise((resolve) => {
    //   setTimeout(() => {
    //     resolve([
    //       {
    //         attributeLogicalName: attributeLogicalNames[0],
    //         triggerAttributeLogicalName: triggerAttributeLogicalName,
    //         policyDetails: {
    //           visible: true,
    //           required: false,
    //           notAllowed: false,
    //         },
    //       },
    //     ]);
    //   }, 250);
    // });
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
