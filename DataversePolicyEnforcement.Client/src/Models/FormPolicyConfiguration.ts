export interface PolicyConfigurationRule {
  attributeLogicalName: string;
  triggerAttributeLogicalName: string;
}

export interface PolicyConfigurationTriggerGroup {
  triggerAttributeLogicalName: string;
  attributeLogicalNames: string[];
}

export class FormPolicyConfiguration {
  rules: PolicyConfigurationRule[];
  triggerGroups: PolicyConfigurationTriggerGroup[];

  constructor(json?: string | unknown) {
    const data = this.parseJson(json);
    this.rules = (data.rules || []).map((r: any) => this.parseRule(r));
    this.ensureDistinctRules(this.rules);

    if ("triggerGroups" in data) {
      this.triggerGroups = (data.triggerGroups || []).map((g: any) =>
        this.parseTriggerGroup(g),
      );
    } else {
      // build trigger groups from rules: group attributes by triggerAttributeLogicalName
      const map = new Map<string, string[]>();
      for (const r of this.rules) {
        const list = map.get(r.triggerAttributeLogicalName) || [];
        list.push(r.attributeLogicalName);
        map.set(r.triggerAttributeLogicalName, list);
      }
      this.triggerGroups = Array.from(map.entries()).map(([k, v]) => ({
        triggerAttributeLogicalName: k,
        attributeLogicalNames: v,
      }));
    }

    this.ensureDistinctTriggerGroups(this.triggerGroups);
  }

  private parseJson(json?: string | unknown): any {
    if (json === undefined || json === null) {
      return { rules: [], triggerGroups: [] };
    }

    let obj: any;
    if (typeof json === "string") {
      try {
        obj = JSON.parse(json);
      } catch (e) {
        throw new SyntaxError(
          "Invalid JSON string provided to FormPolicyConfiguration constructor",
        );
      }
    } else if (typeof json === "object") {
      obj = json;
    } else {
      throw new TypeError("Constructor expects a JSON string or an object");
    }

    if (!("rules" in obj)) {
      throw new TypeError(
        'Missing required property "rules" in FormPolicyConfiguration JSON',
      );
    }

    if (!Array.isArray(obj.rules)) {
      throw new TypeError(
        '"rules" must be an array in FormPolicyConfiguration JSON',
      );
    }

    if ("triggerGroups" in obj && !Array.isArray(obj.triggerGroups)) {
      throw new TypeError(
        '"triggerGroups" must be an array in FormPolicyConfiguration JSON when present',
      );
    }

    return obj;
  }

  private parseRule(r: any): PolicyConfigurationRule {
    if (!r || typeof r !== "object") {
      throw new TypeError("Each rule must be an object");
    }

    if (
      typeof r.attributeLogicalName !== "string" ||
      !r.attributeLogicalName.trim()
    ) {
      throw new TypeError(
        'Each rule must have a non-empty string "attributeLogicalName"',
      );
    }

    if (
      typeof r.triggerAttributeLogicalName !== "string" ||
      !r.triggerAttributeLogicalName.trim()
    ) {
      throw new TypeError(
        'Each rule must have a non-empty string "triggerAttributeLogicalName"',
      );
    }

    return {
      attributeLogicalName: r.attributeLogicalName,
      triggerAttributeLogicalName: r.triggerAttributeLogicalName,
    };
  }

  private parseTriggerGroup(g: any): PolicyConfigurationTriggerGroup {
    if (!g || typeof g !== "object") {
      throw new TypeError("Each trigger group must be an object");
    }

    if (
      typeof g.triggerAttributeLogicalName !== "string" ||
      !g.triggerAttributeLogicalName.trim()
    ) {
      throw new TypeError(
        'Each trigger group must have a non-empty string "triggerAttributeLogicalName"',
      );
    }

    if (!Array.isArray(g.attributeLogicalNames)) {
      throw new TypeError(
        'Each trigger group must have an array "attributeLogicalNames"',
      );
    }

    const names = g.attributeLogicalNames.map((n: any, i: number) => {
      if (typeof n !== "string" || !n.trim()) {
        throw new TypeError(
          `attributeLogicalNames[${i}] in trigger group "${g.triggerAttributeLogicalName}" must be a non-empty string`,
        );
      }
      return n;
    });

    // ensure no duplicate attribute names within a group
    const seen = new Set<string>();
    for (const n of names) {
      if (seen.has(n)) {
        throw new TypeError(
          `Duplicate attributeLogicalName "${n}" in trigger group "${g.triggerAttributeLogicalName}"`,
        );
      }
      seen.add(n);
    }

    return {
      triggerAttributeLogicalName: g.triggerAttributeLogicalName,
      attributeLogicalNames: names,
    };
  }

  private ensureDistinctRules(rules: PolicyConfigurationRule[]): void {
    const seen = new Set<string>();
    for (const r of rules) {
      const key = `${r.attributeLogicalName}::${r.triggerAttributeLogicalName}`;
      if (seen.has(key)) {
        throw new TypeError(
          `Duplicate rule for attributeLogicalName="${r.attributeLogicalName}" and triggerAttributeLogicalName="${r.triggerAttributeLogicalName}"`,
        );
      }
      seen.add(key);
    }
  }

  private ensureDistinctTriggerGroups(
    groups: PolicyConfigurationTriggerGroup[],
  ): void {
    const seen = new Set<string>();
    for (const g of groups) {
      const key = g.triggerAttributeLogicalName;
      if (seen.has(key)) {
        throw new TypeError(
          `Duplicate trigger group for triggerAttributeLogicalName="${g.triggerAttributeLogicalName}"`,
        );
      }
      seen.add(key);
    }
  }
}
