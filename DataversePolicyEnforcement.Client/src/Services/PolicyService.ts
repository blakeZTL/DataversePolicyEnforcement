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
    console.debug(
      "PolicyService: Getting attribute decisions for entity",
      entityLogicalName,
      triggerAttributeLogicalName,
      currentValue,
      attributeLogicalNames,
    );

    let valueForApi: string;
    let lookupEntityName: string | null = null;

    if (Array.isArray(currentValue)) {
      const lookupValues = currentValue as Xrm.LookupValue[];
      const firstValue = lookupValues[0];
      valueForApi = firstValue?.id || "";
      lookupEntityName = firstValue?.entityType || null;
    } else {
      valueForApi = String(currentValue);
    }

    var execute_dpe_GetAttributeDecisions_Request = {
      // Parameters
      dpe_gad_entitylogicalname: entityLogicalName, // Edm.String
      dpe_gad_targetattributelogicalnames: attributeLogicalNames, // Collection(Edm.String)
      dpe_gad_triggerattributelogicalname: triggerAttributeLogicalName, // Edm.String
      dpe_gad_trigger_currentvalue: valueForApi, // Edm.String
      dpe_gad_triggercurrentvalue_lookup_logicalname: lookupEntityName, // Edm.String

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
            dpe_gad_triggercurrentvalue_lookup_logicalname: {
              typeName: "Edm.String",
              structuralProperty: 1,
            },
          },
          operationType: 0, // Action
          operationName: "dpe_GetAttributeDecisions",
        };
      },
    };

    console.debug(
      "PolicyService: Executing request for attribute decisions",
      execute_dpe_GetAttributeDecisions_Request,
    );

    var response = await this._xrmWebApi.online.execute(
      execute_dpe_GetAttributeDecisions_Request,
    );

    if (!response.ok) {
      throw new Error("Failed to retrieve attribute policy decisions.");
    }

    const result = await response.json();
    console.debug("Policy Service Result:", result);

    const raw = result["dpe_gad_attributedecision_results"];

    // Handles both: already-array and JSON-string payloads
    const parsed: any[] =
      typeof raw === "string" ? JSON.parse(raw) : Array.isArray(raw) ? raw : [];

    return parsed.map((d) => ({
      attributeLogicalName: d.AttributeLogicalName ?? d.attributeLogicalName,
      triggerAttributeLogicalName:
        d.TriggerAttributeLogicalName ?? d.triggerAttributeLogicalName,
      policyDetails: {
        visible:
          d.ClientPolicyDetails?.Visible ?? d.policyDetails?.visible ?? true,
        required:
          d.ClientPolicyDetails?.Required ?? d.policyDetails?.required ?? false,
        notAllowed:
          d.ClientPolicyDetails?.NotAllowed ??
          d.policyDetails?.notAllowed ??
          false,
      },
    }));
  }

  // async getDecisionsAsync(
  //   entityLogicalName: string,
  //   attributeLogicalName: string,
  // ): Promise<AttributePolicyDecision[]> {
  //   await new Promise((resolve) => setTimeout(resolve, 1000));

  //   return [
  //     {
  //       attributeLogicalName: attributeLogicalName,
  //       triggerAttributeLogicalName: "triggerAttribute",
  //       policyDetails: {
  //         visible: true,
  //         required: false,
  //         notAllowed: false,
  //       },
  //     },
  //   ];
  // }
}
