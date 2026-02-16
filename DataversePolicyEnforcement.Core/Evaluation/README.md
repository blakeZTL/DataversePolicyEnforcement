# Core Evaluation

This folder implements the core policy evaluation logic used to compute attribute-level policy outcomes.

Key components
- `PolicyEvaluator` — orchestration entry point that evaluates a single attribute and returns a `PolicyDecision` (see `DataversePolicyEnforcement.Core.Model`).
- `PolicyScopeEvaluation` — evaluates rules within a specific scope (client or server) and produces `ClientPolicyDetails` / `ServerPolicyDetails`.
- `IPolicyEvaluator`, `IPolicyScopeEvaluator` — interfaces used for testing and decoupling.

Behavior highlights
- Rules are evaluated in `dpe_Sequence` order; conditions attached to rules must match for a rule to apply.
- Scope isolation: rules with `ServerOnly`, `FormOnly`, or `Both` control whether they affect server or client decisions.
- First-match semantics for `Required`/`Visible` and short-circuit evaluation when `NotAllowed = true`.

Usage
- The evaluator consumes `dpe_PolicyRule` and `dpe_PolicyCondition` entities (from the generated models) and returns a `PolicyDecision` describing client and server outcomes.
