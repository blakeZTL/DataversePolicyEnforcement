# Evaluation Tests

This folder contains unit tests for policy evaluation logic used to compute attribute-level policy decisions (Required, NotAllowed, Visible). Tests exercise `PolicyEvaluator` and `PolicyScopeEvaluation` using an in-memory `FakeXrmEasy` context to simulate `dpe_PolicyRule` and `dpe_PolicyCondition` records.

Key focuses:
- Validation of null/empty inputs and inactive or missing rules.
- Scope isolation (ServerOnly, FormOnly, Both) and that client/server defaults are preserved when only the opposite scope applies.
- Sequence ordering, first-match semantics, and short-circuit behavior when `NotAllowed = true`.
- Condition matching: rules are only applied when attached conditions evaluate to true.

Important files:
- `PolicyEvaluatorTests.cs` — integration-style tests for end-to-end attribute evaluation.
- `PolicyScopeEvaluatorTests.cs` — focused tests for client and server scope evaluation rules.
- `Helpers.cs` — test helpers to attach met/unmet conditions to rules.

