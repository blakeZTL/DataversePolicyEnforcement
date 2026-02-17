Helpers

This folder contains small helper utilities used by the `DataversePolicyEnforcement.Plugins` project.

## `ValueEquality`

`ValueEquality` provides a single static method `AreEqual(object a, object b)` that performs value-aware
comparisons for common Dataverse/XRM types and primitives. It is used by the plugins and unit tests to
compare field values in a consistent way.

Behavior highlights:
- Unwraps `AliasedValue` instances and compares the underlying `Value`.
- Compares `OptionSetValue` by the integer `Value`.
- Compares `EntityReference` by `Id` and `LogicalName` (logical name comparison is case-insensitive).
- Compares `Money` by `Value`.
- Compares `DateTime` by exact equality (no tolerance applied).
- Falls back to `object.Equals` for primitives and other types.

Usage example:

```csharp
using DataversePolicyEnforcement.Plugins.Helpers;

// Compare a field value to an OptionSetValue
bool equal = ValueEquality.AreEqual(entity["statuscode"], new Microsoft.Xrm.Sdk.OptionSetValue(1));
```

Unit tests
- Tests for this helper are in `DataversePolicyEnforcement.Tests\Plugins\Helpers\ValueEqualityTests.cs`.
  Run them from Visual Studio (MSTest) or your usual test runner for the solution.

Notes
- The project targets .NET Framework 4.6.2; run tests using Visual Studio's test runner for best compatibility.
