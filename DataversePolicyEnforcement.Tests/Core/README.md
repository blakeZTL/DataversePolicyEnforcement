# Core Tests

This folder groups unit tests for the core pieces of the policy engine used by the project. Tests exercise data access, comparison logic, and policy evaluation using an in-memory `FakeXrmEasy` context to simulate Dataverse entities.

Subfolders:
- `Data` — tests for `PolicyCollection` data access behavior (filtering, active-state, sequence ordering). See `PolicyCollectionTests.cs`.
- `Comparison` — tests for value comparison logic used by policy conditions (multiple value types and edge cases). See `ConditionTests.cs`.
- `Evaluation` — integration-style and focused tests for `PolicyEvaluator` and `PolicyScopeEvaluation` covering scope isolation, sequence/first-match semantics, and condition matching. See `PolicyEvaluatorTests.cs` and `PolicyScopeEvaluatorTests.cs`.

Each test suite is intended to validate a single responsibility of the policy engine so they can be run and reasoned about independently.
