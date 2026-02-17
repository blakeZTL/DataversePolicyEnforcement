import { OptionSetAttributeMock, XrmMockGenerator } from "xrm-mock";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { onLoad } from "../../src/Form/OnLoad";
import { PolicyService } from "../../src/Services";

let eventContext: Xrm.Events.EventContext;
let formContext: Xrm.FormContext;

let mockDecision = {
  attributeLogicalName: "name",
  triggerAttributeLogicalName: "status",
  policyDetails: {
    visible: true,
    required: true,
    notAllowed: false,
  },
};
let mockRule = {
  attributeLogicalName: "name",
  triggerAttributeLogicalName: "status",
  policyDetails: {
    visible: true,
    required: true,
    notAllowed: false,
  },
};

let triggerAttribute: OptionSetAttributeMock;

describe("Form OnLoad", () => {
  beforeEach(() => {
    XrmMockGenerator.initialise();
    eventContext = XrmMockGenerator.getEventContext();
    formContext = eventContext.getFormContext();

    const nameAttribute = XrmMockGenerator.Attribute.createString(
      "name",
      "Test Name",
    );
    nameAttribute.setRequiredLevel("none");

    triggerAttribute = XrmMockGenerator.Attribute.createOptionSet("status", 1, [
      { text: "Active", value: 1 },
      { text: "Inactive", value: 2 },
    ]);
    const nameControl = XrmMockGenerator.Control.createString(nameAttribute);
    nameControl.setDisabled(true);
    nameControl.setVisible(false);
    XrmMockGenerator.Control.createOptionSet(triggerAttribute);
  });

  it("should throw an error if config is invalid JSON", () => {
    const invalidJson = "{invalid: json}";
    expect(() => onLoad(eventContext, invalidJson)).toThrow(SyntaxError);
  });

  it("should log a warning if no rules are defined", () => {
    const consoleWarnSpy = vi
      .spyOn(console, "warn")
      .mockImplementation(() => {});
    onLoad(eventContext, JSON.stringify({ rules: [] }));
    expect(consoleWarnSpy).toHaveBeenCalledWith(
      "No policy rules defined in configuration",
    );
    consoleWarnSpy.mockRestore();
  });

  it("should log a warning if trigger attribute is not found", () => {
    const consoleWarnSpy = vi
      .spyOn(console, "warn")
      .mockImplementation(() => {});
    const rule = { ...mockRule };
    rule.triggerAttributeLogicalName = "nonexistent";
    const config = {
      rules: [rule],
    };
    onLoad(eventContext, JSON.stringify(config));
    expect(consoleWarnSpy).toHaveBeenCalledWith(
      "Trigger attribute 'nonexistent' not found for rule on attribute 'name'",
    );
    consoleWarnSpy.mockRestore();
  });

  it("should log a warning if affected attribute is not found", async () => {
    const consoleWarnSpy = vi
      .spyOn(console, "warn")
      .mockImplementation(() => {});

    const config = {
      rules: [mockRule],
    };
    const decision = { ...mockDecision };
    decision.attributeLogicalName = "nonexistent";
    const getAttributeDecisionsAsyncSpy = vi
      .spyOn(PolicyService.prototype, "getAttributeDecisionsAsync")
      .mockResolvedValue([decision]);

    onLoad(eventContext, JSON.stringify(config));
    triggerAttribute.fireOnChange();
    await getAttributeDecisionsAsyncSpy.mock.results[0].value;
    expect(consoleWarnSpy).toHaveBeenCalledWith(
      "Affected attribute nonexistent not found on form",
    );
    consoleWarnSpy.mockRestore();
    getAttributeDecisionsAsyncSpy.mockRestore();
  });

  it("should apply policy decisions to controls", async () => {
    const consoleDebugSpy = vi
      .spyOn(console, "debug")
      .mockImplementation(() => {});

    const config = {
      rules: [mockRule],
    };

    const getAttributeDecisionsAsyncSpy = vi
      .spyOn(PolicyService.prototype, "getAttributeDecisionsAsync")
      .mockResolvedValue([mockDecision]);

    onLoad(eventContext, JSON.stringify(config));
    triggerAttribute.fireOnChange();
    await getAttributeDecisionsAsyncSpy.mock.results[0].value;
    const nameAttribute = formContext.getAttribute("name");
    expect(nameAttribute?.getRequiredLevel()).toBe("required");
    const nameControl = nameAttribute.controls.get(0);
    expect(nameControl?.getDisabled()).toBe(false);
    expect(nameControl?.getVisible()).toBe(true);
    consoleDebugSpy.mockRestore();
    getAttributeDecisionsAsyncSpy.mockRestore();
  });
});
