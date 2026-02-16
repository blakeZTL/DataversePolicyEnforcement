# Core Comparison

This folder implements comparison helpers used by the policy engine to evaluate whether policy conditions match runtime values.

Primary implementation
- `Condition` (static) — contains:
  - `ConditionValueEquals(dpe_PolicyCondition, object)` — compares a condition's configured value to a runtime value for multiple supported types (String, WholeNumber, Decimal, Boolean, DateTime, Guid, Money, OptionSet, Lookup). String and lookup logical-name comparisons are case-insensitive.
  - `ConditionMatches(...)` — resolves the trigger attribute from the `target` (or `preImage`) and applies the condition operator (`Equals`, `NotEquals`, `IsNull`, `IsNotNull`).
  - `AllConditionsMatch(...)` — evaluates all conditions attached to a rule in `dpe_Sequence` order and returns true when there are no conditions or when every condition matches.

Key behaviors
- Robust null handling: null condition or null runtime value results in false for single comparisons; `AllConditionsMatch` returns true when no conditions are present or trigger attribute is missing.
- Type conversions use `Convert.*` and guard against `InvalidCastException` (returns false on failure).
- Lookup comparisons require both logical name and id to match the provided `EntityReference`.

Tests
- See `DataversePolicyEnforcement.Tests/Core/Comparison/ConditionTests.cs` for comprehensive unit tests covering each supported type and edge cases.
