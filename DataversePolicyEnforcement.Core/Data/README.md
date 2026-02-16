# Core Data

This folder contains data access helpers used by the policy engine to read `dpe_PolicyRule` and `dpe_PolicyCondition` records from Dataverse.

Key components
- `PolicyCollection` — implementation of `IPolicyCollection` that queries Dataverse for rules and conditions.
- `IPolicyCollection` — interface used to decouple data access from evaluation logic and to make testing easier.

Behavior highlights
- `GetRules(service, entityLogicalName, attributeLogicalName)` validates arguments, returns only active `dpe_PolicyRule` records that match the entity/attribute, ordered by `dpe_Sequence` ascending.
- `GetConditions(service, ruleId)` validates arguments, returns only active `dpe_PolicyCondition` records for the provided rule ordered by `dpe_Sequence` ascending.

Testing
- Tests in `DataversePolicyEnforcement.Tests/Core/Data/PolicyCollectionTests.cs` use `FakeXrmEasy` to create in-memory entities and validate filtering, active-state, and ordering behavior.
