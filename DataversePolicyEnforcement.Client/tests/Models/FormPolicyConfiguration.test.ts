import { FormPolicyConfiguration } from "../../src/Models/FormPolicyConfiguration";
import { describe, test, expect } from "vitest";

describe("FormPolicyConfiguration", () => {
  test("constructs with no input -> empty rules and triggerGroups", () => {
    const cfg = new FormPolicyConfiguration();
    expect(cfg.rules).toEqual([]);
    expect(cfg.triggerGroups).toEqual([]);
  });

  test("parses from object with rules only", () => {
    const input = {
      rules: [
        { attributeLogicalName: "name", triggerAttributeLogicalName: "status" },
      ],
    };
    const cfg = new FormPolicyConfiguration(input);
    expect(cfg.rules).toHaveLength(1);
    expect(cfg.rules[0].attributeLogicalName).toBe("name");
    expect(cfg.rules[0].triggerAttributeLogicalName).toBe("status");
    expect(cfg.triggerGroups).toHaveLength(1);
    expect(cfg.triggerGroups[0].triggerAttributeLogicalName).toBe("status");
    expect(cfg.triggerGroups[0].attributeLogicalNames).toEqual(["name"]);
  });

  test("parses from JSON string", () => {
    const input = JSON.stringify({
      rules: [
        {
          attributeLogicalName: "email",
          triggerAttributeLogicalName: "changed",
        },
      ],
      triggerGroups: [
        {
          triggerAttributeLogicalName: "changed",
          attributeLogicalNames: ["email"],
        },
      ],
    });
    const cfg = new FormPolicyConfiguration(input);
    expect(cfg.rules).toHaveLength(1);
    expect(cfg.triggerGroups).toHaveLength(1);
    expect(cfg.triggerGroups[0].triggerAttributeLogicalName).toBe("changed");
  });

  test("throws SyntaxError on invalid JSON string", () => {
    expect(() => new FormPolicyConfiguration("{not json}")).toThrow(
      SyntaxError,
    );
  });

  test("throws when rules property missing", () => {
    expect(() => new FormPolicyConfiguration({})).toThrow(TypeError);
  });

  test("throws when rules is not array", () => {
    expect(
      () => new FormPolicyConfiguration({ rules: "not-an-array" as any }),
    ).toThrow(TypeError);
  });

  test("throws when a rule is not an object", () => {
    expect(() => new FormPolicyConfiguration({ rules: [null as any] })).toThrow(
      TypeError,
    );
  });

  test("throws when rule missing attributeLogicalName", () => {
    expect(
      () =>
        new FormPolicyConfiguration({
          rules: [{ triggerAttributeLogicalName: "x" }] as any,
        }),
    ).toThrow(TypeError);
  });

  test("throws when rule missing triggerAttributeLogicalName", () => {
    expect(
      () =>
        new FormPolicyConfiguration({
          rules: [{ attributeLogicalName: "a" }] as any,
        }),
    ).toThrow(TypeError);
  });

  test("throws on duplicate rules (attribute + trigger pair)", () => {
    const obj = {
      rules: [
        { attributeLogicalName: "a", triggerAttributeLogicalName: "t" },
        { attributeLogicalName: "a", triggerAttributeLogicalName: "t" },
      ],
    };
    expect(() => new FormPolicyConfiguration(obj)).toThrow(TypeError);
  });

  test("parses triggerGroups when absent -> generated from rules", () => {
    const obj = {
      rules: [{ attributeLogicalName: "a", triggerAttributeLogicalName: "t" }],
    };
    const cfg = new FormPolicyConfiguration(obj as any);
    expect(cfg.triggerGroups).toHaveLength(1);
    expect(cfg.triggerGroups[0].triggerAttributeLogicalName).toBe("t");
    expect(cfg.triggerGroups[0].attributeLogicalNames).toEqual(["a"]);
  });

  test("throws when triggerGroups is present but not array", () => {
    const obj = {
      rules: [{ attributeLogicalName: "a", triggerAttributeLogicalName: "t" }],
      triggerGroups: "no" as any,
    };
    expect(() => new FormPolicyConfiguration(obj as any)).toThrow(TypeError);
  });

  test("parses valid trigger group and enforces unique attribute names inside group", () => {
    const obj = {
      rules: [{ attributeLogicalName: "a", triggerAttributeLogicalName: "t" }],
      triggerGroups: [
        { triggerAttributeLogicalName: "t", attributeLogicalNames: ["x", "y"] },
      ],
    };
    const cfg = new FormPolicyConfiguration(obj as any);
    expect(cfg.triggerGroups).toHaveLength(1);
    expect(cfg.triggerGroups[0].attributeLogicalNames).toEqual(["x", "y"]);
  });

  test("throws when trigger group has duplicate attributeLogicalNames", () => {
    const obj = {
      rules: [{ attributeLogicalName: "a", triggerAttributeLogicalName: "t" }],
      triggerGroups: [
        { triggerAttributeLogicalName: "t", attributeLogicalNames: ["x", "x"] },
      ],
    };
    expect(() => new FormPolicyConfiguration(obj as any)).toThrow(TypeError);
  });

  test("throws when trigger group attributeLogicalNames contains non-string or empty", () => {
    const base = {
      rules: [{ attributeLogicalName: "a", triggerAttributeLogicalName: "t" }],
    };
    expect(
      () =>
        new FormPolicyConfiguration({
          ...base,
          triggerGroups: [
            {
              triggerAttributeLogicalName: "t",
              attributeLogicalNames: [123] as any,
            },
          ],
        } as any),
    ).toThrow(TypeError);
    expect(
      () =>
        new FormPolicyConfiguration({
          ...base,
          triggerGroups: [
            { triggerAttributeLogicalName: "t", attributeLogicalNames: [""] },
          ],
        } as any),
    ).toThrow(TypeError);
  });

  test("throws on duplicate triggerGroups by triggerAttributeLogicalName", () => {
    const obj = {
      rules: [{ attributeLogicalName: "a", triggerAttributeLogicalName: "t" }],
      triggerGroups: [
        { triggerAttributeLogicalName: "t", attributeLogicalNames: ["x"] },
        { triggerAttributeLogicalName: "t", attributeLogicalNames: ["y"] },
      ],
    };
    expect(() => new FormPolicyConfiguration(obj as any)).toThrow(TypeError);
  });
});
