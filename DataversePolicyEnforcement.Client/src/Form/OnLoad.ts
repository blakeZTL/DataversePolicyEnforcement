import { FormPolicyConfiguration } from "../Models";
import { PolicyService } from "../Services";

export function onLoad(
  executionContext: Xrm.Events.EventContext,
  config: object | string | undefined,
) {
  const policyConfig = new FormPolicyConfiguration(config);
  if (!policyConfig.rules.length) {
    console.warn("No policy rules defined in configuration");
    return;
  }
  console.debug(policyConfig);
  const formContext = executionContext.getFormContext();

  const policyService = new PolicyService(Xrm.WebApi);

  policyConfig.triggerGroups.forEach((tg) => {
    console.debug(
      `Setting up trigger group for ${tg.triggerAttributeLogicalName}`,
    );
    const attribute = formContext.getAttribute(
      tg.triggerAttributeLogicalName,
    ) as Xrm.Attributes.Attribute<any>;
    if (!attribute) {
      // warn for each associated rule/affected attribute so tests can assert per-rule warnings
      tg.attributeLogicalNames.forEach((attrName) => {
        console.warn(
          `Trigger attribute '${tg.triggerAttributeLogicalName}' not found for rule on attribute '${attrName}'`,
        );
      });
      return;
    }

    attribute.addOnChange(() => {
      const currentValue = attribute.getValue();
      const affectedAttributes = tg.attributeLogicalNames;
      const decisions = policyService.getAttributeDecisionsAsync(
        formContext.data.entity.getEntityName(),
        tg.triggerAttributeLogicalName,
        currentValue,
        affectedAttributes,
      );

      decisions.then((decisions) => {
        console.debug("Policy decisions", decisions);
        decisions.forEach((d) => {
          const attr = formContext.getAttribute(d.attributeLogicalName);
          console.debug(
            `Applying policy decision for ${d.attributeLogicalName}`,
            d.policyDetails,
            attr,
          );

          if (!attr) {
            console.warn(
              `Affected attribute ${d.attributeLogicalName} not found on form`,
            );
            return;
          }
          const details = d.policyDetails;
          const control = attr.controls.get(0);
          attr.setRequiredLevel(details.required ? "required" : "none");
          control.setVisible(details.visible);
          control.setDisabled(details.notAllowed);
        });
      });
    });
  });
}
