Helpers - Tests

This document describes the unit tests covering the helper classes in the `DataversePolicyEnforcement.Plugins\Helpers` folder.

What is tested
- `ValueEquality` is covered by `DataversePolicyEnforcement.Tests\Plugins\Helpers\ValueEqualityTests.cs`.
  - Tests verify behavior for nulls and reference equality, primitives, `AliasedValue` unwrapping, `OptionSetValue`,
    `EntityReference` (id + logical name), `Money`, and `DateTime`.

How to run tests
- The tests use MSTest. Run them with Visual Studio's Test Explorer (recommended for .NET Framework 4.6.2 projects).
- Alternatively, use the VSTest.Console runner that ships with Visual Studio if you need CLI execution.

How to add tests
- Place new tests under `DataversePolicyEnforcement.Tests\Plugins\Helpers`.
- Use the existing test class patterns (`[TestClass]`, `[TestMethod]`, and `[TestCategory]`).
- Write small, focused tests for each behavior of a helper method and keep assertions explicit.

Tips
- Reuse the `Microsoft.Xrm.Sdk` helper types (for example `AliasedValue`, `OptionSetValue`, `EntityReference`, `Money`) to construct realistic test values.
- When adding new helper behavior, add tests for expected edge cases (nulls, type mismatches, case-sensitivity where relevant).
