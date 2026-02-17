# Data Tests

This folder contains unit tests for the data access helpers used by the policy engine. Tests exercise `PolicyCollection` behavior using an in-memory `FakeXrmEasy` context to simulate `dpe_PolicyRule` and `dpe_PolicyCondition` records.

Key focuses:
- Argument validation (null or empty parameters throw the expected exceptions).
- Filtering by entity and attribute and returning only active rules/conditions.
- Ordering by sequence so evaluation honors rule/condition order.

Important files:
 - `PolicyCollectionTests.cs` — tests for `PolicyCollection.GetRules` and `PolicyCollection.GetConditions` covering validation, filtering, active-state, and sequence ordering.
 - `MetadataValidatorTests.cs` — tests for `MetadataValidator` (implementation of `IMetadataValidator`) that use `FakeXrmEasy` metadata initialization to validate `ValidateAttribute`, `ValidateAllAttributes`, and `ValidateEntity` behavior including argument validation, successful lookups, and missing/invalid metadata scenarios.
