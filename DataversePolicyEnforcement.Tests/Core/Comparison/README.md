# Comparison Tests

This folder contains unit tests for comparison logic used by the policy engine. Tests primarily exercise `Condition.ConditionValueEquals` to ensure policy condition values are compared correctly against runtime values.

Key focuses:
- Robust null handling (null condition or null compared value returns false where appropriate).
- Support for multiple value types: `String`, `WholeNumber`, `Boolean`, `DateTime`, `Guid`, `Money`, `OptionSet`, and `Lookup`.
- Correct behavior for invalid or unsupported value types.
- Lookup comparisons verify both logical name and id, and gracefully handle missing fields.
- String comparisons are case-insensitive.

Important files:
- `ConditionTests.cs` — exhaustive tests covering expected matches and non-matches for each supported value type and edge cases.
- `Condition.cs` (in `DataversePolicyEnforcement.Core.Comparison`) — implementation under test.
